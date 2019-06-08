using System;
using System.IO;
using System.IO.Extensions;
using System.Xml;

namespace NamespaceFixer.NamespaceBuilder
{
    internal abstract class NamespaceBuilderService : INamespaceBuilder
    {
        private readonly INamespaceAdjusterOptions _options;

        public NamespaceBuilderService(INamespaceAdjusterOptions options)
        {
            _options = options;
        }

        public string GetNamespace(string filePath, FileInfo solutionFile, FileInfo projectFile)
        {
            var solutionName = solutionFile.NameWithoutExtension();
            var projectName = projectFile.NameWithoutExtension();
            var projectRootNamespace = GetRootNamespaceFromProject(projectFile);
            var projectToSolutionPhisicalPath = GetProjectToSolutionPysicalPath(solutionFile, projectFile);
            var projectToSolutionVirtualPath = string.Empty; // GetProjectToSolutionVirtualPath(solutionFile, projectFile);
            var fileToProjectPath = GetFileToProjectPath(projectFile, filePath);

            return BuildNamespaceAccordingToOptions(
                solutionName,
                projectName,
                projectRootNamespace,
                projectToSolutionPhisicalPath,
                projectToSolutionVirtualPath,
                fileToProjectPath);
        }

        public abstract bool UpdateFile(ref string fileContent, string desiredNamespace);

        private string GetFileToProjectPath(FileInfo projectFile, string filePath)
        {
            return Directory.GetParent(filePath).FullName.Substring(projectFile.Directory.FullName.Length);
        }

        private string GetProjectToSolutionVirtualPath(FileInfo solutionFile, FileInfo projectFile)
        {
            throw new NotImplementedException();
        }

        private string GetProjectToSolutionPysicalPath(FileInfo solutionFile, FileInfo projectFile)
        {
            var projectAndSolutionFilesAreSameDirectory = projectFile.Directory.FullName.Equals(solutionFile.Directory.FullName);
            if (projectAndSolutionFilesAreSameDirectory)
                return string.Empty;

            return projectFile.Directory.FullName.Substring(solutionFile.Directory.FullName.Length + 1);
        }

        private string BuildNamespaceAccordingToOptions(
            string solutionName,
            string projectName,
            string projectRootNamespace,
            string projectToSolutionPhisicalPath,
            string projectToSolutionVirtualPath,
            string fileToProjectPath)
        {
            var newNamespace = _options.NamespaceFormat;

            Action<string, string> replaceWithFormat = (namespaceSection, sectionValue) =>
            {
                newNamespace = newNamespace.Replace(namespaceSection, "/" + sectionValue);
            };

            replaceWithFormat(NamespaceSections.SolutionName, solutionName);
            replaceWithFormat(NamespaceSections.ProjectName, projectName);
            replaceWithFormat(NamespaceSections.ProjectRootNamespace, projectRootNamespace);
            replaceWithFormat(NamespaceSections.ProjectToSolutionPhysicalPath, projectToSolutionPhisicalPath);
            replaceWithFormat(NamespaceSections.ProjectToSolutionVirtualPath, projectToSolutionVirtualPath);
            replaceWithFormat(NamespaceSections.FileToProjectPath, fileToProjectPath);

            return ToValidFormat(newNamespace);
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

        private string GetRootNamespaceFromProject(FileInfo projectFile)
        {
            var reader = BuildXmlProjectFileReader(projectFile);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "RootNamespace")
                {
                    reader.Read();

                    return reader.NodeType == XmlNodeType.Text ? reader.Value : null;
                }
            }

            return Path.GetFileName(projectFile.FullName);
        }

        private XmlReader BuildXmlProjectFileReader(FileInfo projectFile)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            return XmlReader.Create(projectFile.FullName, settings);
        }
    }
}