// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Ivan Shumilin"/>
//     <version>$Revision$</version>
// </file>

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
