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
			Window owner = window.Owner ?? WorkbenchSingleton.MainWindow;
			Point ownerPos = (owner == null ? new Point(0, 0) : new Point(owner.Left, owner.Top));
			if (isResizable) {
				Rect bounds = PropertyService.Get(propertyName, GetDefaultBounds(window));
				bounds.Offset(ownerPos.X, ownerPos.Y);
				bounds = Validate(bounds);
				window.Left = bounds.X;
				window.Top = bounds.Y;
				window.Width = bounds.Width;
				window.Height = bounds.Height;
			} else {
				Size size = new Size(window.ActualWidth, window.ActualHeight);
				Point location = PropertyService.Get(propertyName, GetDefaultLocation(window));
				location.Offset(ownerPos.X, ownerPos.Y);
				location = Validate(location, size);
				window.Left = location.X;
				window.Top = location.Y;
			}
			window.Closing += delegate {
				owner = window.Owner ?? WorkbenchSingleton.MainWindow;
				ownerPos = (owner == null ? new Point(0, 0) : new Point(owner.Left, owner.Top));
			    if (isResizable) {
					if (window.WindowState == System.Windows.WindowState.Normal) {
						PropertyService.Set(propertyName, new Rect(window.Left, window.Top, window.ActualWidth, window.ActualHeight));
					}
				} else {
					PropertyService.Set(propertyName, new Point(window.Left - ownerPos.X, window.Top - ownerPos.Y));
				                         
				}
			};
		}
		
		/*
		public static void ApplyWindow(Window window, string propertyName, bool isResizable)
		{
			window.WindowStartupLocation = WindowStartupLocation.Manual;
			if (isResizable) {
				Rect bounds = Validate(PropertyService.Get(propertyName, GetDefaultBounds(window)));
				window.Left = bounds.X;
				window.Top = bounds.Y;
				window.Width = bounds.Width;
				window.Height = bounds.Height;
			} else {
				Size size = new Size(window.ActualWidth, window.ActualHeight);
				Point location = Validate(PropertyService.Get(propertyName, GetDefaultLocation(window)), size);
				window.Left = location.X;
				window.Top = location.Y;
			}
			window.Closing += delegate {
				if (isResizable) {
					if (window.WindowState == System.Windows.WindowState.Normal) {
						PropertyService.Set(propertyName, new Rect(window.Left, window.Top, window.ActualWidth, window.ActualHeight));
					}
				} else {
					PropertyService.Set(propertyName, new Point(window.Left, window.Top));
				}
			};
		}
		*/
		
		public static Rect Validate(Rect bounds)
		{
			// Check if form is outside the screen and get it back if necessary.
			// This is important when the user uses multiple screens, a window stores its location
			// on the secondary monitor and then the secondary monitor is removed.
			Rect screen1 = Screen.FromPoint(bounds.TopLeft.ToSystemDrawing()).WorkingArea.ToWpf();
			Rect screen2 = Screen.FromPoint(bounds.TopRight.ToSystemDrawing()).WorkingArea.ToWpf();
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
