using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tinyMangaViewer
{
    public interface IArchiveSource
    {
        IEnumerable<string> Open(string filename);
        Stream GetStream(string filename);
        void Close();
    }

    public interface IArchiveSourceData
    {
        string[] Extensions { get; }
    }
}
