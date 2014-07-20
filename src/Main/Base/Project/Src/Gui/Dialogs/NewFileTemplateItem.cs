using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.Core;
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
        public string Description { get { 
        		return StringParser.Parse(Template.Description); 
        	} }
        
        public ImageSource Picture { get{
        		return Template.Icon.ImageSource;
        	}
        	}
        public string Name { get { return Template.Name; } }
    }
}