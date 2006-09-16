// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using NoGoop.Win32;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Panels
{

	internal class ButtonPanel : System.Windows.Forms.Panel
	{

		internal const bool         CANCEL = true;

		internal ButtonPanel(Form parent) : this(parent, CANCEL)
		{
		}

		internal ButtonPanel(Form parent, bool includeCancel)
		{

			Label l = new Label();
			l.Dock = DockStyle.Fill;
			Controls.Add(l);

			Button ok = Utils.MakeButton(StringParser.Parse("${res:Global.OKButtonText}"));
			ok.Dock = DockStyle.Right;
			ok.DialogResult = DialogResult.OK;
			parent.AcceptButton = ok;
			Controls.Add(ok);

			if (includeCancel)
			{
				l = new Label();
				l.Dock = DockStyle.Right;
				l.Width = 5;
				Controls.Add(l);

				Button cancel = Utils.MakeButton(StringParser.Parse("${res:Global.CancelButtonText}"));
				cancel.Dock = DockStyle.Right;
				cancel.DialogResult = DialogResult.Cancel;
				Controls.Add(cancel);
			}

			Height = Utils.BUTTON_HEIGHT;
		}
	}
}
