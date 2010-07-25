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
	internal class CustomizeDialog : Dialog
	{
		internal CustomizeDialog()
		{
		}

		public void DoShowDialog(ICustPanel panel)
		{
			Panel realPanel = (Panel)panel;

			System.Drawing.Size panelSize = panel.PreferredSize;
			if (panelSize != System.Drawing.Size.Empty) {
				Size = panelSize;
			} else {
				Width = 300;
				Height = 400;
			}

			Text = realPanel.Text;
			realPanel.Dock = DockStyle.Fill;
			Controls.Add(realPanel);
			
			panel.BeforeShow();

			while (true) {
				if (ShowDialog() != DialogResult.OK) {
					Controls.Remove(realPanel);
					return;
				}

				if (panel.AfterShow())
					break;

				continue;
			}

			Controls.Remove(realPanel);
		}
	}
}
