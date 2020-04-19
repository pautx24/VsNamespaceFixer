using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class XamlNamespaceBuilderService : NamespaceBuilderService
    {
        public XamlNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        public override bool UpdateFile(ref string fileContent, string desiredNamespace)
        {
            if (string.IsNullOrEmpty(desiredNamespace)) return false;

            return
                UpdateClassName(ref fileContent, desiredNamespace) ||
                UpdateNamespace(ref fileContent, desiredNamespace);
        }

        protected override Match FindNamespaceMatch(string fileContent) =>
            Regex.Match(fileContent, @"xmlns:local=""clr-namespace:([a-zA-Z0-9_\.]+)""");

        protected override string BuildNamespaceAccordingToOptions(
          string solutionName,
          string projectName,
          string projectRootNamespace,
          string projectToSolutionPhysicalPath,
          string projectToSolutionVirtualPath,
          string fileToProjectPath)
        {
            var newNamespace = GetOptions().NamespaceFormat;

            Action<string, string> replaceWithFormat = (namespaceSection, sectionValue) =>
            {
                newNamespace = newNamespace.Replace(namespaceSection, "/" + sectionValue);
            };

            replaceWithFormat(NamespaceSections.SolutionName, solutionName);
            replaceWithFormat(NamespaceSections.ProjectName, projectName);
            replaceWithFormat(NamespaceSections.ProjectRootNamespace, projectRootNamespace);
            replaceWithFormat(NamespaceSections.ProjectToSolutionPhysicalPath, projectToSolutionPhysicalPath);
            replaceWithFormat(NamespaceSections.ProjectToSolutionVirtualPath, projectToSolutionVirtualPath);
            replaceWithFormat(NamespaceSections.FileToProjectPath, fileToProjectPath);

            return newNamespace;
        }

        private bool UpdateNamespace(ref string fileContent, string desiredNamespace)
        {
            var namespaceMatch = FindNamespaceMatch(fileContent);

            if (!namespaceMatch.Success)
                return false;

            var fileRequiresUpdate = false;

            var namespaceGroup = namespaceMatch.Groups.OfType<Group>().Where(g => !(g is Match)).FirstOrDefault();

            if (namespaceGroup == null) return false;

            var currentNamespace = namespaceGroup.Value.Trim();

            if (currentNamespace != desiredNamespace)
            {
                fileRequiresUpdate = true;
                fileContent = fileContent.Substring(0, namespaceGroup.Index) + desiredNamespace + fileContent.Substring(namespaceGroup.Index + namespaceGroup.Value.Trim().Length);
            }

            return fileRequiresUpdate;
        }

        private Match FindClassNameMatch(string fileContent) =>
            Regex.Match(fileContent, @"x:Class=""([a-zA-Z0-9_\.]+)\.[a-zA-Z0-9_]+""");

        private bool UpdateClassName(ref string fileContent, string desiredNamespace)
        {
            var namespaceMatch = FindClassNameMatch(fileContent);

            if (!namespaceMatch.Success)
                return false;

            var fileRequiresUpdate = false;

            var namespaceGroup = namespaceMatch.Groups.OfType<Group>().Where(g => !(g is Match)).FirstOrDefault();

            if (namespaceGroup == null) return false;

            var currentNamespace = namespaceGroup.Value.Trim();

            if (currentNamespace != desiredNamespace)
            {
                fileRequiresUpdate = true;
                fileContent = fileContent.Substring(0, namespaceGroup.Index) + desiredNamespace + fileContent.Substring(namespaceGroup.Index + namespaceGroup.Value.Trim().Length);
            }

            return fileRequiresUpdate;
        }
    }
}