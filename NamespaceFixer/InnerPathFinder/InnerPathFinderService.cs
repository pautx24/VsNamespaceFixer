using NamespaceFixer.Extensions;
using System.Collections.Generic;
using System.IO;
using System.IO.Extensions;
using System.Linq;
using System.Text.RegularExpressions;

namespace NamespaceFixer.InnerPathFinder
{
    internal class InnerPathFinderService : IInnerPathFinder
    {
        public string[] GetAllInnerPaths(string[] selectedItemPaths)
        {
            var paths = new List<string>();

            foreach (var item in selectedItemPaths)
            {
                if (item.IsProjectFile())
                {
                    paths.AddRange(GetInnerPathsForProject(item));
                }
                else
                if (Directory.Exists(item))
                {
                    paths.AddRange(GetPathsForDirectory(item));
                }
                else
                {
                    paths.AddRange(GetItemWithRelatedPaths(item));
                }
            }

            return paths.ToArray();
        }

        private string GetHiddenFilesRegex(FileInfo file)
        {
            return file.NameWithoutExtension() + "\\.\\w+\\([.cs|.vb])";
        }

        private IEnumerable<string> GetItemWithRelatedPaths(string itemPath)
        {
            var paths = HiddenFilesFor(itemPath).ToList();
            paths.Add(itemPath);
            return paths;
        }

        private IEnumerable<string> HiddenFilesFor(string itemPath)
        {
            var file = new FileInfo(itemPath);
            var hiddenFilesRegex = GetHiddenFilesRegex(file);
            var regex = new Regex(hiddenFilesRegex);
            var extraFiles = Directory.GetParent(itemPath).GetFiles().Where(f => regex.IsMatch(f.Name));

            if (extraFiles.Any())
            {
                return extraFiles.Where(f => f.FullName != file.FullName).Select(f => f.FullName);
            }

            return new List<string>();
        }

        private IEnumerable<string> GetPathsForDirectory(string item)
        {
            var paths = Directory.EnumerateFiles(item).ToList();

            paths.AddRange(GetAllInnerPaths(Directory.EnumerateDirectories((string)item).ToArray()));

            return paths;
        }

        private string[] GetInnerPathsForProject(string item)
        {
            return GetAllInnerPaths(new string[] { Directory.GetParent(item).FullName });
        }
    }
}