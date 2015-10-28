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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.WpfDesign.UIExtensions;

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
			
			return string.IsNullOrEmpty(Filter) || node.DesignItem.Services.GetService<IOutlineNodeNameService>().GetOutlineNodeName(node.DesignItem).ToLower().Contains(Filter.ToLower());
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
