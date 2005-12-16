// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		
		public TreeListViewDebuggerItem(Variable variable)
		{
			this.variable = variable;
			
			variable.ValueChanged += Update;
			
			variable.ValueRemovedFromCollection += delegate {
				variable.ValueChanged -= Update;
				this.Remove();
			};
			
			SubItems.Add("");
			SubItems.Add("");
			
			Update();
		}
		
		void Update(object sender, DebuggerEventArgs e)
		{
			Highlight = (Variable.Value.AsString != SubItems[1].Text);
			Update();
		}
		
		public void Update()
		{
			if (this.SubItems[0].Text != Variable.Name)
				this.SubItems[0].Text = Variable.Name;
			if (this.SubItems[1].Text != Variable.Value.AsString)
				this.SubItems[1].Text = Variable.Value.AsString;
			if (this.SubItems[2].Text != Variable.Value.Type)
				this.SubItems[2].Text = Variable.Value.Type;
			
			int imageIndex = DebuggerIcons.GetImageListIndex(variable);
			if (this.ImageIndex != imageIndex) {
				this.ImageIndex = imageIndex;
			}
			
			if (IsExpanded) {
				variable.SubVariables.Update();
			} else {
				// Show plus sign
				if (variable.Value.MayHaveSubVariables && Items.Count == 0) {
					TreeListViewItem dummy = new TreeListViewItem();
					this.AfterExpand += delegate { dummy.Remove(); };
					Items.Add(dummy);
				}
			}
		}
		
		public void BeforeExpand()
		{
			if (populated) {
				variable.SubVariables.Update();
			} else {
				Populate();
				populated = true;
			}
		}
		
		public void Populate()
		{
			Items.Clear();
			
			// Do not sort names of array items
			if (variable.Value is ArrayValue) {
				this.Items.SortOrder = SortOrder.None;
			} else {
				this.Items.SortOrder = SortOrder.Ascending;
			}
			
			LocalVarPad.AddVariableCollectionToTree(variable.SubVariables, this.Items);
		}
		
	}
}
