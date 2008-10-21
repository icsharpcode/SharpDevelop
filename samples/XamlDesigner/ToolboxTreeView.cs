using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.WpfDesign.Designer.OutlineView;

namespace ICSharpCode.XamlDesigner
{
	class ToolboxTreeView : DragTreeView
	{
		protected override bool CanInsert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
		{
			return base.CanInsert(target, items, after, copy);
		}

		protected override void Insert(DragTreeViewItem target, DragTreeViewItem[] items, DragTreeViewItem after, bool copy)
		{
			base.Insert(target, items, after, copy);
		}

		protected override void Remove(DragTreeViewItem target, DragTreeViewItem item)
		{
			base.Remove(target, item);
		}
	}
}
