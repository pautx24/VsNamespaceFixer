using System.IO;
using System.IO.Extensions;

namespace NamespaceFixer.InnerPathFinder
{
    internal class CsInnerPathFinderService : InnerPathFinderService
    {
        internal override string GetHiddenFilesRegex(FileInfo file)
        {
            return file.NameWithoutExtension() + "\\.\\w+\\.cs";
        }
    }
}