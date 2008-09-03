using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	public class Category : INotifyPropertyChanged
	{
		public Category(string name)
		{
			Name = name;
			Properties = new PropertyNodeCollection();
			//MoreProperties = new ObservableCollection<PropertyNode>();
		}

		public string Name { get; private set; }
		public PropertyNodeCollection Properties { get; private set; }
		//public ObservableCollection<PropertyNode> MoreProperties { get; private set; }

		bool isExpanded = true;

		public bool IsExpanded {
			get {
				return isExpanded;
			}
			set {
				isExpanded = value;
				RaisePropertyChanged("IsExpanded");
			}
		}

		//bool showMore;
		//internal bool ShowMoreByFilter;

		//public bool ShowMore {
		//    get {
		//        return showMore;
		//    }
		//    set {
		//        showMore = value;
		//        RaisePropertyChanged("ShowMore");
		//    }
		//}

		bool isVisible;

		public bool IsVisible
		{
			get
			{
				return isVisible;
			}
			set
			{
				isVisible = value;
				RaisePropertyChanged("IsVisible");
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
