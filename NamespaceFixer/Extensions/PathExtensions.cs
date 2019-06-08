using System.IO;

namespace NamespaceFixer.Extensions
{
    public static class PathExtensions
    {
        public static bool IsProjectFile(this string path)
        {
            return ProjectHelper.IsValidProjectExtension(Path.GetExtension(path));
        }

        public static bool IsProjectFile(this FileInfo fileInfo)
        {
            return ProjectHelper.IsValidProjectExtension(fileInfo.Extension);
        }
    }
}