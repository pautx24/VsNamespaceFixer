using System.IO;

namespace NamespaceFixer.Extensions
{
    public static class PathExtensions
    {
        public static bool IsProjectFile(this string path)
        {
            return Path.GetExtension(path) == Statics.CsProjectFileExtension;
        }

        public static bool IsProjectFile(this FileInfo fileInfo)
        {
            return fileInfo.Extension.EndsWith(Statics.CsProjectFileExtension);
        }
    }
}