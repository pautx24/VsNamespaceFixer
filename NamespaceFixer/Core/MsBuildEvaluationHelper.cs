using Microsoft.Build.Evaluation;
using System.Linq;

namespace NamespaceFixer.Core
{
    public class MsBuildEvaluationHelper
    {

        /// <summary>
        /// Absolute path to the solution file.
        /// Example : J:\Repos\Tr4ncer\VsNamespaceFixer\NamespaceFixer.sln
        /// </summary>
        public const string cSolutionPathProperty = "SolutionPath";

        private static ProjectCollection _AllProjects = null;

        /// <summary>
        ///     ''' Nettoyage des variables mises en cache.
        ///     ''' </summary>
        public static void ClearCache()
        {
            if (_AllProjects != null)
            {
                _AllProjects.Dispose();
                _AllProjects = null;
            }
        }

        /// <summary>
        /// returns the known project.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static Project GetProject(string fullPath)
        {
            return GetAllProjects().GetLoadedProjects(fullPath).FirstOrDefault();
        }

        /// <summary>
        /// returns all known projects.
        /// </summary>
        /// <returns></returns>
        private static ProjectCollection GetAllProjects()
        {
            if (_AllProjects == null)
                _AllProjects = ProjectCollection.GlobalProjectCollection;

            return _AllProjects;
        }
    }

}
