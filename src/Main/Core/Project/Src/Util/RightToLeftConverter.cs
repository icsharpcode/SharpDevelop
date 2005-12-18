// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of RightToLeftConverter.
	/// </summary>
	public static class RightToLeftConverter
	{
		public static RightToLeft RightToLeft {
			get {
				return RightToLeft.Inherit;
			}
		}
		
		static AnchorStyles Convert(AnchorStyles anchor)
		{
			bool right = (anchor & AnchorStyles.Right) == AnchorStyles.Right;
			bool left  = (anchor & AnchorStyles.Left ) == AnchorStyles.Left ;
			if (right) {
				anchor = anchor | AnchorStyles.Left;
			} else {
				anchor = anchor & ~AnchorStyles.Left;
			}
			if (left) {
				anchor = anchor | AnchorStyles.Right;
			} else {
				anchor = anchor & ~AnchorStyles.Right;
			}
			return anchor;
		}
		
		static Point ConvertLocation(Control control)
		{
			return new Point(control.Parent.Size.Width -  control.Location.X - control.Size.Width, 
			                 control.Location.Y);
		}
		
		static DockStyle Convert(DockStyle dock)
		{
			switch (dock) {
				case DockStyle.Left:
					return DockStyle.Right;
				case DockStyle.Right:
					return DockStyle.Left;
			}
			return dock;
		}
		
		public static void Convert(Control control)
		{
//			control.RightToLeft = RightToLeft.Yes;
//			control.Anchor = Convert(control.Anchor);
//			control.Dock = Convert(control.Dock);
//			if (control.Parent != null) {
//				control.Location = ConvertLocation(control);
//			}
//			
//			foreach (Control c in control.Controls) {
//				Convert(c);
//			}
		}
	}
}
