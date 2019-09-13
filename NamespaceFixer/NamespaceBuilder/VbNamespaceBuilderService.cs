using System;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class VbNamespaceBuilderService : NamespaceBuilderService
    {
        protected override string NamespaceStartLimiter => string.Empty;
        protected override string NamespaceEndLimiter => "End Namespace";

        public VbNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected override Match FindNamespaceMatch(string fileContent)
        {
            return Regex.Match(fileContent, @"[\r\n|\r|\n]?Namespace\s(.+)[\r\n|\r|\n]");
        }

        protected override MatchCollection FindUsingMatches(string fileContent)
        {
            return Regex.Matches(fileContent, @"[\r\n|\r|\n]?Imports\s(.+)[\r\n|\r|\n]");
        }

        protected override string BuildNamespaceLine(string desiredNamespace)
        {
            return "Namespace " + desiredNamespace;
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