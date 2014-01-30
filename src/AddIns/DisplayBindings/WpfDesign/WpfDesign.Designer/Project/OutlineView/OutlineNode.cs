// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Xaml;
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

			var hidden = designItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).ValueOnInstance;
			if (hidden != null && (bool) hidden == true) {
				this._isDesignTimeVisible = false;
				((FrameworkElement) this.DesignItem.Component).Visibility = Visibility.Hidden;
			}

			var locked = designItem.Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).ValueOnInstance;
			if (locked != null && (bool) locked == true) {
				this._isDesignTimeLocked = true;
			}
			
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
		
		bool _isDesignTimeVisible = true;

		public bool IsDesignTimeVisible
		{
			get {
				return _isDesignTimeVisible;
			}
			set {
				_isDesignTimeVisible = value;
				var ctl = DesignItem.Component as UIElement;
				ctl.Visibility = _isDesignTimeVisible ? Visibility.Visible : Visibility.Hidden;

				RaisePropertyChanged("IsDesignTimeVisible");

				if (!value)
					DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).SetValue(true);
				else
					DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).Reset();
			}
		}

		bool _isDesignTimeLocked = false;
		
		public bool IsDesignTimeLocked
		{
			get {
				return _isDesignTimeLocked;
			}
			set {
				_isDesignTimeLocked = value;
				((XamlDesignItem)DesignItem).IsDesignTimeLocked = _isDesignTimeLocked;
				
				RaisePropertyChanged("IsDesignTimeLocked");

//				if (value)
//					DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).SetValue(true);
//				else
//					DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).Reset();
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
				else
				{
					var content = item.ContentProperty;
					if (content != null)
					{
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
			}
		}

		public bool CanInsert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
		{
			var placementBehavior = DesignItem.GetBehavior<IPlacementBehavior>();
			if (placementBehavior == null)
				return false;
			var operation = PlacementOperation.Start(nodes.Select(node => node.DesignItem).ToArray(), DummyPlacementType);
			if (operation != null) {
				bool canEnter = placementBehavior.CanEnterContainer(operation, true);
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
