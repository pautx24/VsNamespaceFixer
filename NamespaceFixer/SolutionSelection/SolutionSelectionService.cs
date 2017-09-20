using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;

namespace NamespaceFixer.SolutionSelection
{
    internal class SolutionSelectionService : ISolutionSelectionService
    {
        public string[] GetSelectedItemsPaths()
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems == null)
            {
                return new string[] { };
            }

            var paths = new List<string>();
            foreach (UIHierarchyItem selItem in selectedItems)
            {
                var prjItem = (ProjectItem)selItem.Object;
                var filePath = prjItem.Properties.Item("FullPath").Value.ToString();
                paths.Add(filePath);
            }

            return paths.ToArray();
        }

        private Array GetSelectedItems()
        {
            var _applicationObject = GetDTE2();
            var uih = _applicationObject.ToolWindows.SolutionExplorer;
            return (Array)uih.SelectedItems;
        }

        private EnvDTE80.DTE2 GetDTE2()
        {
            return Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
        }
    }
}