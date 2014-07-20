using System.Collections.ObjectModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui
{
    public class NewFileCategory : ViewModelBase
    {
        string _displayName;

        public NewFileCategory(string displayName)
        {
            Children = new ObservableCollection<NewFileCategory>();
            Templates = new ObservableCollection<NewFileTemplate>();
            DisplayName =  StringParser.Parse(displayName);
        }

        public ObservableCollection<NewFileCategory> Children { get; set; }
        public ObservableCollection<NewFileTemplate> Templates { get; set; }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }
    }
}