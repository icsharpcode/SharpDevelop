// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class TreeListViewDebuggerItem: TreeListViewItem
	{
		ListItem listItem;
		
		bool populated = false;
		
		public ListItem ListItem {
			get {
				return listItem;
			}
		}
		
		bool IsVisible {
			get {
				if (this.Parent == null) {
					return true;
				} else {
					foreach(TreeListViewItem parent in this.ParentsInHierarch) {
						if (!parent.IsExpanded) return false;
					}
					return true;
				}
			}
		}
		
		public TreeListViewDebuggerItem(NamedValue val): this(new ValueItem(val))
		{
			
		}
		
		public TreeListViewDebuggerItem(ListItem listItem)
		{
			this.listItem = listItem;
			
			listItem.Changed += delegate { Update(); };
			
			SubItems.Add("");
			SubItems.Add("");
			
			Update();
		}
		
		public void Update()
		{
			if (!IsVisible) return;
			
			if (this.TreeListView != null) {
				((DebuggerTreeListView)this.TreeListView).DelayRefresh();
			}
			
			this.ImageIndex = listItem.ImageIndex;
			this.SubItems[0].Text = listItem.Name;
			this.SubItems[1].Text = listItem.Text;
			this.SubItems[2].Text = listItem.Type;
			
			if (!IsExpanded && !populated && listItem.HasSubItems) {
				Items.Add(new TreeListViewItem()); // Show plus sign
			}
		}
		
		public void Populate()
		{
			if (!populated) {
				Items.Clear();
				this.Items.SortOrder = SortOrder.None;
				foreach(ListItem subItem in listItem.SubItems) {
					Items.Add(new TreeListViewDebuggerItem(subItem));
				}
				populated = true;
			}
		}
	}
}
