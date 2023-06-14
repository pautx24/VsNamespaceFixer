using System.Text.RegularExpressions;

namespace NamespaceFixer.Shared.NamespaceBuilder.CsNamespaceBuilders
{
    internal class CsCurlyBracketNamespaceBuilderService : ICsNamespaceTypeBuilder
    {
        private readonly string _newLine;

        public CsCurlyBracketNamespaceBuilderService(string newLine)
        {
            _newLine = newLine;
        }

        public string NamespaceStartLimiter => "{" + _newLine;
        public string NamespaceEndLimiter => "}";

        public Match FindNamespaceMatch(string fileContent) => Regex.Match(fileContent, @"[\r\n|\r|\n]?namespace\s(.+)[\r\n|\r|\n]*{");
    }
}