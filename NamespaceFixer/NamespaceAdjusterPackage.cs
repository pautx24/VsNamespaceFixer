using Microsoft.VisualStudio.Shell;
using NamespaceFixer.InnerPathFinder;
using NamespaceFixer.NamespaceBuilder;
using NamespaceFixer.SolutionSelection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NamespaceFixer
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Guids.NamespaceFixerPackage)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionPage),
        "Namespace Fixer options", "Use default project namespace", 0, 0, true)]
    public sealed class NamespaceAdjusterPackage : Package
    {
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            var options = (OptionPage)GetDialogPage(typeof(OptionPage));
            var solutionSelection = new SolutionSelectionService();
            var innerPathFinder = new InnerPathFinderService();
            var namespaceBuilder = new NamespaceBuilderService(options);
            NamespaceAdjuster.Initialize(this, solutionSelection, innerPathFinder, namespaceBuilder, options);
            base.Initialize();
        }
    }
}