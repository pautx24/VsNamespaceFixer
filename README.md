# VsNamespaceFixer
An very simple and light extension, compatible with VS 2015 & VS 2017 & VS 2019.

It only works for C#-like and VB.NET namespace format.

Adds an 'Adjust namespace' option on your Solution Explorer contextual menu. It will only appear for files (one or more) and folders (one or more) for the same project. Just right-click them and you will see how it works.

A new way of customizing the namespace has been added! There is a single option for this extension (can be found at Tools > Options > Namespace fixer options). There is the format you can give to a namespace. The specified sections will be replaced for the file/project specifications, and any other text will remain static. The sections that can be used are:

{solutionName}: just the solution file name.
{projectName}: just the project file name.
{projectRootNamespace}: the 'Default namespace' specified in the properties of the project.
{projectToSolutionPhisicalPath}: the path from the project file directory to the solution file directory.
{fileToProjectPath}: the physical path from the file adjusting the path of to the project directory.
The default namespace format is specified as: {projectName}{fileToProjectPath}.

The source code can be found on GitHub.

Hope it fits for you.
