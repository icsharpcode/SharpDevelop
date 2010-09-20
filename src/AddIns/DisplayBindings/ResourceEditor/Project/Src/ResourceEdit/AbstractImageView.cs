// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ResourceEditor
{
	/// <summary>
	/// This control is used for displaying images. Large images
	/// can be scrolled.
	/// </summary>
	abstract class AbstractImageView : ScrollableControl, IResourceView
	{
		protected PictureBox pictureBox;
		
		public event ResourceChangedEventHandler ResourceChanged;
		
		public abstract ResourceItem ResourceItem
		{
			get;
			set;
		}
		
		public abstract bool WriteProtected
		{
			get;
			set;
		}
		
		protected void resized(object sender, EventArgs e)
		{
			adjustMargin();
		}
		
		protected AbstractImageView(ResourceItem item)
		{
			Dock = DockStyle.Fill;
			AutoScroll = true;
			pictureBox = new PictureBox();
			pictureBox.BorderStyle = BorderStyle.FixedSingle;
			this.SizeChanged += new EventHandler(resized);
			pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
			Controls.Add(this.pictureBox);
			ResourceItem = item;
		}
		
		protected void OnResourceChanged(string resourceName, object val)
		{
			if(ResourceChanged != null) {
				ResourceChanged(this, new ResourceEventArgs(resourceName, val));
			}
		}
		
		protected void adjustMargin()
		{
			int deltaY = Height - pictureBox.Image.Height;
			int deltaX = Width - pictureBox.Image.Width;
			
			if(deltaY > 0) {
				pictureBox.Top = deltaY / 2;
			}
			pictureBox.Top = Math.Max(pictureBox.Top, 20);
			
			if(deltaX > 0) {
				pictureBox.Left = deltaX / 2;
			}
			pictureBox.Left = Math.Max(pictureBox.Left, 20);
			AutoScrollMargin = new Size(pictureBox.Left / 2, pictureBox.Top / 2);
		}
	}
}
