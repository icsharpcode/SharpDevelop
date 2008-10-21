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
			return ModelTools.CanInsert(
				(target.DataContext as OutlineNode).DesignItem,
				items.Select(t => (t.DataContext as OutlineNode).DesignItem),
				after == null ? null : (after.DataContext as OutlineNode).DesignItem,
				copy);
		}

		protected override void Insert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
		{
			ModelTools.Insert(
				(target.DataContext as OutlineNode).DesignItem,
				items.Select(t => (t.DataContext as OutlineNode).DesignItem),
				after == null ? null : (after.DataContext as OutlineNode).DesignItem,
				copy);
		}
	}
}
