// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

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
				
				if(fileDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
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
