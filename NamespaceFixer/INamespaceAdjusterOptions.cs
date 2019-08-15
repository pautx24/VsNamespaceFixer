namespace NamespaceFixer
{
    internal interface INamespaceAdjusterOptions
    {
        string NamespaceFormat { get; }

        string FileExtensionsToIgnore { get; }
    }
}