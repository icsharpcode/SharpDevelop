// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;

namespace ICSharpCode.SharpDevelop.Widgets.Resources
{
	/// <summary>
	/// Manages the bitmap resources used by the widgets library.
	/// These resources are kept outside of the SharpDevelop core
	/// so that this library can be used independently.
	/// </summary>
	public static class BitmapResources
	{
		static readonly Dictionary<string, Bitmap> bitmapCache = new Dictionary<string, Bitmap>();
		
		/// <summary>
		/// Gets a bitmap from the embedded bitmap resources.
		/// </summary>
		/// <param name="name">The name of the bitmap to get.</param>
		/// <returns>The Bitmap.</returns>
		public static Bitmap GetBitmap(string name)
		{
			lock(bitmapCache) {
				Bitmap bmp;
				
				if (bitmapCache.TryGetValue(name, out bmp)) {
					return bmp;
				}
				
				bmp = new Bitmap(typeof(BitmapResources).Assembly.GetManifestResourceStream(typeof(BitmapResources), name));
				bitmapCache[name] = bmp;
				return bmp;
			}
		}
	}
}
