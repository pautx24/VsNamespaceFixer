# Visual Studio Namespace Fixer
An very simple and light extension, compatible with VS 2015, VS 2017 & VS 2019.

It only works for C#-like and VB.NET namespace format.

Adds an 'Adjust namespace' option on your Solution Explorer contextual menu. It will only appear for files (one or more) and folders (one or more) for the same project. Just right-click them and you will see how it works.

There are a couple of options you can choose from (can be found at Tools > Options > Namespace fixer options):
1. The extensions of the files that will be ignored when adjusting the namespace.
1. Customize how the namespace will be formatted. The specified sections that matches the expected ones will be replaced and any other text will remain. The sections that can be used are:

* `{solutionName}`: just the solution file name.
* `{projectName}`: just the project file name.
* `{projectRootNamespace}`: the 'Default namespace' specified in the properties of the project.
* `{projectToSolutionPhysicalPath}`: the path from the project file directory to the solution file directory.
* `{fileToProjectPath}`: the physical path from the file adjusting the path of to the project directory.

The default namespace format is specified as: `{projectName}{fileToProjectPath}`.

The source code can be found on [GitHub](https://github.com/pauer24/VsNamespaceFixer).
The extension is available in the [Marketplace](https://marketplace.visualstudio.com/items?itemName=p2410.NamespaceFixer).

Hope it fits for you.

![Example](/Example.png)

Special thanks go to the pull-requesters:
* [@Tr4ncer](https://github.com/Tr4ncer)
* [@spottedmahn](https://github.com/spottedmahn)
* [@mniak](https://github.com/mniak)
* [@DawidB](https://github.com/DawidB)
* [@angelobreuer](https://github.com/angelobreuer)
