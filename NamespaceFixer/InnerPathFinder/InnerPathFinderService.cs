using NamespaceFixer.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    paths.Add(item);
                }
            }

            return paths.ToArray();
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