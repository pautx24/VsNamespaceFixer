using NamespaceFixer.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class NamespaceBuilderService : INamespaceBuilder
    {
        private readonly INamespaceAdjusterOptions _options;

        public NamespaceBuilderService(INamespaceAdjusterOptions options)
        {
            _options = options;
        }

        public string GetIdealNamespace(string filePath, string solutionPath)
        {
            var directory = Directory.GetParent(filePath);

            var idealNamespace = string.Empty;

            while (true)
            {
                if (IsProjectFolder(directory, solutionPath))
                {
                    return BuildFinalNamespace(directory, idealNamespace);
                }

                idealNamespace = AppendNamespace(idealNamespace, directory.Name);
                directory = directory.Parent;
            }
        }

        private string BuildFinalNamespace(DirectoryInfo directory, string idealNamespace)
        {
            string namespaceStart;
            if (_options.UseProjectDefaultNamespace)
            {
                namespaceStart = GetRootNamespaceFromProject(directory);
                if (namespaceStart == null)
                {
                    throw new Exception("Unable to find RootNamespace from project file");
                }
            }
            else
            {
                namespaceStart = directory.Name;
            }

            return TuneNamespace(AppendNamespace(idealNamespace, namespaceStart));
        }

        private string TuneNamespace(string name)
        {
            return name.Replace('-', '_');
        }

        private string AppendNamespace(string existingNamespace, string startNamespace)
        {
            return existingNamespace == string.Empty ? startNamespace : $"{startNamespace}.{existingNamespace}";
        }

        private string GetRootNamespaceFromProject(DirectoryInfo projectDirectory)
        {
            var projectFile = GetProjectFile(projectDirectory);

            var reader = BuildXmlProjectFileReader(projectFile);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "RootNamespace")
                {
                    reader.Read();

                    return reader.NodeType == XmlNodeType.Text ? reader.Value : null;
                }
            }

            return null;
        }

        private XmlReader BuildXmlProjectFileReader(FileInfo projectFile)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            return XmlReader.Create(projectFile.FullName, settings);
        }

        private FileInfo GetProjectFile(DirectoryInfo projectDirectory)
        {
            var file = projectDirectory.EnumerateFiles().SingleOrDefault(f => f.IsProjectFile());

            if (file == null)
            {
                throw new Exception($"Could not find project file on project directory: {projectDirectory}");
            }

            return file;
        }

        private bool IsProjectFolder(DirectoryInfo currentPath, string solutionPath)
        {
            return currentPath.Parent.FullName == solutionPath;
        }
    }
}