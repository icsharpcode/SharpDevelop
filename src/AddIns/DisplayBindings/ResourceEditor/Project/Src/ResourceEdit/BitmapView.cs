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
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ResourceEditor
{
	/// <summary>
	/// This control is used for displaying images. Large images
	/// can be scrolled.
	/// </summary>
	class BitmapView : AbstractImageView
	{
		ResourceItem resourceItem;
		LinkLabel updateLinkLabel;
		
		public BitmapView(ResourceItem item) : base(item)
		{
			
			updateLinkLabel = new LinkLabel();
			updateLinkLabel.Text = ResourceService.GetString("ResourceEditor.BitmapView.UpdateBitmap");
			updateLinkLabel.Location = new Point(4, 4);
			updateLinkLabel.AutoSize = true;
			updateLinkLabel.Click += new EventHandler(updateBitmapLinkLabelClick);
			Controls.Add(updateLinkLabel);
		}
		
		void updateBitmapLinkLabelClick(object sender, EventArgs e)
		{
			using(OpenFileDialog fileDialog = new OpenFileDialog())
			{
				Bitmap bitmap;
				fileDialog.AddExtension = true;
				fileDialog.Filter = "All files (*.*)|*.*";
				fileDialog.CheckFileExists = true;
				
				if(fileDialog.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					try {
						bitmap = new Bitmap(fileDialog.FileName);
					} catch {
						
						MessageService.ShowWarning("Can't load bitmap file.");
						return;
					}
					ResourceItem = new ResourceItem(resourceItem.Name, bitmap);
					OnResourceChanged(resourceItem.Name, bitmap);
				}
			}
		}
		
		public override bool WriteProtected
		{
			get {
				return true;
			}
			set {
			}
		}
		
		public override ResourceItem ResourceItem
		{
			get {
				return resourceItem;
			}
			set {
				resourceItem = value;
				pictureBox.Image = (Bitmap)value.ResourceValue;
				adjustMargin();
			}
		}
	}
}
