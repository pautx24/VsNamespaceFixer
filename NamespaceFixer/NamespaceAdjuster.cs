using Microsoft.VisualStudio.Shell;
using NamespaceFixer.Core;
using NamespaceFixer.NamespaceBuilder;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace NamespaceFixer
{
    internal sealed class NamespaceAdjuster
    {
        private readonly NamespaceAdjusterPackage _package;
        private readonly VsServiceInfo _serviceInfo;

        private NamespaceAdjuster(NamespaceAdjusterPackage package)
        {
            _package = package;
            _serviceInfo = new VsServiceInfo(this);
        }

        public static NamespaceAdjuster Instance { get; private set; }

        internal IServiceProvider ServiceProvider => (Package)_package;

        public static void Initialize(NamespaceAdjusterPackage package)
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
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                //menuItem.BeforeQueryStatus += ButtonQueryStatus; TODO

                commandService.AddCommand(menuItem);
            }
        }

        // TODO
        //private void ButtonQueryStatus(object sender, EventArgs e)
        //{
        //    var button = (MenuCommand)sender;
        //    button.Visible = false;

        //    _serviceInfo.GetSelectedProjects();
        //}

        /// <summary>
        /// Click on the button 'Adjust namespaces'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var selectedItemPaths = _serviceInfo.SolutionSelectionService.GetSelectedItemsPaths();

                var allPaths = _serviceInfo.InnerPathFinder.GetAllInnerPaths(selectedItemPaths);

                if (!allPaths.Any())
                {
                    return;
                }

                var solutionFile = _package.GetSolutionFile();
                var projectFile = ProjectHelper.GetProjectFilePath(allPaths[0]);

                foreach (var filePath in allPaths.ToList())
                {
                    var builder = NamespaceBuilderFactory.CreateNamespaceBuilderService(projectFile.Extension, _package.GetOptionPage(), filePath);
                    FixNamespace(builder, filePath, solutionFile, projectFile);
                }
            }
            finally
            {
                MsBuildEvaluationHelper.ClearCache();
            }
        }

        private void FixNamespace(INamespaceBuilder namespaceBuilder, string filePath, FileInfo solutionFile, FileInfo projectFile)
        {
            if (!File.Exists(filePath) || IgnoreFile(filePath))
            {
                return;
            }

            var encoding = NamespaceFixer.Extensions.PathExtensions.GetEncoding(filePath);

            var fileContent = File.ReadAllText(filePath, encoding);

            var desiredNamespace = namespaceBuilder.GetNamespace(filePath, solutionFile, projectFile);

            var updated = namespaceBuilder.UpdateFile(ref fileContent, desiredNamespace);

            if (updated)
            {
                File.WriteAllText(filePath, fileContent, encoding);
            }
        }

        private bool IgnoreFile(string path)
        {
            var extensionWithoutDot = Path.GetExtension(path).Substring(1);

            return ExtensionsToIgnore.Contains(extensionWithoutDot);
        }

        private string[] ExtensionsToIgnore => _package.GetOptionPage()
                    .FileExtensionsToIgnore
                    .Split(';')
                    .Select(ignoredExtension => ignoredExtension.Replace(".", string.Empty).Trim())
                    .Where(ext => !string.IsNullOrEmpty(ext))
                    .ToArray();
    }
}