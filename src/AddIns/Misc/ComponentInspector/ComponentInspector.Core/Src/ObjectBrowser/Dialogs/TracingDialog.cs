// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

using NoGoop.Controls;
using NoGoop.Debug;
using NoGoop.ObjBrowser.Types;
using NoGoop.Win32;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Dialogs
{

	internal class TracingDialog : Dialog
	{
		protected TextBox           _level;

		internal TracingDialog()
		{
			Panel panel;
			Label l;

			Text = "Set Trace Level";
			
			panel = new Panel();
			panel.Dock = DockStyle.Top;
			Controls.Add(panel);

			_level = new NumericTextBox();
			_level.Dock = DockStyle.Left;
			_level.Width = 50;
			panel.Controls.Add(_level);
			l = new Label();
			l.Dock = DockStyle.Left;
			l.Text = "Trace Level";
			l.AutoSize = true;
			panel.Controls.Add(l);
		}

		public void DoShowDialog()
		{
			_level.Text = ((int)TraceUtil.Level).ToString();

			if (ShowDialog() != DialogResult.OK)
				return;

			try {
				TraceUtil.Level = (TraceLevel)Convert.ToInt32(_level.Text);
			} catch (Exception ex) {
				ErrorDialog.Show(ex, 
								 "Error setting trace level",
								 MessageBoxIcon.Error);
			}
		}
	}
}
