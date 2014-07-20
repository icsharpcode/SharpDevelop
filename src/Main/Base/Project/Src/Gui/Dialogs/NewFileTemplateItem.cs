using ICSharpCode.SharpDevelop.Templates;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui
{
    public class NewFileTemplateItem : ViewModelBase
    {
        public NewFileTemplateItem(FileTemplate template)
        {
            Template = template;
        }

        public FileTemplate Template { get; private set; }
    }
}