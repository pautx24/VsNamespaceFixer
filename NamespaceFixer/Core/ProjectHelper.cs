using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.IO.Extensions;
using System.Linq;

namespace NamespaceFixer.Core
{
    internal class ProjectHelper
    {

        public static bool IsValidProjectExtension(string extensionName)
        {
            string projectName = GetProjectExtensionName(extensionName);
            return projectName == Statics.CsProjectFileExtension || projectName == Statics.VbProjectFileExtension;
        }

        public static string GetProjectExtensionName(string extensionName)
        {
            return extensionName.StartsWith(".") ? extensionName.Substring(1) : extensionName;
        }

        public static FileInfo GetProjectFilePath(string filePath)
        {
            var directory = Directory.GetParent(filePath);
            FileInfo file;

            while (!TryGetProjectFile(directory, out file)) { directory = directory.Parent; }

            return file;
        }

        public static FileInfo GetSolutionFilePath(string projectFilePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FileInfo solutionFile = null;
            VsItemInfo startupProject = PackageHelper.GetStartupProject();

            if (startupProject != null)
            {
                solutionFile = new FileInfo(startupProject.GetSolutionFullPath());
            }

            if (solutionFile == null || !solutionFile.Exists)
            {
                var directory = new DirectoryInfo(projectFilePath);

                while (!TryGetSolutionFile(directory, out solutionFile)) { directory = directory.Parent; }
            }

            return solutionFile;
        }

        private static bool TryGetProjectFile(DirectoryInfo directory, out FileInfo directoryFile)
        {
            AssertIsNotRootDirectory(directory, "project");
            directoryFile = directory.EnumerateFiles().FirstOrDefault(f => IsValidProjectExtension(f.Extension));

            return directoryFile != null;
        }

        private static bool TryGetSolutionFile(DirectoryInfo directory, out FileInfo solutionFile)
        {
            AssertIsNotRootDirectory(directory, "solution");

            solutionFile = directory.EnumerateFiles().FirstOrDefault(f => f.Extension == ".sln");

            return solutionFile != null;
        }

        private static void AssertIsNotRootDirectory(DirectoryInfo directory, string fileLookingFor)
        {
            if (directory.IsRoot())
            {
                throw new Exception($"The root has been reached and the {fileLookingFor} has been not found");
            }
        }
    }
}
