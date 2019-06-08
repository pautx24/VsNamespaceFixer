using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using NamespaceFixer.InnerPathFinder;
using NamespaceFixer.NamespaceBuilder;
using NamespaceFixer.SolutionSelection;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace NamespaceFixer
{
    internal sealed class NamespaceAdjuster
    {
        private IInnerPathFinder _innerPathFinder;
        private INamespaceBuilder _namespaceBuilder;

        private readonly ISolutionSelectionService _solutionSelectionService;
        private readonly INamespaceAdjusterOptions _options;
        private readonly Package _package;

        private NamespaceAdjuster(
            Package package,
            ISolutionSelectionService solutionSelectionService,
            INamespaceAdjusterOptions options)
        {
            _package = package;
            _solutionSelectionService = solutionSelectionService;
            _options = options;
        }

        public static NamespaceAdjuster Instance { get; private set; }

        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(
            Package package,
            ISolutionSelectionService solutionSelectionService,
            INamespaceAdjusterOptions options)
        {
            Instance = new NamespaceAdjuster(
                package,
                solutionSelectionService,
                options);
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
            var selectedItemPaths = _solutionSelectionService.GetSelectedItemsPaths();

            var allPaths = _innerPathFinder.GetAllInnerPaths(selectedItemPaths);

            if (!allPaths.Any())
            {
                return;
            }

            var projectFile = ProjectHelper.GetProjectFilePath(allPaths[0]);
            var solutionFile = ProjectHelper.GetSolutionFilePath(projectFile.Directory.FullName);

            _innerPathFinder = ServiceFactory.CreateInnerPathFinderService(projectFile.Extension);
            _namespaceBuilder = ServiceFactory.CreateNamespaceBuilderService(projectFile.Extension, _options);

            allPaths.ToList().ForEach(f => FixNamespace(f, solutionFile, projectFile));
        }

        private void FixNamespace(string filePath, FileInfo solutionFile, FileInfo projectFile)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            var fileContent = File.ReadAllText(filePath);

            var desiredNamespace = _namespaceBuilder.GetNamespace(filePath, solutionFile, projectFile);

            var updated = _namespaceBuilder.UpdateFile(ref fileContent, desiredNamespace);

            if (updated)
            {
                File.WriteAllText(filePath, fileContent);
            }
        }
    }
}