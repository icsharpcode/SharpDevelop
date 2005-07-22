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
