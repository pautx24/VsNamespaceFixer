using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using NamespaceFixer.InnerPathFinder;
using NamespaceFixer.NamespaceBuilder;
using NamespaceFixer.SolutionSelection;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Extensions;
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

            var projectFile = GetProjectFilePath(allPaths[0]);
            var solutionFile = GetSolutionFilePath(projectFile.Directory.FullName);
            allPaths.ToList().ForEach(f => FixNamespace(f, solutionFile, projectFile));
        }

        private FileInfo GetProjectFilePath(string filePath)
        {
            var directory = Directory.GetParent(filePath);
            FileInfo file;

            while (!TryGetProjectFile(directory, out file)) { directory = directory.Parent; }

            return file;
        }

        private string BuildNamespaceLine(string desiredNamespace)
        {
            return "namespace " + desiredNamespace;
        }

        private void FixNamespace(string filePath, FileInfo solutionFile, FileInfo projectFile)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            var fileContent = File.ReadAllText(filePath);

            var desiredNamespace = _namespaceBuilder.GetNamespace(filePath, solutionFile, projectFile);

            var updated = UpdateFile(ref fileContent, desiredNamespace);

            if (updated)
            {
                File.WriteAllText(filePath, fileContent);
            }
        }

        private FileInfo GetSolutionFilePath(string projectFilePath)
        {
            var directory = new DirectoryInfo(projectFilePath);
            FileInfo solutionFile;

            while (!TryGetSolutionFile(directory, out solutionFile)) { directory = directory.Parent; }

            return solutionFile;
        }

        private bool TryGetProjectFile(DirectoryInfo directory, out FileInfo directoryFile)
        {
            AssertIsNotRootDirectory(directory, "project");
            directoryFile = directory.EnumerateFiles().SingleOrDefault(f => f.Extension == ".csproj");

            return directoryFile != null;
        }

        private bool TryGetSolutionFile(DirectoryInfo directory, out FileInfo solutionFile)
        {
            AssertIsNotRootDirectory(directory, "solution");

            solutionFile = directory.EnumerateFiles().SingleOrDefault(f => f.Extension == ".sln");

            return solutionFile != null;
        }

        private void AssertIsNotRootDirectory(DirectoryInfo directory, string fileLookingFor)
        {
            if (directory.IsRoot())
            {
                throw new Exception($"The root has been reached and the {fileLookingFor} has been not found");
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
                    var currentNamespace = match.Value.Trim().Split(' ').Last().Trim();

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