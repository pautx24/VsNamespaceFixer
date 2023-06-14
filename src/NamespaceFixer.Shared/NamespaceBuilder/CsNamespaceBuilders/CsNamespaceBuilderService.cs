using NamespaceFixer.Shared.NamespaceBuilder.CsNamespaceBuilders;
using System;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class CsNamespaceBuilderService : LogicNamespaceBuilderService
    {
        private ICsNamespaceTypeBuilder SpecificNamespaceBuilder;

        protected override string NamespaceStartLimiter => SpecificNamespaceBuilder.NamespaceStartLimiter;
        protected override string NamespaceEndLimiter => SpecificNamespaceBuilder.NamespaceEndLimiter;

        public CsNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected override Match FindNamespaceMatch(string fileContent)
        {
            if (CsInlineNamespaceBuilderService.IsInlineNamespace(fileContent)) SpecificNamespaceBuilder = new CsInlineNamespaceBuilderService();
            else SpecificNamespaceBuilder = new CsCurlyBracketNamespaceBuilderService(NewLine);

            return SpecificNamespaceBuilder.FindNamespaceMatch(fileContent);
        }

        protected override MatchCollection FindUsingMatches(string fileContent) =>
            Regex.Matches(fileContent, @"\n?using\s(.+);");

        protected override string BuildNamespaceLine(string desiredNamespace) => "namespace " + desiredNamespace;

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
    }
}