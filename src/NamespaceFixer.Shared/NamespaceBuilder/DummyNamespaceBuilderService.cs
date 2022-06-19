using System.IO;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class DummyNamespaceBuilderService : INamespaceBuilder
    {
        public string GetNamespace(string filePath, FileInfo solutionFile, FileInfo projectFile) => string.Empty;

        public bool UpdateFile(ref string fileContent, string desiredNamespace) => false;
    }
}