using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using NamespaceFixer.InnerPathFinder;
using NamespaceFixer.NamespaceBuilder;
using NamespaceFixer.SolutionSelection;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NamespaceFixer
{
    internal sealed class NamespaceAdjuster
    {
        private readonly IInnerPathFinder _innerPathFinder;
        private readonly ISolutionSelectionService _solutionSelectionService;
        private readonly INamespaceBuilder _namespaceBuilder;
        private readonly INamespaceAdjusterOptions _options;
        private readonly Package _package;

        private NamespaceAdjuster(
            Package package,
            ISolutionSelectionService solutionSelectionService,
            IInnerPathFinder innerPathFinder,
            INamespaceBuilder namespaceBuilder,
            INamespaceAdjusterOptions options)
        {
            _package = package;
            _solutionSelectionService = solutionSelectionService;
            _innerPathFinder = innerPathFinder;
            _namespaceBuilder = namespaceBuilder;
            _options = options;
        }

        public static NamespaceAdjuster Instance { get; private set; }

        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(
            Package package,
            ISolutionSelectionService solutionSelectionService,
            IInnerPathFinder innerPathFinder,
            INamespaceBuilder namespaceBuilder,
            INamespaceAdjusterOptions options)
        {
            Instance = new NamespaceAdjuster(
                package,
                solutionSelectionService,
                innerPathFinder,
                namespaceBuilder,
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

            var basePath = GetSolutionPath(allPaths[0]);
            allPaths.ToList().ForEach(f => FixNamespace(f, basePath));
        }

        private string BuildNamespaceLine(string desiredNamespace)
        {
            return "namespace " + desiredNamespace;
        }

        private void FixNamespace(string filePath, string basePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            var fileContent = File.ReadAllText(filePath);
            var desiredNamespace = _namespaceBuilder.GetIdealNamespace(filePath, basePath);

            var updated = UpdateFile(ref fileContent, desiredNamespace);

            if (updated)
            {
                File.WriteAllText(filePath, fileContent);
            }
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
    }
}