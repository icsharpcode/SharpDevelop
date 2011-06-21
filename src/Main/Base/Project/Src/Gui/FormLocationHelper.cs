// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
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
		
		public static void ApplyWindow(Window window, string propertyName, bool isResizable)
		{
			window.WindowStartupLocation = WindowStartupLocation.Manual;
			var ownerLocation = GetOwnerLocation(window);
			if (isResizable) {
				Rect bounds = Validate(PropertyService.Get(propertyName, GetDefaultBounds(window)));
				bounds.Offset(ownerLocation.X, ownerLocation.Y);
				window.Left = bounds.X;
				window.Top = bounds.Y;
				window.Width = bounds.Width;
				window.Height = bounds.Height;
			} else {
				Size size = new Size(window.ActualWidth, window.ActualHeight);
				Point location = Validate(PropertyService.Get(propertyName, GetDefaultLocation(window)), size);
				location.Offset(ownerLocation.X, ownerLocation.Y);
				window.Left = location.X;
				window.Top = location.Y;
			}
			window.Closing += delegate {
				var relativeToOwner = GetLocationRelativeToOwner(window);
				if (isResizable) {
					if (window.WindowState == System.Windows.WindowState.Normal) {
						PropertyService.Set(propertyName, new Rect(relativeToOwner, new Size(window.ActualWidth, window.ActualHeight)));
					}
				} else {
					PropertyService.Set(propertyName, relativeToOwner);
				}
			};
		}
		
		static Point GetLocationRelativeToOwner(Window window)
		{
			Point ownerLocation = GetOwnerLocation(window);
			// TODO : do we need special support for maximized child windows?
			// window.Left / window.Top do not return the proper values if the window is maximized
			return new Point(window.Left - ownerLocation.X, window.Top - ownerLocation.Y);
		}
		
		static Point GetOwnerLocation(Window window)
		{
			var owner = window.Owner ?? WorkbenchSingleton.MainWindow;
			if (owner == null)
				return new Point(0,0);
			if (owner.WindowState == System.Windows.WindowState.Maximized) {
				// find screen on which owner window is maximized
				// owner.Left / owner.Top do not return the proper values if the window is maximized
				var screen = Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(owner).Handle);
				return screen.WorkingArea.Location.ToWpf();
			}
			return new Point(owner.Left, owner.Top);
		}
		
		public static Rect Validate(Rect bounds)
		{
			// Check if form is outside the screen and get it back if necessary.
			// This is important when the user uses multiple screens, a window stores its location
			// on the secondary monitor and then the secondary monitor is removed.
			foreach (var screen in Screen.AllScreens) {
				var rect = System.Drawing.Rectangle.Intersect(bounds.ToSystemDrawing(), screen.WorkingArea);
				if (rect.Width > 10 && rect.Height > 10)
					return bounds;
			}
			// center on primary screen
			// TODO : maybe use screen where main window is most visible?
			var targetScreen = Screen.PrimaryScreen;
			return new Rect((targetScreen.WorkingArea.Width - bounds.Width) / 2, (targetScreen.WorkingArea.Height - bounds.Height) / 2, bounds.Width, bounds.Height);
		}
		
		static Point Validate(Point location, Size size)
		{
			return Validate(new Rect(location, size)).Location;
		}
		
		static System.Drawing.Rectangle Validate(System.Drawing.Rectangle bounds)
		{
			return Validate(bounds.ToWpf()).ToSystemDrawing();
		}
		
		static System.Drawing.Point Validate(System.Drawing.Point location, System.Drawing.Size size)
		{
			return Validate(location.ToWpf(), size.ToWpf()).ToSystemDrawing();
		}
		
		static System.Drawing.Rectangle GetDefaultBounds(Form form)
		{
			return new System.Drawing.Rectangle(GetDefaultLocation(form.Size.ToWpf()).ToSystemDrawing(), form.Size);
		}
		
		static Rect GetDefaultBounds(Window window)
		{
			Size size = new Size(window.Width, window.Height);
			return new Rect(GetDefaultLocation(size), size);
		}
		
		static System.Drawing.Point GetDefaultLocation(Form form)
		{
			return GetDefaultLocation(form.Size.ToWpf()).ToSystemDrawing();
		}
		
		static Point GetDefaultLocation(Window window)
		{
			Size size = new Size(window.Width, window.Height);
			return GetDefaultLocation(size);
		}
		
		static Point GetDefaultLocation(Size formSize)
		{
			var mainWindow = WorkbenchSingleton.MainWindow;
			Rect parent = new Rect(
				mainWindow.Left, mainWindow.Top, mainWindow.Width, mainWindow.Height
			);
			return new Point(parent.Left + (parent.Width - formSize.Width) / 2,
			                 parent.Top + (parent.Height - formSize.Height) / 2);
		}
	}
}
