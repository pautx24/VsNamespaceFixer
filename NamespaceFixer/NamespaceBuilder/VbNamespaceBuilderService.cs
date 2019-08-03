using System;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class VbNamespaceBuilderService : NamespaceBuilderService
    {
        public VbNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected override Match GetNamespaceMatch(string fileContent)
        {
            return Regex.Match(fileContent, "Namespace\\s([^\n]+)");
        }

        protected override MatchCollection GetUsingMatches(string fileContent)
        {
            return Regex.Matches(fileContent, "import\\s([^\n]+)");
        }

        protected override string BuildNamespaceLine(string desiredNamespace)
        {
            return "Namespace " + desiredNamespace;
        }

        protected override string GetNamespaceStartLimiter()
        {
            return string.Empty;
        }

        protected override string GetNamespaceEndLimiter()
        {
            return "End Namespace";
        }

        internal override string BuildNamespaceAccordingToOptions(
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

            replaceWithFormat(NamespaceSections.SolutionName, String.Empty);
            replaceWithFormat(NamespaceSections.ProjectName, String.Empty);
            replaceWithFormat(NamespaceSections.ProjectRootNamespace, String.Empty);
            replaceWithFormat(NamespaceSections.ProjectToSolutionPhysicalPath, String.Empty);
            replaceWithFormat(NamespaceSections.ProjectToSolutionVirtualPath, String.Empty);
            replaceWithFormat(NamespaceSections.FileToProjectPath, fileToProjectPath);

            return newNamespace;
        }
    }
}