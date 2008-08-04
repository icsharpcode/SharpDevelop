using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.XamlDesigner
{
	public class OutlineTreeView : DragTreeView
	{
		protected override bool CanInsert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
		{
			return (target.DataContext as OutlineNode).CanInsert(items.Select(t => t.DataContext as OutlineNode), 
				after == null ? null : after.DataContext as OutlineNode, copy);
		}

		protected override void Insert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
		{
			(target.DataContext as OutlineNode).Insert(items.Select(t => t.DataContext as OutlineNode), 
				after == null ? null : after.DataContext as OutlineNode, copy);
		}
	}
}
