using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NamespaceFixer.InnerPathFinder;
using NamespaceFixer.SolutionSelection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NamespaceFixer.Core
{
    /// <summary>
    /// Information provider on Visual Studio.
    /// Based on the visual studio shell.
    /// </summary>
    internal class VsServiceInfo
    {

        private IVsSolutionBuildManager _vsSolutionBuildManager = null;
        private IVsMonitorSelection _vsMonitorSelection = null;

        private VsItemInfo _startupProject = null;
        private FileInfo _solutionFile = null;

        internal NamespaceAdjuster NamespaceAdjuster { get; } = null;
        internal ISolutionSelectionService SolutionSelectionService { get; } = null;
        internal IInnerPathFinder InnerPathFinder { get; } = null;

        public VsServiceInfo(NamespaceAdjuster namespaceAdjuster)
        {
            NamespaceAdjuster = namespaceAdjuster;
            SolutionSelectionService = new SolutionSelectionService();
            InnerPathFinder = new InnerPathFinderService();
        }

        /// <summary>
        /// Returns the build manager of the solution.
        /// </summary>
        /// <returns></returns>
        public IVsSolutionBuildManager GetVsSolutionBuildManager()
        {
            if (_vsSolutionBuildManager == null)
                _vsSolutionBuildManager = PackageHelper.GetService<IVsSolutionBuildManager>(typeof(SVsSolutionBuildManager));
            return _vsSolutionBuildManager;
        }

        /// <summary>
        /// Returns the selection monitor.
        /// </summary>
        /// <returns></returns>
        public IVsMonitorSelection GetVsMonitorSelection()
        {
            if (_vsMonitorSelection == null)
                _vsMonitorSelection = PackageHelper.GetService<IVsMonitorSelection>(typeof(SVsShellMonitorSelection));
            return _vsMonitorSelection;
        }

        /// <summary>
        /// Returns the startup project (based on the build manager).
        /// </summary>
        /// <returns></returns>
        public VsItemInfo GetStartupProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_startupProject == null)
            {
                IVsSolutionBuildManager solutionBuildManager = GetVsSolutionBuildManager();

                if (solutionBuildManager != null)
                {
                    bool success = PackageHelper.Success(solutionBuildManager.get_StartupProject(out IVsHierarchy value));

                    if (success && value != null)
                        _startupProject = new VsItemInfo(value);
                }
            }
            return _startupProject;
        }

        /// <summary>
        /// Returns the solution file (based on the startup project).
        /// </summary>
        /// <returns></returns>
        public FileInfo GetSolutionFileInfo()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_solutionFile == null)
            {
                VsItemInfo startupProject = GetStartupProject();

                if (startupProject != null)
                {
                    string solutionFullPath = startupProject.GetSolutionFullPath();

                    if (solutionFullPath != null)
                        _solutionFile = new FileInfo(solutionFullPath);
                }
            }
            return _solutionFile;
        }

        public IList<IVsHierarchy> GetSelectedProjects()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<IVsHierarchy> rslt = null;
            IVsMonitorSelection vsMonitorSelection = GetVsMonitorSelection();

            if (vsMonitorSelection != null)
            {
                bool success = PackageHelper.Success(vsMonitorSelection.GetCurrentSelection(out IntPtr hierarchyPtr, out uint itemId, out IVsMultiItemSelect multiSelect, out IntPtr containerPtr));

                if (IntPtr.Zero != containerPtr)
                    Marshal.Release(containerPtr);

                if (success)
                {
                    rslt = new List<IVsHierarchy>();

                    if (itemId == (uint)VSConstants.VSITEMID.Selection && multiSelect != null)
                    {
                        success = PackageHelper.Success(multiSelect.GetSelectionInfo(out uint itemCount, out int fSingleHierarchy));

                        if (success)
                        {
                            VSITEMSELECTION[] items = new VSITEMSELECTION[itemCount];

                            success = PackageHelper.Success(multiSelect.GetSelectedItems(0, itemCount, items));

                            if (success)
                            {
                                foreach (VSITEMSELECTION item in items)
                                {
                                    if (item.pHier == null || rslt.Contains(item.pHier))
                                        continue;

                                    rslt.Add(item.pHier);
                                }
                            }
                        }
                    }
                    else if (hierarchyPtr != IntPtr.Zero)
                    {
                        object uniqueObjectForIUnknown = Marshal.GetUniqueObjectForIUnknown(hierarchyPtr);

                        if (uniqueObjectForIUnknown != null && uniqueObjectForIUnknown is IVsHierarchy)
                        {
                            IVsHierarchy hierarchy = (IVsHierarchy)uniqueObjectForIUnknown;
                            rslt.Add(hierarchy);
                        }
                    }
                }
            }
            return rslt;
        }

    }
}
