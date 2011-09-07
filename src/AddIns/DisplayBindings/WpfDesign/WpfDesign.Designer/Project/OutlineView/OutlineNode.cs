// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ICSharpCode.WpfDesign;
using System.Collections.ObjectModel;
using System.Collections;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	public class OutlineNode : INotifyPropertyChanged
	{
		//Used to check if element can enter other containers
		public static PlacementType DummyPlacementType;
		
		static OutlineNode()
		{
			DummyPlacementType = PlacementType.Register("DummyPlacement");
		}
		public static OutlineNode Create(DesignItem designItem)
		{
			OutlineNode node;
			if (!outlineNodes.TryGetValue(designItem, out node)) {
				node = new OutlineNode(designItem);
				outlineNodes[designItem] = node;
			}
			return node;
		}

		//TODO: Reset with DesignContext
		static Dictionary<DesignItem, OutlineNode> outlineNodes = new Dictionary<DesignItem, OutlineNode>();

		OutlineNode(DesignItem designItem)
		{
			DesignItem = designItem;
			UpdateChildren();

			//TODO
			DesignItem.NameChanged += new EventHandler(DesignItem_NameChanged);
			DesignItem.PropertyChanged += new PropertyChangedEventHandler(DesignItem_PropertyChanged);
			SelectionService.SelectionChanged += new EventHandler<DesignItemCollectionEventArgs>(Selection_SelectionChanged);
		}

		public DesignItem DesignItem { get; private set; }

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
			Children.Clear();

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
			foreach (var item in items) {
				if (ModelTools.CanSelectComponent(item)) {
					var node = OutlineNode.Create(item);
					Children.Add(node);
				}
			}
		}

		public bool CanInsert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
		{
			var placementBehavior = DesignItem.GetBehavior<IPlacementBehavior>();
			if (placementBehavior == null)
				return false;
			var operation = PlacementOperation.Start(nodes.Select(node => node.DesignItem).ToArray(), DummyPlacementType);
			if (operation != null) {
				bool canEnter = placementBehavior.CanEnterContainer(operation);
				operation.Abort();
				return canEnter;
			}
			return false;
		}

		public void Insert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
		{
			using (var moveTransaction = DesignItem.Context.OpenGroup("Item moved in outline view", nodes.Select(n => n.DesignItem).ToList())) {
				if (copy) {
					nodes = nodes.Select(n => OutlineNode.Create(n.DesignItem.Clone())).ToList();
				} else {
					foreach (var node in nodes) {
						node.DesignItem.Remove();
					}
				}

				var index = after == null ? 0 : Children.IndexOf(after) + 1;

				var content = DesignItem.ContentProperty;
				if (content.IsCollection) {
					foreach (var node in nodes) {
						content.CollectionElements.Insert(index++, node.DesignItem);
					}
				} else {
					content.SetValue(nodes.First().DesignItem);
				}
				moveTransaction.Commit();
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
