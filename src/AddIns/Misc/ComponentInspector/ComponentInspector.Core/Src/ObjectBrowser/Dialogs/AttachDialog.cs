// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using NoGoop.Controls;
using NoGoop.Debug;

namespace NoGoop.ObjBrowser.Dialogs
{

	internal class AttachDialog : Dialog
	{
		protected TextBox           _process;

		internal AttachDialog()
		{
			Panel panel;
			Label l;

			Text = StringParser.Parse("${res:ComponentInspector.AttachDialog.Title}");
			
			panel = new Panel();
			panel.Dock = DockStyle.Top;
			Controls.Add(panel);

			_process = new NumericTextBox();
			_process.Dock = DockStyle.Left;
			_process.Width = 50;
			panel.Controls.Add(_process);
			l = new Label();
			l.Dock = DockStyle.Left;
			l.Text = StringParser.Parse("${res:ComponentInspector.AttachDialog.ProcessLabel}");
			l.AutoSize = true;
			panel.Controls.Add(l);

			ObjectBrowser.Debugger = new Debugger();
		}

		public void DoShowDialog()
		{
			if (ShowDialog() != DialogResult.OK)
				return;

			uint processId = Convert.ToUInt32(_process.Text);

			try {
				ObjectBrowser.Debugger.Attach(processId);
			} catch (Exception ex) {
				ErrorDialog.Show(ex, 
				                 String.Format(StringParser.Parse("${res:ComponentInspector.AttachDialog.AttachFailedMessage}"), processId),
								 MessageBoxIcon.Error);
			}
		}
	}
}
