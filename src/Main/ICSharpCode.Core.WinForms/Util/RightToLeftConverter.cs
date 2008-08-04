// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	/// <summary>
	/// Description of RightToLeftConverter.
	/// </summary>
	public static class RightToLeftConverter
	{
		public static string[] RightToLeftLanguages = new string[] {"ar", "he", "fa", "urdu"};
		
		public static bool IsRightToLeft {
			get {
				foreach (string language in RightToLeftLanguages) {
					if (ResourceService.Language.StartsWith(language))
						return true;
				}
				return false;
			}
		}
		
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
