using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ICSharpCode.WpfDesign;
using System.Collections.ObjectModel;
using System.Collections;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.XamlBackend;

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	public class OutlineNode : INotifyPropertyChanged
	{
		public static OutlineNode Create(DesignItem item)
		{
			var xamlDesignItem = item as XamlDesignItem;
			var result = xamlDesignItem.GetAnnotation<OutlineNode>();
			if (result == null) {
				result = new OutlineNode(item);
				xamlDesignItem.AnnotateWith(result);
			}
			return result;
		}

		OutlineNode(DesignItem designItem)
		{
			DesignItem = designItem;
		}

		public DesignItem DesignItem { get; private set; }

		bool isExpanded = true;

		public bool IsExpanded
		{
			get
			{
				return isExpanded;
			}
			set
			{
				isExpanded = value;
				RaisePropertyChanged("IsExpanded");
			}
		}

		public bool IsSelected
		{
			get
			{
				return DesignItem.Context.SelectionService.IsSelected(DesignItem);
			}
			set
			{
				DesignItem.Context.SelectionService.Select(
					new[] { DesignItem }, value ? SelectionTypes.Add : SelectionTypes.Remove);
				RaisePropertyChanged("IsSelected");
			}
		}

		public OutlineNode[] Children
		{
			get
			{
				return ModelTools.Children(DesignItem)
					.Select(item => Create(item)).ToArray();
			}
		}

		public string Name
		{
			get
			{
				if (string.IsNullOrEmpty(DesignItem.Name)) {
					return DesignItem.ComponentType.Name;
				}
				return DesignItem.ComponentType.Name + " (" + DesignItem.Name + ")";
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
