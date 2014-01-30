// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.WinForms;

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
		public ToolNotFoundDialog(string description, string linkTarget, IImage icon = null)
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			
			descriptionLabel.Text = description;
			linkLabel.Text = linkTarget;
			pictureBox.Image = (icon != null ? icon.Bitmap : SD.ResourceService.GetBitmap("Icons.32x32.Information"));
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
