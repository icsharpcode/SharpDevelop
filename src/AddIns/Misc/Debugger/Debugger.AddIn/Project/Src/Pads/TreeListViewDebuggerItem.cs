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
		bool created;
		
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
			variable.ValueChanged += delegate { Update(); };
			
			SubItems.Add("");
			SubItems.Add("");
			
			Update();
			
			created = true; // Used to prevent highlighting of new variables
		}
		
		public void Update()
		{
			Highlight = (Variable.Value.AsString != SubItems[1].Text && created);
			
			this.SubItems[0].Text = Variable.Name;
			this.SubItems[1].Text = Variable.Value.AsString;
			this.SubItems[2].Text = Variable.Value.Type;
			
			if (variable.Value is ObjectValue) {
				this.ImageIndex = 0; // Class
			} else if (variable is PropertyVariable){
				this.ImageIndex = 2; // Property
			} else {
				this.ImageIndex = 1; // Field
			}
			
//			if (IsExpanded) {
//				UpdateSubVariables();
//			} else {
//				if (variable.Value.MayHaveSubVariables) {
//					Items.Add(new PlaceHolderItem()); // Show plus icon
//				}
//			}
		}
		
		public void BeforeExpand()
		{
			
		}
		
//		public GetBaseClass()
//		{
//			ObjectValue objectValue = uncastedVariable.Value as ObjectValue;
//			if (objectValue != null && objectValue.HasBaseClass && objectValue.BaseClass.Type != "System.Object") {
//				this.Variable = VariableFactory.CreateVariable(objectValue.BaseClass, uncastedVariable.Name);
//			}
//		}
//		
//		protected void UpdateSubVariables() {
//			if (!baseClassItemAdded) {
//				VariableListItem baseClassItem = new BaseClassItem(variable);
//				if (baseClassItem.IsValid) {
//					this.Items.Add(baseClassItem);
//				}
//				baseClassItemAdded = true;
//			}
//			
//			// Do not sort names of array items
//			if (Variable.Value is ArrayValue) {
//				this.Items.SortOrder = SortOrder.None;
//			} else {
//				this.Items.SortOrder = SortOrder.Ascending;
//			}
//			
//			LocalVarPad.UpdateVariables(this.Items, Variable.Value.SubVariables);
//		}
		
	}
}
