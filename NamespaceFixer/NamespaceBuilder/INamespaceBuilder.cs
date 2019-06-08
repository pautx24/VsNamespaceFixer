using System.IO;

namespace NamespaceFixer.NamespaceBuilder
{
    internal interface INamespaceBuilder
    {
        string GetNamespace(string filePath, FileInfo solutionFile, FileInfo projectFile);

        bool UpdateFile(ref string fileContent, string desiredNamespace);

    }
}