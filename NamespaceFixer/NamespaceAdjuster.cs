using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace NamespaceFixer
{
    /// <summary>
    /// Command handler
    /// </summary>
    //[PackageRegistration(UseManagedResourcesOnly = true)]
    //[ProvideMenuResource("Menus.ctmenu", 1)]
    //[Guid(Guids.NamespaceFixerPackage)]
    //[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    //[ComVisible(true)]
    internal sealed class NamespaceAdjuster // : Package
    {
        private readonly Package _package;

        private NamespaceAdjuster(Package package)
        {
            _package = package;
        }

        public static NamespaceAdjuster Instance { get; private set; }

        private IServiceProvider ServiceProvider => _package;
        
        public static void Initialize(Package package)
        {
            Instance = new NamespaceAdjuster(package);
            Instance.Initialize();
        }
        
        public void Initialize()
        {
            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(Guids.NamespaceFixerCmdSet, Ids.CmdIdAdjustNamespace);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var selectedItemPaths = GetSelectedItemsPaths();

            var allPaths = GetAllPaths(selectedItemPaths);

            if (!allPaths.Any())
            {
                return;
            }
            
            var basePath = GetSolutionPath(allPaths[0]);

            allPaths.ToList().ForEach(f => FixNamespace(f, basePath));
        }

        private void FixNamespace(string filePath, string basePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            var fileContent = File.ReadAllText(filePath);
            var desiredNamespace = GetIdealNamespace(filePath, basePath);

            var updated = UpdateFile(ref fileContent, desiredNamespace);

            if (updated)
            {
                File.WriteAllText(filePath, fileContent);
            }
        }

        private bool UpdateFile(ref string fileContent, string desiredNamespace)
        {
            var namespaceMatch = Regex.Match(fileContent, "namespace\\s([^\n{]+)");

            if (!namespaceMatch.Success)
            {
                return false;
            }

            var fileRequiresUpdate = false;
            foreach (var group in namespaceMatch.Groups)
            {
                if (group is Match match)
                {
                    var currentNamespace = fileContent.Substring(match.Index, match.Length).Split(' ').Last().Trim();

                    if (currentNamespace != desiredNamespace)
                    {
                        fileRequiresUpdate = true;
                        fileContent = fileContent.Replace(BuildNamespaceLine(currentNamespace), BuildNamespaceLine(desiredNamespace));
                    }
                }
            }

            return fileRequiresUpdate;
        }

        private string BuildNamespaceLine(string desiredNamespace)
        {
            return "namespace " + desiredNamespace;
        }

        private string GetIdealNamespace(string filePath, string basePath)
        {
            var directory = Directory.GetParent(filePath);

            var idealNamespace = directory.Name;

            directory = directory.Parent;

            while (directory.FullName.Length > basePath.Length)
            {
                idealNamespace = $"{directory.Name}.{idealNamespace}";
                
                directory = directory.Parent;
            }

            return idealNamespace;
        }

        private string GetSolutionPath(string path)
        {
            var directory = Directory.GetParent(path);

            while (!IsSolutionContainerDirectory(directory)) { directory = directory.Parent; }

            return directory.FullName;
        }

        private bool IsSolutionContainerDirectory(DirectoryInfo directory)
        {
            return directory.EnumerateFiles().Any(f => f.Extension == ".sln");
        }

        private string[] GetAllPaths(string[] selectedItemPaths)
        {
            var paths = new List<string>();

            for (var i = selectedItemPaths.Length - 1; i >= 0; i--)
            {
                var currentPath = selectedItemPaths[i];

                if (Directory.Exists(currentPath))
                {
                    paths.AddRange(Directory.EnumerateFiles(currentPath));
                    paths.AddRange(GetAllPaths(Directory.EnumerateDirectories(currentPath).ToArray()));
                }
                else
                {
                    paths.Add(currentPath);
                }
            }

            return paths.ToArray();
        }

        private EnvDTE80.DTE2 GetDTE2()
        {
            return Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
        }

        private string[] GetSelectedItemsPaths()
        {
            var _applicationObject = GetDTE2();
            var uih = _applicationObject.ToolWindows.SolutionExplorer;
            var selectedItems = (Array)uih.SelectedItems;
            if (selectedItems == null)
            {
                return new string[] { };
            }

            var paths = new List<string>();
            foreach (UIHierarchyItem selItem in selectedItems)
            {
                var prjItem = selItem.Object as ProjectItem;
                var filePath = prjItem.Properties.Item("FullPath").Value.ToString();
                paths.Add(filePath);
            }

            return paths.ToArray();
        }
    }
}