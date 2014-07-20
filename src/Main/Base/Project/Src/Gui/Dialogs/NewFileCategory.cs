using System.Collections.ObjectModel;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui
{
    public class NewFileCategory : ViewModelBase
    {
        private string _displayName;

        public NewFileCategory(string displayName)
        {
            Children = new ObservableCollection<NewFileCategory>();
            Templates = new ObservableCollection<NewFileTemplateItem>();
            DisplayName = displayName;
        }

        public ObservableCollection<NewFileCategory> Children { get; set; }
        public ObservableCollection<NewFileTemplateItem> Templates { get; set; }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmpty
        {
            get { return Children.Count == 0; }
        }
    }
}