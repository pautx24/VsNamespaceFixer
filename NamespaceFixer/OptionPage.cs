using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace NamespaceFixer
{
    internal class OptionPage : DialogPage, INamespaceAdjusterOptions
    {
        [Category("My Category")]
        [DisplayName("Use project specified namespace")]
        [Description("Use project specified namespace instead of the containing folder")]
        public bool UseProjectDefaultNamespace { get; set; } = false;
    }
}