// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.CodeCoverage
{
	public class ShowDisplayOptionsDropDown : AbstractMenuCommand
	{
		ToolBarDropDownButton dropDownButton;

		public override void Run()
		{
		}
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			dropDownButton = (ToolBarDropDownButton)Owner;
			MenuService.AddItemsToMenu(dropDownButton.DropDownItems, this, "/SharpDevelop/Pads/CodeCoveragePad/Toolbar/CodeCoveragePadDisplayOptions");
		}		
	}
}
