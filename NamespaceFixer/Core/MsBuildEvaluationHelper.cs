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
        public const string SolutionPathProperty = "SolutionPath";

        private static ProjectCollection _allProjects = null;

        /// <summary>
        /// Cleaning cached variables.
        /// </summary>
        public static void ClearCache()
        {
            if (_allProjects != null)
            {
                _allProjects.Dispose();
                _allProjects = null;
            }
        }

        /// <summary>
        /// Returns the known project.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static Project GetProject(string fullPath)
        {
            return GetAllProjects().GetLoadedProjects(fullPath).FirstOrDefault();
        }

        /// <summary>
        /// Returns all known projects.
        /// </summary>
        /// <returns></returns>
        private static ProjectCollection GetAllProjects()
        {
            if (_allProjects == null)
                _allProjects = ProjectCollection.GlobalProjectCollection;

            return _allProjects;
        }
    }

}
