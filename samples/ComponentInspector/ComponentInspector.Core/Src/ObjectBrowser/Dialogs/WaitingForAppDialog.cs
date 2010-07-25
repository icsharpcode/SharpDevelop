// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Dialogs
{
	internal class WaitingForAppDialog : Dialog
	{
		protected RichTextBox           _textBox;

		internal WaitingForAppDialog() : base(!INCLUDE_BUTTONS)
		{
			Text = StringParser.Parse("${res:ComponentInspector.WaitingForAppDialog.Title}");
			Height = 150;

			String descText = StringParser.Parse("${res:ComponentInspector.WaitingForAppDialog.Information}");

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

			Button cancel = Utils.MakeButton(StringParser.Parse("${res:Global.CancelButtonText}"));
			cancel.Dock = DockStyle.Right;
			cancel.DialogResult = DialogResult.Cancel;
			bottomPanel.Controls.Add(cancel);

			bottomPanel.Height = Utils.BUTTON_HEIGHT;
			Controls.Add(bottomPanel);
		}

	}
}
