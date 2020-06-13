using Microsoft.VisualStudio.Shell;
using NamespaceFixer.NamespaceBuilder;
using System.ComponentModel;

namespace NamespaceFixer
{
    internal class OptionPage : DialogPage, INamespaceAdjusterOptions
    {
        [Category("Options")]
        [DisplayName("Namespace format")]
        [Description(@"
This is the path that will be used when adjusting the namespace of a file. Feel free to change it to match your exacts needs.
Default namespace format:" + NamespaceSections.ProjectName + NamespaceSections.FileToProjectPath + @".
The sections that can be used are: 
    • " + NamespaceSections.SolutionName + @": just the solution file name.
    • " + NamespaceSections.ProjectName + @": just the project file name.
    • " + NamespaceSections.ProjectRootNamespace + @": the 'Default namespace' specified in the properties of the project.
    • " + NamespaceSections.ProjectToSolutionPhysicalPath + @": the path from the project file directory to the solution file directory.
    • " + NamespaceSections.FileToProjectPath + @": the physical path from the file adjusting the path of to the project directory.
")]
        public string NamespaceFormat { get; set; } =
            NamespaceSections.ProjectRootNamespace +
            NamespaceSections.FileToProjectPath;

        [Category("Options")]
        [DisplayName("Extensions of files that will be ignored")]
        [Description(@"Extensions of files that will be ignored when adjusting namespaces. Please, use ';' to split if more than one.")]
        public string FileExtensionsToIgnore { get; set; } = string.Empty;
    }
}