using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ICSharpCode.WpfDesign;
using System.Collections.ObjectModel;
using System.Collections;

namespace ICSharpCode.XamlDesigner
{
	public class OutlineNode : INotifyPropertyChanged
	{
		public OutlineNode(DesignItem designItem)
		{
			DesignItem = designItem;
			UpdateChildren();

			//TODO (possible bug)
			DesignItem.NameChanged += new EventHandler(DesignItem_NameChanged);
			DesignItem.PropertyChanged += new PropertyChangedEventHandler(DesignItem_PropertyChanged);
			SelectionService.SelectionChanged += new EventHandler<DesignItemCollectionEventArgs>(Selection_SelectionChanged);
		}

		bool freezeChildren;

		public DesignItem DesignItem { get; private set; }
		public OutlineNode Parent { get; private set; }

		public ISelectionService SelectionService {
			get { return DesignItem.Services.Selection; }
		}

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

		bool isSelected;

		public bool IsSelected {
			get {
				return isSelected;
			}
			set {
				if (isSelected != value) {
					isSelected = value;
					SelectionService.SetSelectedComponents(new[] { DesignItem }, 
						value ? SelectionTypes.Add : SelectionTypes.Remove);
					RaisePropertyChanged("IsSelected");
				}
			}
		}

		ObservableCollection<OutlineNode> children = new ObservableCollection<OutlineNode>();

		public ObservableCollection<OutlineNode> Children  {
			get { return children; }
		}

		public string Name {
			get  {
				if (string.IsNullOrEmpty(DesignItem.Name)) {
					return DesignItem.ComponentType.Name;					
				}
				return DesignItem.ComponentType.Name + " (" + DesignItem.Name + ")";
			}
		}

		void Selection_SelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			IsSelected = DesignItem.Services.Selection.IsComponentSelected(DesignItem);
		}

		void DesignItem_NameChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged("Name");
		}

		void DesignItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == DesignItem.ContentPropertyName) {
				UpdateChildren();
			}
		}

		void UpdateChildren()
		{
			if (freezeChildren) return;
			if (DesignItem.ContentPropertyName != null) {
				var content = DesignItem.ContentProperty;
				if (content.IsCollection) {
					UpdateChildrenCore(content.CollectionElements);
				}
				else {
					if (content.Value != null) {
						UpdateChildrenCore(new[] { content.Value });
					}
				}
			}
		}

		void UpdateChildrenCore(IEnumerable<DesignItem> items)
		{
			var cache = Children.ToDictionary(n => n.DesignItem);
			
			Children.Clear();

			foreach (var item in items) {
				OutlineNode node;
				if (!cache.TryGetValue(item, out node)) {
					node = new OutlineNode(item);
				}
				Children.Add(node);
				node.Parent = this;
			}
		}

		public bool CanInsert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
		{
			if (DesignItem.ContentPropertyName == null) return false;

			if (DesignItem.ContentProperty.IsCollection) {
				foreach (var node in nodes) {
					if (!CanAdd(DesignItem.ContentProperty.ReturnType, 
						node.DesignItem.ComponentType)) {
						return false;
					}
				}
				return true;
			}
			else {
				return after == null && nodes.Count() == 1 &&
					DesignItem.ContentProperty.DeclaringType.IsAssignableFrom(
					nodes.First().DesignItem.ComponentType);
			}
		}

		public static bool CanAdd(Type col, Type item)
		{
			var e = col.GetInterface("IEnumerable`1");
			if (e != null && e.IsGenericType) {
				var a = e.GetGenericArguments()[0];
				return a.IsAssignableFrom(item);
			}
			return true;
		}

		public void Insert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
		{
			freezeChildren = true;

			if (copy) {
				nodes = nodes.Select(n => new OutlineNode(n.DesignItem.Clone()));
			}
			else {
				foreach (var node in nodes) {
					Remove(node);
				}
			}

			var index = after == null ? 0 : Children.IndexOf(after) + 1;
			var tempIndex = index;

			foreach (var node in nodes) {
				Children.Insert(tempIndex++, node);
				node.Parent = this;
			}

			var content = DesignItem.ContentProperty;
			if (content.IsCollection) {
				tempIndex = index;
				foreach (var node in nodes) {
					content.CollectionElements.Insert(tempIndex++, node.DesignItem);
				}
			}
			else {
				content.SetValue(nodes.First().DesignItem);
			}

			freezeChildren = false;
		}

		void Remove(OutlineNode node)
		{
			node.DesignItem.Remove();

			if (node.Parent != null) {
				node.Parent.Children.Remove(node);
				node.Parent = null;
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
