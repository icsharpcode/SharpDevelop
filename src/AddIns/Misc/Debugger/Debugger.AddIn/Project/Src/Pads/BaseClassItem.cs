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
	class BaseClassItem: VariableListItem
	{
		ObjectVariable variable;

		public override bool IsValid {
			get {
				return variable != null &&
				       variable.HasBaseClass &&
				       variable.BaseClass.Type != "System.Object";
			}
		}

		public BaseClassItem(Variable baseClassOfVariable): base()
		{
			this.variable = baseClassOfVariable as ObjectVariable;
			Refresh();
		}

		public override void Refresh()
		{
			if (!IsValid) {
				return;
			}

			SetTexts("<Base class>",
			         variable.BaseClass.Value.ToString(),
			         variable.BaseClass.Type);

			ImageIndex = 0; // Class

			if (variable.BaseClass.MayHaveSubVariables) { // Always true
				Items.Add(new PlaceHolderItem()); // Show plus icon
			}
		}
	}
}
