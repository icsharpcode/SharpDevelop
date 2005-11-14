// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	class BaseClassItem: VariableItem
	{
		public BaseClassItem(Variable uncastedVariable)
		{
			ObjectValue objectValue = uncastedVariable.Value as ObjectValue;
			if (objectValue != null && objectValue.HasBaseClass && objectValue.BaseClass.Type != "System.Object") {
				this.Variable = VariableFactory.CreateVariable(objectValue.BaseClass, uncastedVariable.Name);
			} else {
				this.Variable = null;
			}
			Refresh();
		}

		public override void Refresh()
		{
			if (!IsValid) {
				return;
			}
			
			SetTexts("<Base class>",
			         Variable.Value.AsString.ToString(),
			         Variable.Value.Type);
			
			ImageIndex = 0; // Class
			
			if (IsExpanded) {
				UpdateSubVariables();
			} else {
				if (Variable.Value.MayHaveSubVariables) { // Always true
					Items.Add(new PlaceHolderItem()); // Show plus icon
				}
			}
		}
	}
}
