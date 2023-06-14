using System.Text.RegularExpressions;

namespace NamespaceFixer.Shared.NamespaceBuilder.CsNamespaceBuilders
{
    internal interface ICsNamespaceTypeBuilder
    {
        string NamespaceStartLimiter { get; }
        string NamespaceEndLimiter { get; }
        Match FindNamespaceMatch(string fileContent);
    }
}