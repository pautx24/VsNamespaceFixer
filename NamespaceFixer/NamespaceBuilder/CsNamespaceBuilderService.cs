using System;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class CsNamespaceBuilderService : NamespaceBuilderService
    {
        protected override string NamespaceStartLimiter => "{" + NewLine;
        protected override string NamespaceEndLimiter => "}";

        public CsNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected override Match FindNamespaceMatch(string fileContent)
        {
            return Regex.Match(fileContent, @"[\r\n|\r|\n]?namespace\s(.+)[\r\n|\r|\n]*{");
        }

        protected override MatchCollection FindUsingMatches(string fileContent)
        {
            return Regex.Matches(fileContent, @"\n?using\s(.+);");
        }

        protected override string BuildNamespaceLine(string desiredNamespace)
        {
            return "namespace " + desiredNamespace;
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

            replaceWithFormat(NamespaceSections.SolutionName, solutionName);
            replaceWithFormat(NamespaceSections.ProjectName, projectName);
            replaceWithFormat(NamespaceSections.ProjectRootNamespace, projectRootNamespace);
            replaceWithFormat(NamespaceSections.ProjectToSolutionPhysicalPath, projectToSolutionPhysicalPath);
            replaceWithFormat(NamespaceSections.ProjectToSolutionVirtualPath, projectToSolutionVirtualPath);
            replaceWithFormat(NamespaceSections.FileToProjectPath, fileToProjectPath);

            return newNamespace;
        }
    }
}