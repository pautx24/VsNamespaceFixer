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

        private static bool TryGetProjectFile(DirectoryInfo directory, out FileInfo directoryFile)
        {
            AssertIsNotRootDirectory(directory, "project");
            directoryFile = directory.EnumerateFiles().FirstOrDefault(f => IsValidProjectExtension(f.Extension));

            return directoryFile != null;
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
