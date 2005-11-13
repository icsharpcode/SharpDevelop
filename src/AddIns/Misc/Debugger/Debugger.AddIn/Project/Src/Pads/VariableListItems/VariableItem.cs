// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	class VariableItem: VariableListItem
	{
		Value variable;

		bool baseClassItemAdded = false;

		public Value Variable {
			get {
				return variable;
			}
			set {
				variable = value;
			}
		}

		public override bool IsValid {
			get {
				return variable != null &&
				       !variable.Name.StartsWith("CS$");
			}
		}
		
		protected VariableItem()
		{
			
		}
		
		public VariableItem(Value variable): base()
		{
			this.variable = variable;
			Refresh();
		}

		public override void PrepareForExpansion()
		{
			UpdateSubVariables();
		}

		public override void Refresh()
		{
			if (!IsValid) {
				return;
			}

			SetTexts(variable.Name,
			         variable.AsString.ToString(),
			         variable.Type);

			if (variable is ObjectValue) {
				ImageIndex = 0; // Class
			} else if (variable is PropertyVariable){
				ImageIndex = 2; // Property
			} else {
				ImageIndex = 1; // Field
			}
			
			if (IsExpanded) {
				UpdateSubVariables();
			} else {
				if (variable.MayHaveSubVariables) {
					Items.Add(new PlaceHolderItem()); // Show plus icon
				}
			}
		}

		protected void UpdateSubVariables() {
			if (!baseClassItemAdded) {
				VariableListItem baseClassItem = new BaseClassItem(variable);
				if (baseClassItem.IsValid) {
					this.Items.Add(baseClassItem);
				}
				baseClassItemAdded = true;
			}
			
			// Do not sort names of array items
			if (Variable is ArrayValue) {
				this.Items.SortOrder = SortOrder.None;
			} else {
				this.Items.SortOrder = SortOrder.Ascending;
			}
			
			LocalVarPad.UpdateVariables(this.Items, Variable.SubVariables);
		}
	}
}
