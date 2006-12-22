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
		NamedValue val;
		bool populated = false;
		bool dirty = true;
		
		public NamedValue Value {
			get {
				return val;
			}
		}
		
		public bool Highlight {
			set {
				if (value) {
					if (SubItems[1].ForeColor != Color.Blue) { // smart update
						SubItems[1].ForeColor = Color.Blue;
						SubItems[1].Font = new Font(SubItems[1].Font, FontStyle.Bold);
					}
				} else {
					if (SubItems[1].ForeColor != Color.Black) { // smart update
						SubItems[1].ForeColor = Color.Black;
						SubItems[1].Font = new Font(SubItems[1].Font, FontStyle.Regular);
					}
				}
			}
		}
		
		bool IsVisible {
			get {
				if (this.Parent == null) return true;
				foreach(TreeListViewItem parent in this.ParentsInHierarch) {
					if (!parent.IsExpanded) return false;
				}
				return true;
			}
		}
		
		public TreeListViewDebuggerItem(NamedValue val)
		{
			this.val = val;
			
			val.Changed += delegate { dirty = true; Update(); };
			val.Expired += delegate { this.Remove(); };
			
			SubItems.Add("");
			SubItems.Add("");
			
			Update();
		}
		
		public void Update()
		{
			if (!dirty) return;
			if (!IsVisible) return;
			
			DateTime startTime = Debugger.Util.HighPrecisionTimer.Now;
			
			if (this.TreeListView != null) {
				((DebuggerTreeListView)this.TreeListView).DelayRefresh();
				Highlight = (val.AsString != SubItems[1].Text);
			}
			
			this.SubItems[0].Text = val.Name;
			this.SubItems[1].Text = val.AsString;
			this.SubItems[2].Text = val.IsNull ? String.Empty : val.Type.Name;
			
			this.ImageIndex = DebuggerIcons.GetImageListIndex(val);
			
			if (!IsExpanded) {
				// Show plus sign
				if ((val.IsObject || val.IsArray) && Items.Count == 0) {
					TreeListViewItem dummy = new TreeListViewItem();
					this.AfterExpand += delegate { dummy.Remove(); };
					Items.Add(dummy);
				}
			}
			
			dirty = false;
			
			TimeSpan totalTime = Debugger.Util.HighPrecisionTimer.Now - startTime;
			//val.Process.TraceMessage("Local Variables Pad item updated: " + val.Name + " (" + totalTime.TotalMilliseconds + " ms)");
		}
		
		public void BeforeExpand()
		{
			if (!populated) {
				Items.Clear();
				if (val.IsArray) {
					// Do not sort names of array items
					this.Items.SortOrder = SortOrder.None;
					LocalVarPad.AddVariableCollectionToTree(val.GetArrayElements(), this.Items);
				} else if (val.IsObject) {
					this.Items.SortOrder = SortOrder.Ascending;
					LocalVarPad.AddVariableCollectionToTree(val.GetMembers(), this.Items);
				}
				populated = true;
			}
		}
	}
}
