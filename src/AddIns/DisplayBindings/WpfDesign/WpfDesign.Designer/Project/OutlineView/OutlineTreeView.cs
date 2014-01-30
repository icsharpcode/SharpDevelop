// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	public class OutlineTreeView : DragTreeView
	{
		protected override bool CanInsert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
		{
			UpdateCustomNodes(items);
			return (target.DataContext as IOutlineNode).CanInsert(_customOutlineNodes,
			                                                     after == null ? null : after.DataContext as IOutlineNode, copy);
		}

		protected override void Insert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
		{
			UpdateCustomNodes(items);
			(target.DataContext as IOutlineNode).Insert(_customOutlineNodes,
			                                           after == null ? null : after.DataContext as IOutlineNode, copy);
		}
		
		// Need to do this through a seperate List since previously LINQ queries apparently disconnected DataContext;bug in .NET 4.0
		private List<IOutlineNode> _customOutlineNodes;

		void UpdateCustomNodes(IEnumerable<DragTreeViewItem> items)
		{
			_customOutlineNodes = new List<IOutlineNode>();
			foreach (var item in items)
				_customOutlineNodes.Add(item.DataContext as IOutlineNode);
		}
		
		public override bool ShouldItemBeVisible(DragTreeViewItem dragTreeViewitem)
		{
			var node = dragTreeViewitem.DataContext as IOutlineNode;
			
			return string.IsNullOrEmpty(Filter) || node.Name.ToLower().Contains(Filter.ToLower());
		}
		
		protected override void SelectOnly(DragTreeViewItem item)
		{
			base.SelectOnly(item);
			
			var node = item.DataContext as IOutlineNode;
			
			var surface = node.DesignItem.View.TryFindParent<DesignSurface>();
			if (surface != null)
				surface.ScrollIntoView(node.DesignItem);
		}
	}
}
