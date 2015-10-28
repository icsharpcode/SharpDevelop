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
	public interface IOutlineNode
	{
		ISelectionService SelectionService { get; }
		bool IsExpanded { get; set; }
		DesignItem DesignItem { get; set; }
		bool IsSelected { get; set; }
		bool IsDesignTimeVisible { get; set; }
		bool IsDesignTimeLocked { get; }
		string Name { get; }
		bool CanInsert(IEnumerable<IOutlineNode> nodes, IOutlineNode after, bool copy);
		void Insert(IEnumerable<IOutlineNode> nodes, IOutlineNode after, bool copy);
		ObservableCollection<IOutlineNode> Children{ get; }
	}
	

	public class OutlineNode: OutlineNodeBase
	{
		//TODO: Reset with DesignContext
		static Dictionary<DesignItem, IOutlineNode> outlineNodes = new Dictionary<DesignItem, IOutlineNode>();

		protected OutlineNode(DesignItem designitem): base(designitem)
		{
			UpdateChildren();
			SelectionService.SelectionChanged += new EventHandler<DesignItemCollectionEventArgs>(Selection_SelectionChanged);
		}

		static OutlineNode()
		{
			DummyPlacementType = PlacementType.Register("DummyPlacement");
		}

		public static IOutlineNode Create(DesignItem designItem)
		{
			IOutlineNode node = null;
			if (designItem != null && !outlineNodes.TryGetValue(designItem, out node)) {
				node = new OutlineNode(designItem);
				outlineNodes[designItem] = node;
			}
			return node;
		}

		void Selection_SelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			IsSelected = DesignItem.Services.Selection.IsComponentSelected(DesignItem);
		}

		protected override void UpdateChildren()
		{
			Children.Clear();

			if (DesignItem.ContentPropertyName != null) {
				var content = DesignItem.ContentProperty;
				if (content.IsCollection) {
					UpdateChildrenCore(content.CollectionElements);
				} else {
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
				} else {
					var content = item.ContentProperty;
					if (content != null) {
						if (content.IsCollection) {
							UpdateChildrenCore(content.CollectionElements);
						} else {
							if (content.Value != null) {
								UpdateChildrenCore(new[] { content.Value });
							}
						}
					}
				}
			}
		}
	}
}
