// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using NoGoop.ObjBrowser.Panels;

namespace NoGoop.ObjBrowser.Dialogs
{
	public class Dialog : Form
	{
		internal const bool         INCLUDE_BUTTONS = true;

		internal Dialog() : this(INCLUDE_BUTTONS)
		{
		}

		internal Dialog(bool includeButtons)
		{
			DockPadding.All = 10;
			StartPosition = FormStartPosition.CenterParent;
			MinimizeBox = false;
			MaximizeBox = false;
			FormBorderStyle = FormBorderStyle.FixedDialog;

			if (includeButtons){
				ButtonPanel bp = new ButtonPanel(this, ButtonPanel.CANCEL);
				bp.Dock = DockStyle.Bottom;
				Controls.Add(bp);
			}
		}
	}
}
