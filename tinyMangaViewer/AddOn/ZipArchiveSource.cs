using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.ComponentModel.Composition;

namespace tinyMangaViewer.AddOn
{
    [Export(typeof(IArchiveSource))]
    [ExportMetadata(nameof(IArchiveSourceData.Extension), ".zip")]
    public class ZipArchiveSource : IArchiveSource
    {
        private ZipArchive zip;
        public int Length => zip?.Entries.Count ?? 0;

        public Stream GetStream(string filename)
        {
            var entry = zip.GetEntry(filename);
            var stream = new MemoryStream((int)entry.Length);
            using (var ds = entry.Open())
            {
                ds.CopyTo(stream);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public IEnumerable<string> Open(string filename)
        {
            zip = ZipFile.OpenRead(filename);
            return zip.Entries.Select(entry => entry.FullName);
        }

        public void Close()
        {
            zip?.Dispose();
        }
    }
}
