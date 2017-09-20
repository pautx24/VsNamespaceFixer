namespace NamespaceFixer.NamespaceBuilder
{
    internal interface INamespaceBuilder
    {
        string GetIdealNamespace(string filePath, string basePath);
    }
}