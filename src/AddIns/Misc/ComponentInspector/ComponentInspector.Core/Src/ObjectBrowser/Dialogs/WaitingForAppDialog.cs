// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

using NoGoop.Win32;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Dialogs
{
	internal class WaitingForAppDialog : Dialog
	{
		protected RichTextBox           _textBox;

		internal WaitingForAppDialog() : base(!INCLUDE_BUTTONS)
		{
			Text = "Waiting for Application to Start";
			Height = 150;

			String descText;


			descText = 
				"Waiting for application to start. "
				+ "If the application fails to start (or you "
				+ "become tired of waiting), "
				+ "you may cancel by pressing Cancel.";

			_textBox = Utils.MakeDescText(descText, this);
			_textBox.Dock = DockStyle.Fill;
			Controls.Add(_textBox);

			Label l = new Label();
			l.Dock = DockStyle.Fill;
			Controls.Add(l);

			Panel bottomPanel = new Panel();
			bottomPanel.Dock = DockStyle.Bottom;

			l = new Label();
			l.Dock = DockStyle.Fill;
			bottomPanel.Controls.Add(l);

			Button cancel = Utils.MakeButton("Cancel");
			cancel.Dock = DockStyle.Right;
			cancel.DialogResult = DialogResult.Cancel;
			bottomPanel.Controls.Add(cancel);

			bottomPanel.Height = Utils.BUTTON_HEIGHT;
			Controls.Add(bottomPanel);
		}

	}
}
