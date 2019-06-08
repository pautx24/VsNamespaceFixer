using NamespaceFixer.InnerPathFinder;
using NamespaceFixer.NamespaceBuilder;
using System;

namespace NamespaceFixer
{
    internal class ServiceFactory
    {
        internal static IInnerPathFinder CreateInnerPathFinderService(string extension)
        {
            IInnerPathFinder rslt = null;

            switch (extension)
            {
                case "cs":
                    rslt = new CsInnerPathFinderService();
                    break;

                case "vb":
                    rslt = new VbInnerPathFinderService();
                    break;
            }

            if (rslt is null)
            {
                throw new Exception($"Unsupported file '{extension}'.");
            }

            return rslt;
        }

        internal static INamespaceBuilder CreateNamespaceBuilderService(string extension, INamespaceAdjusterOptions options)
        {
            INamespaceBuilder rslt = null;

            switch (extension)
            {
                case "cs":
                    rslt = new CsNamespaceBuilderService(options);
                    break;

                case "vb":
                    rslt = new VbNamespaceBuilderService(options);
                    break;
            }

            if (rslt is null)
            {
                throw new Exception($"Unsupported file '{extension}'.");
            }

            return rslt;
        }
    }
}
