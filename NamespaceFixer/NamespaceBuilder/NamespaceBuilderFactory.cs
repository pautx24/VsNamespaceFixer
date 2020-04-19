using NamespaceFixer.Core;
using System;
using System.IO;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class NamespaceBuilderFactory
    {
        internal static INamespaceBuilder CreateNamespaceBuilderService(string extension, INamespaceAdjusterOptions options, string filePath)
        {
            string projectName = ProjectHelper.GetProjectExtensionName(extension);
            string fileExtension = Path.GetExtension(filePath);

            if(projectName == Statics.CsProjectFileExtension)
            {
                switch (fileExtension)
                {
                    case ".cs":
                        return new CsNamespaceBuilderService(options);
                    case ".xaml":
                        return new XamlNamespaceBuilderService(options);
                    default:
                        throw new Exception($"Unsupported file extension '{fileExtension}'.");
                }
            }
            else if(projectName == Statics.VbProjectFileExtension)
            {
                return new VbNamespaceBuilderService(options);
            }
            else
            {
                throw new Exception($"Unsupported project file '{projectName}'.");
            }
        }

    }
}
