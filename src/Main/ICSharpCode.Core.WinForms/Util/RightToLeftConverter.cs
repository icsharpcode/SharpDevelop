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

namespace ICSharpCode.Core.WinForms
{
	/// <summary>
	/// Allows converting forms to right-to-left layout.
	/// </summary>
	public static class RightToLeftConverter
	{
		public static bool IsRightToLeft { get; set; }
		
		static AnchorStyles Mirror(AnchorStyles anchor)
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
		
		static Point MirrorLocation(Control control)
		{
			return new Point(control.Parent.ClientSize.Width - control.Left - control.Width,
			                 control.Top);
		}
		
		/// <summary>
		/// Mirrors a control and its child controls if right to left is activated.
		/// Call this only for controls that aren't mirrored automatically by .NET!
		/// </summary>
		static void Mirror(Control control)
		{
			if (!(control.Parent is SplitContainer)) {
				switch (control.Dock) {
					case DockStyle.Left:
						control.Dock = DockStyle.Right;
						break;
					case DockStyle.Right:
						control.Dock = DockStyle.Left;
						break;
					case DockStyle.None:
						control.Anchor = Mirror(control.Anchor);
						control.Location = MirrorLocation(control);
						break;
				}
			}
			// Panels with RightToLeft = No won't have their children mirrored
			if (control.RightToLeft != RightToLeft.Yes)
				return;
			foreach (Control child in control.Controls) {
				Mirror(child);
			}
		}
		
		public static void Convert(Control control)
		{
			bool isRTL = IsRightToLeft;
			if (isRTL) {
				if (control.RightToLeft != RightToLeft.Yes)
					control.RightToLeft = RightToLeft.Yes;
			} else {
				if (control.RightToLeft == RightToLeft.Yes)
					control.RightToLeft = RightToLeft.No;
			}
			ConvertLayout(control);
		}
		
		static void ConvertLayout(Control control)
		{
			bool isRTL = IsRightToLeft;
			
			DateTimePicker picker = control as DateTimePicker;
			Form form = control as Form;
			ListView listView = control as ListView;
			ProgressBar pg = control as ProgressBar;
			TabControl tc = control as TabControl;
			TrackBar trb = control as TrackBar;
			TreeView treeView = control as TreeView;
			if (form != null && form.RightToLeftLayout != isRTL)
				form.RightToLeftLayout = isRTL;
			if (listView != null && listView.RightToLeftLayout != isRTL)
				listView.RightToLeftLayout = isRTL;
			if (pg != null && pg.RightToLeftLayout != isRTL)
				pg.RightToLeftLayout = isRTL;
			if (tc != null && tc.RightToLeftLayout != isRTL)
				tc.RightToLeftLayout = isRTL;
			if (trb != null && trb.RightToLeftLayout != isRTL)
				trb.RightToLeftLayout = isRTL;
			if (treeView != null && treeView.RightToLeftLayout != isRTL)
				treeView.RightToLeftLayout = isRTL;
		}
		
		static void ConvertLayoutRecursive(Control control)
		{
			bool isRTL = IsRightToLeft;
			if (isRTL == (control.RightToLeft == RightToLeft.Yes)) {
				ConvertLayout(control);
				foreach (Control child in control.Controls) {
					ConvertLayoutRecursive(child);
				}
			}
		}
		
		public static void ConvertRecursive(Control control)
		{
			if (IsRightToLeft == (control.RightToLeft == RightToLeft.Yes)) {
				// already converted
				return;
			}
			ReConvertRecursive(control);
		}
		
		public static void ReConvertRecursive(Control control)
		{
			Convert(control);
			foreach (Control child in control.Controls) {
				ConvertLayoutRecursive(child);
			}
			if (IsRightToLeft) {
				if (control is Form) {
					// direct children seem to be mirrored by .NET
					foreach (Control child in control.Controls) {
						foreach (Control subChild in child.Controls) {
							Mirror(subChild);
						}
					}
				} else {
					foreach (Control child in control.Controls) {
						Mirror(child);
					}
				}
			}
		}
	}
}
