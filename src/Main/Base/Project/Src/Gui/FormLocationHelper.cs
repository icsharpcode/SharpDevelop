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
				form.Location = Validate(new System.Drawing.Rectangle(PropertyService.Get(propertyName, GetDefaultLocation(form)), form.Size)).Location;
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
				Rect bounds = PropertyService.Get(propertyName, GetDefaultBounds(window));
				bounds.Offset(ownerLocation.X, ownerLocation.Y);
				window.SourceInitialized += delegate {
					bounds = Validate(bounds.TransformToDevice(window).ToSystemDrawing())
						.ToWpf().TransformFromDevice(window);
					window.Left = bounds.X;
					window.Top = bounds.Y;
					window.Width = bounds.Width;
					window.Height = bounds.Height;
				};
			} else {
				window.SourceInitialized += delegate {
					Size size = new Size(window.ActualWidth, window.ActualHeight);
					Point location = PropertyService.Get(propertyName, GetDefaultLocation(window));
					location.Offset(ownerLocation.X, ownerLocation.Y);
					var bounds = Validate(new Rect(location, size).TransformToDevice(window).ToSystemDrawing())
						.ToWpf().TransformFromDevice(window);
					window.Left = bounds.X;
					window.Top = bounds.Y;
				};
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
		
		/// <remarks>Requires Pixels!!!</remarks>
		public static System.Drawing.Rectangle Validate(System.Drawing.Rectangle bounds)
		{
			// Check if form is outside the screen and get it back if necessary.
			// This is important when the user uses multiple screens, a window stores its location
			// on the secondary monitor and then the secondary monitor is removed.
			LoggingService.InfoFormatted("Number of screens: {0}", Screen.AllScreens.Length);
			
			foreach (var screen in Screen.AllScreens) {
				var rect = System.Drawing.Rectangle.Intersect(bounds, screen.WorkingArea);
				LoggingService.InfoFormatted("Screen {2}: Validating {0}; intersection {1}", bounds, rect, screen.Bounds);
				if (rect.Width > 10 && rect.Height > 10)
					return bounds;
			}
			// center on primary screen
			LoggingService.InfoFormatted("Validating {0}; center on screen", bounds);
			// TODO : maybe use screen where main window is most visible?
			var targetScreen = Screen.PrimaryScreen;
			return new System.Drawing.Rectangle((targetScreen.WorkingArea.Width - bounds.Width) / 2, (targetScreen.WorkingArea.Height - bounds.Height) / 2, bounds.Width, bounds.Height);
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
