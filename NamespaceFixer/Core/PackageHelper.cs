using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace NamespaceFixer.Core
{
    internal class PackageHelper
    {

        public static bool Success(int requestRslt)
        {
            return (requestRslt == VSConstants.S_OK);
        }

        public static T GetService<T>(Type type)
        {
            IServiceProvider provider = null;

            if (NamespaceAdjuster.Instance != null)
            {
                provider = NamespaceAdjuster.Instance.ServiceProvider;
            }

            return GetService<T>(provider, type);
        }

        public static T GetService<T>(IServiceProvider provider, Type type)
        {
            T rslt = default(T);

            if (provider != null && type != null)
            {
                object service = provider.GetService(type);

                if (service != null && service is T)
                    rslt = (T)service;
            }

            return rslt;
        }

        public static IVsSolutionBuildManager GetVsSolutionBuildManager()
        {
            IVsSolutionBuildManager solutionBuildManager = PackageHelper.GetService<IVsSolutionBuildManager>(typeof(SVsSolutionBuildManager));
            return solutionBuildManager;
        }

        public static VsItemInfo GetStartupProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            VsItemInfo startupProject = null;
            IVsSolutionBuildManager solutionBuildManager = GetVsSolutionBuildManager();

            if (solutionBuildManager != null)
            {
                IVsHierarchy value = null;
                bool success = Success(solutionBuildManager.get_StartupProject(out value));

                if (success && value != null)
                    startupProject = new VsItemInfo(value);
            }

            return startupProject;
        }
    }
}
