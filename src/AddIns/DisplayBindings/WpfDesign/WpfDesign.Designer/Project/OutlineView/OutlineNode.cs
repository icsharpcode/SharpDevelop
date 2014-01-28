// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			IOutlineNode node;
			if (!outlineNodes.TryGetValue(designItem, out node)) {
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
