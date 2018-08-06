namespace System.IO.Extensions
{
    public static class DirectoryExtensions
    {
        public static bool IsSamePath(this DirectoryInfo current, DirectoryInfo other)
        {
            return current.FullName == other.FullName;
        }

        public static bool IsRoot(this DirectoryInfo current)
        {
            return current.FullName == Directory.GetDirectoryRoot(current.FullName);
        }
    }
}