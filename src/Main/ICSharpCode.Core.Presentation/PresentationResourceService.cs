// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Creates WPF BitmapSource objects from images in the ResourceService.
	/// </summary>
	public static class PresentationResourceService
	{
		static readonly Dictionary<string, BitmapSource> bitmapCache = new Dictionary<string, BitmapSource>();
		static readonly IResourceService resourceService;
		
		static PresentationResourceService()
		{
			resourceService = ServiceSingleton.GetRequiredService<IResourceService>();
			resourceService.LanguageChanged += OnLanguageChanged;
		}
		
		static void OnLanguageChanged(object sender, EventArgs e)
		{
			lock (bitmapCache) {
				bitmapCache.Clear();
			}
		}
		
		/// <summary>
		/// Creates a new System.Windows.Controls.Image object containing the image with the
		/// specified resource name.
		/// </summary>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		[Obsolete("Use SD.ResourceService.GetImage(name).CreateImage() instead, or just create the image manually")]
		public static System.Windows.Controls.Image GetImage(string name)
		{
			return new System.Windows.Controls.Image {
				Source = GetBitmapSource(name)
			};
		}
		
		/// <summary>
		/// Returns a BitmapSource from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public static BitmapSource GetBitmapSource(string name)
		{
			if (resourceService == null)
				throw new ArgumentNullException("resourceService");
			lock (bitmapCache) {
				BitmapSource bs;
				if (bitmapCache.TryGetValue(name, out bs))
					return bs;
				System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)resourceService.GetImageResource(name);
				if (bmp == null) {
					throw new ResourceNotFoundException(name);
				}
				IntPtr hBitmap = bmp.GetHbitmap();
				try {
					bs = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
					                                           Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					bs.Freeze();
					bitmapCache[name] = bs;
				} finally {
					NativeMethods.DeleteObject(hBitmap);
				}
				return bs;
			}
		}
	}
}
