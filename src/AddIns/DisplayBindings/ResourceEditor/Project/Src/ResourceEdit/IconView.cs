// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;

namespace ResourceEditor
{
	/// <summary>
	/// This control is used for displaying images. Large images
	/// can be scrolled.
	/// </summary>
	class IconView : AbstractImageView
	{
		ResourceItem resourceItem;
		
		public IconView(ResourceItem item) : base(item)
		{
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
				pictureBox.Image = ((Icon)value.ResourceValue).ToBitmap();
				adjustMargin();
			}
		}
	}
}
