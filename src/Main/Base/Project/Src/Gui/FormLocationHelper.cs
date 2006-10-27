// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Static helper class that loads and stores the position and size of a Form in the
	/// PropertyService.
	/// </summary>
	public static class FormLocationHelper
	{
		public static void Apply(Form form, string propertyName, bool isResizable)
		{
			form.StartPosition = FormStartPosition.Manual;
			if (isResizable) {
				form.Bounds = Validate(PropertyService.Get(propertyName, GetDefaultBounds(form)));
			} else {
				form.Location = Validate(PropertyService.Get(propertyName, GetDefaultLocation(form)), form.Size);
			}
			form.Closing += delegate {
				if (isResizable) {
					if (form.WindowState == FormWindowState.Normal) {
						PropertyService.Set(propertyName, form.Bounds);
					}
				} else {
					PropertyService.Set(propertyName, form.Location);
				}
			};
		}
		
		static Rectangle Validate(Rectangle bounds)
		{
			// Check if form is outside the screen and get it back if necessary.
			// This is important when the user uses multiple screens, a window stores its location
			// on the secondary monitor and then the secondary monitor is removed.
			Rectangle screen1 = Screen.FromPoint(new Point(bounds.X, bounds.Y)).WorkingArea;
			Rectangle screen2 = Screen.FromPoint(new Point(bounds.X + bounds.Width, bounds.Y)).WorkingArea;
			if (bounds.Y < screen1.Y - 5 && bounds.Y < screen2.Y - 5)
				bounds.Y = screen1.Y - 5;
			if (bounds.X < screen1.X - bounds.Width / 2)
				bounds.X = screen1.X - bounds.Width / 2;
			else if (bounds.X > screen2.Right - bounds.Width / 2)
				bounds.X = screen2.Right - bounds.Width / 2;
			return bounds;
		}
		
		static Point Validate(Point location, Size size)
		{
			return Validate(new Rectangle(location, size)).Location;
		}
		
		static Rectangle GetDefaultBounds(Form form)
		{
			return new Rectangle(GetDefaultLocation(form), form.Size);
		}
		
		static Point GetDefaultLocation(Form form)
		{
			Rectangle parent = WorkbenchSingleton.MainForm.Bounds;
			Size size = form.Size;
			return new Point(parent.Left + (parent.Width - size.Width) / 2,
			                 parent.Top + (parent.Height - size.Height) / 2);
		}
	}
}
