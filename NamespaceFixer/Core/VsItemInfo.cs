using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace NamespaceFixer.Core
{
    internal class VsItemInfo
    {

        private readonly IVsHierarchy _VsHierarchy;

        private string _Name = null;
        private Project _MsBuildProject = null;

        public VsItemInfo(IVsHierarchy pVsHierarchy)
        {
            _VsHierarchy = pVsHierarchy;
        }

        public IVsHierarchy GetVsHierarchy()
        {
            return _VsHierarchy;
        }

        public IVsProject GetVsProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return _VsHierarchy is IVsProject ? (IVsProject)_VsHierarchy : null;
        }

        public string GetProjectFullPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string fullPath = null;
            IVsProject project = GetVsProject();

            if (project != null)
                project.GetMkDocument(VSConstants.VSITEMID_ROOT, out fullPath);

            return fullPath;
        }

        public string GetSolutionFullPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return GetMsBuildProjectValue(MsBuildEvaluationHelper.SolutionPathProperty);
        }

        public string GetName()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_Name == null)
                _Name = GetName(_VsHierarchy, "no-name");

            return _Name;
        }

        private string GetMsBuildProjectValue(string key)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string rslt = null;
            Project msBuildProject = GetMsBuildProject();

            if (msBuildProject != null)
                rslt = msBuildProject.GetPropertyValue(key);

            return rslt;
        }

        private Project GetMsBuildProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_MsBuildProject == null)
            {
                string fullPath = GetProjectFullPath();

                if (fullPath != null)
                    _MsBuildProject = MsBuildEvaluationHelper.GetProject(fullPath);
            }

            return _MsBuildProject;
        }

        private static string GetName(IVsHierarchy vsHierarchy, string @default)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            object value = GetProperty(vsHierarchy, __VSHPROPID.VSHPROPID_Name, Convert.ToUInt32(VSConstants.VSITEMID.Root));
            return value != null ? value.ToString() : @default;
        }

        private static object GetProperty(IVsHierarchy vsHierarchy, __VSHPROPID key, uint vsItemId)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            object value = null;
            bool success = PackageHelper.Success(vsHierarchy.GetProperty(vsItemId, Convert.ToInt32(key), out value));
            return success ? value : null;
        }
    }
}
