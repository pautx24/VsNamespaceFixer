using System.IO;
using System.Text;

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

        public static Encoding GetEncoding(this string path)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return new System.Text.UTF8Encoding(true); // UTF-8 BOM
            if (bom[0] == 0x75 && bom[1] == 0x73 && bom[2] == 0x69 && bom[3] == 0x6e) return new UTF8Encoding(false);
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            return Encoding.ASCII;
        }
    }
}