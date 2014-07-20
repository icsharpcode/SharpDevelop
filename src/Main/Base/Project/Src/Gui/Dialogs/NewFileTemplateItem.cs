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
        public string DisplayName { get { return Template.DisplayName; } }
        public string Description { get { return Template.Description; } }
        public string Name { get { return Template.Name; } }
    }
}