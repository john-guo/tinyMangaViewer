using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace tinyMangaViewer
{
    public class ArchiveSourceCache : IDisposable
    {
        private IArchiveSource _source;
        private ReadOnlyCollection<string> _entries;
        private const int _cacheNum = 5;
        private Func<IFilter> _filter;

        public int Count => _entries?.Count ?? 0;
        public string Entry { get; set; }

        private ConcurrentDictionary<int, BitmapSource> _cache;
        private Task _cacheTask;
        private volatile int _current;
        private ManualResetEventSlim _event;
        private CancellationTokenSource _cts;

        public ArchiveSourceCache(Func<IFilter> filter)
        {
            _filter = filter;
            _cache = new ConcurrentDictionary<int, BitmapSource>(2, _cacheNum * 2 + 1);
            _cts = new CancellationTokenSource();
            _event = new ManualResetEventSlim();

            _cacheTask = new Task(() =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        _event.Wait(_cts.Token);
                        _event.Reset();

                        int current = _current;
                        int start = Math.Max(0, current - _cacheNum);
                        int end = Math.Min(Count - 1, current + _cacheNum);
                        List<int> expiredKeys = new List<int>();
                        foreach (var pair in _cache)
                        {
                            if (pair.Key < start || pair.Key > end)
                            {
                                expiredKeys.Add(pair.Key);
                            }
                        }

                        foreach (int key in expiredKeys)
                        {
                            _cache.TryRemove(key, out BitmapSource _);
                        }

                        for (int i = start; i <= end; ++i)
                        {
                            if (_cache.ContainsKey(i))
                                continue;
                            CacheImage(i, out string _);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

            }, _cts.Token, TaskCreationOptions.LongRunning);

            _cacheTask.Start();
        }

        public void Open(string filename)
        {
            _entries = Filter(_source.Open(filename));
            NotifyCacheTask(0);
        }

        public void SetSource(IArchiveSource source)
        {
            if (_source != null)
                _source.Close();
            _source = source;
            _cache.Clear();
            _event.Reset();
            _current = 0;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cacheTask.Wait();
            _cacheTask.Dispose();
            _event.Dispose();
            _cts.Dispose();
            _cache.Clear();
        }

        private void NotifyCacheTask(int index)
        {
            _current = index;
            _event.Set();
        }

        private BitmapSource CacheImage(int index, out string entry)
        {
            entry = _entries[index];
            BitmapSource image;
            if (!_cache.TryGetValue(index, out image))
            {
                lock (_source)
                {
                    image = _cache.GetOrAdd(index, i =>
                    {
                        try
                        {
                            using (var stream = _source.GetStream(_entries[i]))
                            using (var mat = Mat.FromStream(stream, ImreadModes.Color))
                            {
                                var bitmap = mat.ToBitmapSource();
                                bitmap.Freeze();
                                return bitmap;
                                //return BitmapFrame.Create(stream, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
                            }
                        }
                        catch { return null; }
                    });
                }
            }
            return image;
        }

        public BitmapSource GetImage(int index)
        {
            var image = CacheImage(index, out string entry);
            Entry = entry;

            NotifyCacheTask(index);
            return image;
        }

        private ReadOnlyCollection<string> Filter(IEnumerable<string> list)
        {
            var validEntries = list.Where(item =>
                                WICHelper.Info.Value.Any(decoder =>
                                    decoder.FileExtensions.Any(ext =>
                                        string.Compare(Path.GetExtension(ext), Path.GetExtension(item), true) == 0)));

            var sortedEntries = _filter().Filter(validEntries);

            return new ReadOnlyCollection<string>(sortedEntries.ToList());
        }
    }
}
