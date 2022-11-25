using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tinyMangaViewer.AddOn
{
    [Export(typeof(IArchiveSource))]
    [ExportMetadata(nameof(IArchiveSourceData.Extensions), new string[0] )]
    class DirectoryArchiveSource : IArchiveSource
    {
        public Stream GetStream(string filename)
        {
            return File.OpenRead(filename);
        }

        public IEnumerable<string> Open(string filename)
        {
            if (!Directory.Exists(filename))
                return Enumerable.Empty<string>();
            return Directory.EnumerateFiles(filename, "*.*", SearchOption.AllDirectories);
        }

        public void Close()
        { }
    }
}
