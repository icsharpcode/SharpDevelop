// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
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
			lock (bitmapCache) {
				BitmapSource bs;
				if (bitmapCache.TryGetValue(name, out bs))
					return bs;
				System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)ResourceService.GetImageResource(name);
				if (bmp == null) {
					throw new ResourceNotFoundException(name);
				}
				bs = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero,
				                                           Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				bs.Freeze();
				bitmapCache[name] = bs;
				return bs;
			}
		}
	}
}
