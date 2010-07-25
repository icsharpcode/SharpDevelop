// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Dialogs
{
	internal class AboutDialog : Form
	{
		protected LinkLabel _linkLabel;

		internal AboutDialog()
		{
			Label l;
			Text = StringParser.Parse("${res:ComponentInspector.AboutDialog.Title}");

			DockPadding.All = 10;
			StartPosition = FormStartPosition.CenterParent;
			MinimizeBox = false;
			MaximizeBox = false;
			FormBorderStyle = FormBorderStyle.FixedDialog;

			Panel bp = new Panel();
			bp.Height = Utils.BUTTON_HEIGHT;
			bp.Dock = DockStyle.Bottom;
			Controls.Add(bp);

			l = new Label();
			l.Dock = DockStyle.Fill;
			bp.Controls.Add(l);

			Button ok = Utils.MakeButton(StringParser.Parse("${res:Global.OKButtonText}"));
			ok.Dock = DockStyle.Right;
			ok.DialogResult = DialogResult.OK;
			bp.Controls.Add(ok);

			// In reverse order
			_linkLabel = new LinkLabel();
			_linkLabel.Dock = DockStyle.Top;
			_linkLabel.AutoSize = true;
			_linkLabel.Text = Constants.NOGOOP_URL;
			_linkLabel.Links.Add(0, Constants.NOGOOP_URL.Length, Constants.NOGOOP_URL);
			_linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked);
			Controls.Add(_linkLabel);

			// Spacing
			l = new Label();
			l.Dock = DockStyle.Top;
			l.Height = 5;
			Controls.Add(l);

			l = new Label();
			l.Dock = DockStyle.Top;
			l.AutoSize = true;
			l.Text = " © 2002-2006 oakland software inc. All Rights Reserved";
			Controls.Add(l);

			// Spacing
			l = new Label();
			l.Dock = DockStyle.Top;
			l.Height = 5;
			Controls.Add(l);

			l = new Label();
			l.Dock = DockStyle.Top;
			l.AutoSize = true;
			l.Text = StringParser.Parse("${res:ComponentInspector.AboutDialog.VersionLabel}") + " " + ObjectBrowser.CODEBASE_VERSION;
			Controls.Add(l);

			// Spacing
			l = new Label();
			l.Dock = DockStyle.Top;
			Controls.Add(l);

			l = new Label();
			l.Dock = DockStyle.Top;
			l.AutoSize = true;
			l.Font = new Font(l.Font, FontStyle.Bold);
			l.Text = Constants.NOGOOP + " " + ".NET Inspector"; //ObjectBrowser.License.ProductName;
			Controls.Add(l);
		}

		protected void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			// Determine which link was clicked within the LinkLabel.
			_linkLabel.Links[_linkLabel.Links.IndexOf(e.Link)].Visited = true;
			Utils.InvokeBrowser(e.Link.LinkData.ToString());
		}
	}
}
