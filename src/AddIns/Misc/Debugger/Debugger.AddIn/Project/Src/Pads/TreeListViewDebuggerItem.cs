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
		Variable variable;
		bool populated = false;
		bool dirty = true;
		
		public Variable Variable {
			get {
				return variable;
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
		
		public TreeListViewDebuggerItem(Variable variable)
		{
			this.variable = variable;
			
			variable.ValueChanged += delegate { dirty = true; Update(); };
			variable.Expired += delegate { this.Remove(); };
			
			SubItems.Add("");
			SubItems.Add("");
			
			Update();
		}
		
		public void Update()
		{
			if (!dirty) return;
			if (!IsVisible) return;
			
			if (this.TreeListView != null) {
				((DebuggerTreeListView)this.TreeListView).DelayRefresh();
				Highlight = (Variable.Value.AsString != SubItems[1].Text);
			}
			
			this.SubItems[0].Text = Variable.Name;
			this.SubItems[1].Text = Variable.Value.AsString;
			this.SubItems[2].Text = Variable.Value.Type;
			
			this.ImageIndex = DebuggerIcons.GetImageListIndex(variable);
			
			if (!IsExpanded) {
				// Show plus sign
				if (variable.Value.MayHaveSubVariables && Items.Count == 0) {
					TreeListViewItem dummy = new TreeListViewItem();
					this.AfterExpand += delegate { dummy.Remove(); };
					Items.Add(dummy);
				}
			}
			
			dirty = false;
		}
		
		public void BeforeExpand()
		{
			if (!populated) {
				Items.Clear();
				// Do not sort names of array items
				this.Items.SortOrder = variable.Value is ArrayValue ? SortOrder.None : SortOrder.Ascending;
				LocalVarPad.AddVariableCollectionToTree(variable.Value.SubVariables, this.Items);
				populated = true;
			}
		}
	}
}
