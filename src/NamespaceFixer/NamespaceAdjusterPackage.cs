﻿using EnvDTE;
using Microsoft.VisualStudio.Shell;
using NamespaceFixer.Core;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;

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
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Statics.PackageVersion, IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Guids.NamespaceFixerPackage)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionPage), "Namespace Fixer options", "Use default project namespace", 0, 0, true)]
    //[ProvideUIContextRule(Guids.UiContextSupportedFiles, TODO
    //    name: "Supported Files",
    //    expression: "CSharp | VisualBasic",
    //    termNames: new[] { "CSharp", "VisualBasic" },
    //    termValues: new[] { "HierSingleSelectionName:.cs$", "HierSingleSelectionName:.vb$" })]
    public sealed class NamespaceAdjusterPackage : AsyncPackage
    {
        private OptionPage _options;

        protected override System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            NamespaceAdjuster.Initialize(this);
            base.Initialize();

            return base.InitializeAsync(cancellationToken, progress);
        }

        /// <summary>
        /// Retrieves the file information for the current solution file.
        /// </summary>
        /// <returns>the file information for the current solution file</returns>
        internal FileInfo GetSolutionFile()
        {
            // verify thread access
            Dispatcher.CurrentDispatcher.VerifyAccess();

            // retrieve DTE as service
            if (!(GetService(typeof(DTE)) is DTE dte))
            {
                // failed to retrieve service
                throw new InvalidOperationException("Could not receive DTE service.");
            }

            // create file information
            return new FileInfo(dte.Solution.FullName);
        }

        internal OptionPage GetOptionPage()
        {
            if (_options == null)
            {
                _options = (OptionPage)GetDialogPage(typeof(OptionPage));
            }
            return _options;
        }
    }
}