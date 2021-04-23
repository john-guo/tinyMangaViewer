using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.ComponentModel.Composition;
using SharpCompress.Archives.Rar;

namespace tinyMangaViewer.AddOn
{
    [Export(typeof(IArchiveSource))]
    [ExportMetadata(nameof(IArchiveSourceData.Extension), ".rar")]
    public class RarArchiveSource : IArchiveSource
    {
        private RarArchive zip;
        public int Length => zip?.Entries.Count(entry => !entry.IsDirectory) ?? 0;

        public Stream GetStream(string filename)
        {
            var entry = zip.Entries.FirstOrDefault(e => e.Key == filename);
            if (entry == null)
                throw new Exception($"Not found {filename}");
            var stream = new MemoryStream((int)entry.Size);
            using (var ds = entry.OpenEntryStream())
            {
                ds.CopyTo(stream);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public IEnumerable<string> Open(string filename)
        {
            zip = RarArchive.Open(filename);
            return zip.Entries.Where(entry => !entry.IsDirectory).Select(entry => entry.Key);
        }

        public void Close()
        {
            zip?.Dispose();
        }
    }
}
