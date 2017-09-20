using System.IO;

namespace NamespaceFixer.Extensions
{
    public static class PathExtensions
    {
        public static bool IsProjectFile(this string path)
        {
            return Path.GetExtension(path) == Statics.ProjectFileExtension;
        }

        public static bool IsProjectFile(this FileInfo fileInfo)
        {
            return fileInfo.Extension.EndsWith(Statics.ProjectFileExtension);
        }
    }
}