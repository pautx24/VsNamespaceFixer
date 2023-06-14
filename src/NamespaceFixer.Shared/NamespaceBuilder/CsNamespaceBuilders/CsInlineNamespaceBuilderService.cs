using System.Text.RegularExpressions;

namespace NamespaceFixer.Shared.NamespaceBuilder.CsNamespaceBuilders
{
    internal class CsInlineNamespaceBuilderService : ICsNamespaceTypeBuilder
    {
        private static readonly Regex NamespaceRegex = new Regex(@"[\r\n|\r|\n]?namespace\s(.+);[\r\n|\r|\n]?");

        public string NamespaceStartLimiter => ";";

        public string NamespaceEndLimiter => string.Empty;

        public static bool IsInlineNamespace(string fileContent) => NamespaceRegex.IsMatch(fileContent);

        public Match FindNamespaceMatch(string fileContent) => NamespaceRegex.Match(fileContent);
    }
}