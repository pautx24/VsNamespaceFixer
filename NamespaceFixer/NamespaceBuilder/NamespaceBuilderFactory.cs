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

            if (projectName == Statics.CsProjectFileExtension)
            {
                switch (fileExtension)
                {
                    case Statics.CsFileExtension:
                        return new CsNamespaceBuilderService(options);

                    case Statics.XamlFileExtension:
                        return new XamlNamespaceBuilderService(options);
                }
            }
            else if (projectName == Statics.VbProjectFileExtension)
            {
                return new VbNamespaceBuilderService(options);
            }

            return new DummyNamespaceBuilderService();
        }
    }
}