// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using System;
using System.Windows.Forms;

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
			GenerateDropDownItems();
		}
		
		void GenerateDropDownItems()
		{
			ToolStripItem[] items = (ToolStripItem[])(AddInTree.GetTreeNode("/SharpDevelop/Pads/CodeCoveragePad/Toolbar/CodeCoveragePadDisplayOptions").BuildChildItems(this)).ToArray(typeof(ToolStripItem));
			foreach (ToolStripItem item in items) {
				if (item is IStatusUpdate) {
					((IStatusUpdate)item).UpdateStatus();
				}
			}
			dropDownButton.DropDownItems.AddRange(items);
		}
	}
}
