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

        public abstract bool UpdateFile(ref string fileContent, string desiredNamespace);

        internal INamespaceAdjusterOptions GetOptions()
        {
            return _options;
        }

        internal abstract string BuildNamespaceAccordingToOptions(
         string solutionName,
         string projectName,
         string projectRootNamespace,
         string projectToSolutionPhysicalPath,
         string projectToSolutionVirtualPath,
         string fileToProjectPath);

        private string GetFileToProjectPath(FileInfo projectFile, string filePath)
        {
            return Directory.GetParent(filePath).FullName.Substring(projectFile.Directory.FullName.Length);
        }

        private string GetProjectToSolutionVirtualPath(FileInfo solutionFile, FileInfo projectFile)
        {
            throw new NotImplementedException();
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
    }
}