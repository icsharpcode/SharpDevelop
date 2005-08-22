// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	class BaseClassItem: VariableItem
	{
		public BaseClassItem(Variable uncastedVariable)
		{
			ObjectVariable variable = uncastedVariable as ObjectVariable;
			if (variable != null && variable.HasBaseClass && variable.BaseClass.Type != "System.Object") {
				this.Variable = variable.BaseClass;
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
			         Variable.Value.ToString(),
			         Variable.Type);
			
			ImageIndex = 0; // Class
			
			if (IsExpanded) {
				UpdateSubVariables();
			} else {
				if (Variable.MayHaveSubVariables) { // Always true
					Items.Add(new PlaceHolderItem()); // Show plus icon
				}
			}
		}
	}
}
