// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.SharpDevelop.WinForms
{
	public static class WinFormsExtensions
	{
		/// <summary>
		/// Gets a bitmap from the resource service.
		/// This method returns an existing bitmap, do not dispose it!
		/// </summary>
		/// <exception cref="ResourceNotFoundException">Resource with the specified name does not exist</exception>
		public static Bitmap GetBitmap(this IResourceService resourceService, string resourceName)
		{
			return SD.WinForms.GetResourceServiceBitmap(resourceName);
		}
		
		/// <summary>
		/// Gets an icon from the resource service.
		/// This method returns an existing icon, do not dispose it!
		/// </summary>
		/// <exception cref="ResourceNotFoundException">Resource with the specified name does not exist</exception>
		public static Icon GetIcon(this IResourceService resourceService, string resourceName)
		{
			return SD.WinForms.GetResourceServiceIcon(resourceName);
		}
		
		/// <summary>
		/// Recursively gets all WinForms controls.
		/// </summary>
		public static IEnumerable<Control> GetRecursive(this Control.ControlCollection collection)
		{
			return collection.Cast<Control>().Flatten(c => c.Controls.Cast<Control>());
		}
		
		#region System.Drawing <-> WPF conversions
		public static System.Drawing.Point ToSystemDrawing(this System.Windows.Point p)
		{
			return new System.Drawing.Point((int)p.X, (int)p.Y);
		}
		
		public static System.Drawing.Size ToSystemDrawing(this System.Windows.Size s)
		{
			return new System.Drawing.Size((int)s.Width, (int)s.Height);
		}
		
		public static System.Drawing.Rectangle ToSystemDrawing(this System.Windows.Rect r)
		{
			return new System.Drawing.Rectangle(r.TopLeft.ToSystemDrawing(), r.Size.ToSystemDrawing());
		}
		
		public static System.Drawing.Color ToSystemDrawing(this System.Windows.Media.Color c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}
		
		public static System.Windows.Point ToWpf(this System.Drawing.Point p)
		{
			return new System.Windows.Point(p.X, p.Y);
		}
		
		public static System.Windows.Size ToWpf(this System.Drawing.Size s)
		{
			return new System.Windows.Size(s.Width, s.Height);
		}
		
		public static System.Windows.Rect ToWpf(this System.Drawing.Rectangle rect)
		{
			return new System.Windows.Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
		}
		
		public static System.Windows.Media.Color ToWpf(this System.Drawing.Color c)
		{
			return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
		}
		#endregion
	}
}
