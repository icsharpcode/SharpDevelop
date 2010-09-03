// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class ToolNotFoundDialog : Form
	{
		/// <summary>
		/// Creates a new instance of the ToolNotFoundDialog.
		/// </summary>
		/// <param name="description">The description text</param>
		/// <param name="linkTarget">The link target (with leading http://)</param>
		/// <param name="icon">32x32 icon to display next to the description. May be null.</param>
		public ToolNotFoundDialog(string description, string linkTarget, Image icon = null)
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			
			descriptionLabel.Text = description;
			linkLabel.Text = linkTarget;
			pictureBox.Image = icon ?? WinFormsResourceService.GetBitmap("Icons.32x32.Information");
			this.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.ToolNotFoundDialog.Title}");
			okButton.Text = StringParser.Parse("${res:Global.OKButtonText}");
		}
		
		void LinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try {
				Process.Start(linkLabel.Text);
			} catch (Exception ex) {
				LoggingService.Warn("Cannot start " + linkLabel.Text, ex);
			}
		}
	}
}
