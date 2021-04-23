using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using PropertyChanged;
using System.Diagnostics;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace tinyMangaViewer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BitmapSource Image { get; set; }
        public RelayCommand Prev { get; set; }
        public RelayCommand Next { get; set; }
        public RelayCommand Maximize { get; set; }

        public RelayCommand<DragEventArgs> Drop { get; set; }
        public RelayCommand<DragEventArgs> DragOver { get; set; }

        private CompositionContainer container;
        private IArchiveSource zip;
        private ReadOnlyCollection<string> entries;
        public WindowStyle WindowStyle { get; set; }

        [ImportMany]
        private IEnumerable<Lazy<IArchiveSource, IArchiveSourceData>> archiveSource { get; set; }

        private void DoImport()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
            container = new CompositionContainer(catalog);
            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Debug.WriteLine(compositionException);
            }
        }

        public MainViewModel()
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            DoImport();

            Drop = new RelayCommand<DragEventArgs>(ev =>
            {
                ev.Handled = true;
                var filename = ((DataObject)ev.Data).GetFileDropList()[0];
                Open(filename);
            });

            DragOver = new RelayCommand<DragEventArgs>(ev =>
            {
                ev.Handled = true;
                var filename = ((DataObject)ev.Data).GetFileDropList()[0];
                ev.Effects = DragDropEffects.None;
                foreach (var i in archiveSource)
                {
                    if (i.Metadata.Extension.ToLower() == Path.GetExtension(filename).ToLower())
                    {
                        ev.Effects = DragDropEffects.Link;
                        break;
                    }
                }
            });

            Next = new RelayCommand(obj =>
            {
                Current++;
            });

            Prev = new RelayCommand(obj =>
            {
                Current--;
            });

            Maximize = new RelayCommand(obj =>
            {
                if (WindowStyle == WindowStyle.None)
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                }
                else
                {
                    WindowStyle = WindowStyle.None;
                }
            });

            Reset();
            if (!string.IsNullOrWhiteSpace(App.Argument))
            {
                Open(App.Argument);
            }
        }

        private int current;
        [DoNotNotify]
        public int Current 
        {
            get
            {
                return current;
            }
            set
            {
                if (zip == null || value < 0 || value >= entries.Count || value == current)
                    return;
                current = value;
                Display();
                OnPropertyChanged();
            }
        }

        private void Reset()
        {
            current = -1;
            Entry = null;
        }

        private void Open(string filename)
        {
            if (zip != null)
            {
                zip.Close();
                zip = null;
            }
            foreach (var i in archiveSource)
            {
                if (string.IsNullOrEmpty(i.Metadata.Extension) && Directory.Exists(filename))
                {
                    zip = i.Value;
                    break;
                }
                if (string.Compare(i.Metadata.Extension, Path.GetExtension(filename), true) == 0)
                {
                    zip = i.Value;
                    break;
                }    
            }
            if (zip == null)
                return;

            Reset();
            entries = Filter(zip.Open(filename));

            Count = entries.Count;
            Current = 0;
        }

        private void Display()
        {
            Entry = entries[Current];
            using (var stream = zip.GetStream(Entry))
            {
                Image = BitmapFrame.Create(stream, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
            }
        }

        private ReadOnlyCollection<string> Filter(IEnumerable<string> list)
        {
            var validEntries = list.Where(item => 
                                WICHelper.Info.Value.Any(decoder => 
                                    decoder.FileExtensions.Any(ext => 
                                        string.Compare(Path.GetExtension(ext), Path.GetExtension(item), true) == 0)));

            var sortedEntries = validEntries.OrderBy(name => Path.GetDirectoryName(name))
                                    .ThenBy(name =>
                                    {
                                        if (int.TryParse(Path.GetFileNameWithoutExtension(name), out int i))
                                        {
                                            return i;
                                        }
                                        return int.MaxValue;
                                    }).ThenBy(name => name);

            return new ReadOnlyCollection<string>(sortedEntries.ToList());
        }

        public string Entry { get; set; }
        public int Skip { get; set; }
        public int Count { get; set; }
        private void OnCountChanged()
        {
            Skip = Math.Max(Count / 20, 1);
        }
    }
}
