using System.Linq;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class CsNamespaceBuilderService : NamespaceBuilderService
    {
        public CsNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        public override bool UpdateFile(ref string fileContent, string desiredNamespace)
        {
            var namespaceMatch = Regex.Match(fileContent, "namespace\\s([^\n{]+)");

            if (!namespaceMatch.Success)
            {
                return false;
            }

            var fileRequiresUpdate = false;
            foreach (var group in namespaceMatch.Groups)
            {
                if (group is Match match)
                {
                    var currentNamespace = match.Value.Trim().Split(' ').Last().Trim();

                    if (currentNamespace != desiredNamespace)
                    {
                        fileRequiresUpdate = true;
                        fileContent = fileContent.Replace(BuildNamespaceLine(currentNamespace), BuildNamespaceLine(desiredNamespace));
                    }
                }
            }

            return fileRequiresUpdate;
        }

        private string BuildNamespaceLine(string desiredNamespace)
        {
            return "namespace " + desiredNamespace;
        }
    }
}