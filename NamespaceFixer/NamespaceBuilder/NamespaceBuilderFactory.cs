using NamespaceFixer.Core;
using System;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class NamespaceBuilderFactory
    {
        internal static INamespaceBuilder CreateNamespaceBuilderService(string extension, INamespaceAdjusterOptions options)
        {
            INamespaceBuilder rslt = null;
            string projectName = ProjectHelper.GetProjectExtensionName(extension);

            switch (projectName)
            {
                case Statics.CsProjectFileExtension:
                    rslt = new CsNamespaceBuilderService(options);
                    break;

                case Statics.VbProjectFileExtension:
                    rslt = new VbNamespaceBuilderService(options);
                    break;
            }

            if (rslt is null)
            {
                throw new Exception($"Unsupported project file '{projectName}'.");
            }

            return rslt;
        }

    }
}
