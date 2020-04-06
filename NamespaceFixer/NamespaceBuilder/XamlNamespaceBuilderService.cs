using System;
using System.IO;
using System.IO.Extensions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class XamlNamespaceBuilderService : INamespaceBuilder
    {
        public XamlNamespaceBuilderService(INamespaceAdjusterOptions options)
        {
            _options = options;
        }

        protected Match FindNamespaceMatch(string fileContent)
        {
            return Regex.Match(fileContent, @"x:Class=""([a-zA-Z0-9_\.]+)\.[a-zA-Z0-9_]+""");
        }

        public string GetNamespace(string filePath, FileInfo solutionFile, FileInfo projectFile)
        {
            var solutionName = solutionFile.NameWithoutExtension();
            var projectName = projectFile.NameWithoutExtension();
            var projectRootNamespace = GetRootNamespaceFromProject(projectFile);
            var projectToSolutionPhysicalPath = GetProjectToSolutionPhysicalPath(solutionFile, projectFile);
            var projectToSolutionVirtualPath = string.Empty; // GetProjectToSolutionVirtualPath(solutionFile, projectFile);
            var fileToProjectPath = GetFileToProjectPath(projectFile, filePath);

            string result = BuildNamespaceAccordingToOptions(
                solutionName,
                projectName,
                projectRootNamespace,
                projectToSolutionPhysicalPath,
                projectToSolutionVirtualPath,
                fileToProjectPath);

            return ToValidFormat(result);
        }

        public bool UpdateFile(ref string fileContent, string desiredNamespace)
        {
            if (string.IsNullOrEmpty(desiredNamespace)) return false;

            var namespaceMatch = FindNamespaceMatch(fileContent);

            return namespaceMatch.Success ?
                UpdateNamespace(ref fileContent, desiredNamespace, namespaceMatch) :
                CreateNamespace(ref fileContent, desiredNamespace);
        }

        private bool UpdateNamespace(ref string fileContent, string desiredNamespace, Match namespaceMatch)
        {
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

        private bool CreateNamespace(ref string fileContent, string desiredNamespace)
        {
            return false;
        }

        private string GetRootNamespaceFromProject(FileInfo projectFile)
        {
            using (var reader = BuildXmlProjectFileReader(projectFile))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "RootNamespace")
                    {
                        reader.Read();

                        return reader.NodeType == XmlNodeType.Text ? reader.Value : null;
                    }
                }
            }
            return Path.GetFileNameWithoutExtension(projectFile.FullName);
        }

        private XmlReader BuildXmlProjectFileReader(FileInfo projectFile)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            return XmlReader.Create(projectFile.FullName, settings);
        }

        private string GetFileToProjectPath(FileInfo projectFile, string filePath)
        {
            return Directory.GetParent(filePath).FullName.Substring(projectFile.Directory.FullName.Length);
        }

        private string GetProjectToSolutionPhysicalPath(FileInfo solutionFile, FileInfo projectFile)
        {
            string solutionDirectoryFullName = solutionFile.Directory.FullName;
            string projectDirectoryFullName = projectFile.Directory.FullName;

            if (!projectDirectoryFullName.StartsWith(solutionDirectoryFullName))
                return string.Empty;

            var projectAndSolutionFilesAreSameDirectory = projectDirectoryFullName.Equals(solutionDirectoryFullName);
            if (projectAndSolutionFilesAreSameDirectory)
                return string.Empty;

            return projectDirectoryFullName.Substring(solutionDirectoryFullName.Length + 1);
        }

        private string ToValidFormat(string name)
        {
            return name
                .Replace(' ', '_')
                .Replace('-', '_')
                .Replace("\\", "/")
                .Replace('/', '.')
                .Replace("..", ".")
                .Trim('.');
        }

        protected string BuildNamespaceAccordingToOptions(
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

        private readonly INamespaceAdjusterOptions _options;

        internal INamespaceAdjusterOptions GetOptions()
        {
            return _options;
        }
    }
}