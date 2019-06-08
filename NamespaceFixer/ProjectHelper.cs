using System;
using System.IO;
using System.IO.Extensions;
using System.Linq;

namespace NamespaceFixer
{
    internal class ProjectHelper
    {

        public static bool IsValidProjectExtension(string pExtensionName)
        {
            string projectName = pExtensionName.StartsWith(".") ? pExtensionName.Substring(1) : pExtensionName;
            return projectName == Statics.CsProjectFileExtension || projectName == Statics.VbProjectFileExtension;
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
            var directory = new DirectoryInfo(projectFilePath);
            FileInfo solutionFile;

            while (!TryGetSolutionFile(directory, out solutionFile)) { directory = directory.Parent; }

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
