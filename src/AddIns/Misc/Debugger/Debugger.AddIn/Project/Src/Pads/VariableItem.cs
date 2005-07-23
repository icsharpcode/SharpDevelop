using System;
using System.Collections.Generic;
using System.Text;
using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	class VariableItem: VariableListItem
	{
		Variable variable;

		bool baseClassItemAdded = false;

		public Variable Variable {
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

		public VariableItem(Variable variable): base()
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
			         variable.Value.ToString(),
			         variable.Type);

			if (variable is ObjectVariable) {
				ImageIndex = 0; // Class
			} else if (variable is PropertyVariable){
				ImageIndex = 2; // Property
			} else {
				ImageIndex = 1; // Field
			}

			if (variable.MayHaveSubVariables && !IsExpanded) {
				Items.Add(new PlaceHolderItem()); // Show plus icon
			}

			if (IsExpanded) {
				UpdateSubVariables();
			}
		}

		void UpdateSubVariables() {
			if (!baseClassItemAdded) {
				VariableListItem baseClassItem = new BaseClassItem(variable);
				if (baseClassItem.IsValid) {
					this.Items.Add(baseClassItem);
				}
				baseClassItemAdded = true;
			}
			LocalVarPad.UpdateVariables(this.Items, Variable.SubVariables);
		}
	}
}
