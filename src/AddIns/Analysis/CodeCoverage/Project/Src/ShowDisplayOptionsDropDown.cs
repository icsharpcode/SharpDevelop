// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
