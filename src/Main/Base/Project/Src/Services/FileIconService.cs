// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This Class contains a ResourceManager, which handles bitmap resources
	/// for the application which were loaded from the filesystem.
	/// </summary>
	public static class FileIconService
	{
		static Dictionary<string, Bitmap> bitmapCache = new Dictionary<string, Bitmap>();
		
		static FileIconService()
		{
			ResourceService.ClearCaches += ResourceService_ClearCaches;
		}
		
		static void ResourceService_ClearCaches(object sender, EventArgs e)
		{
			lock (bitmapCache) {
				bitmapCache.Clear();
			}
		}
		
		/// <summary>
		/// Returns a bitmap from the file system. Paceholders like ${SharpDevelopBinPath}
		/// and AddinPath (e. g. ${AddInPath:ICSharpCode.FiletypeRegisterer}) are resolved 
		/// through the StringParser.
		/// The bitmaps are reused, you must not dispose the Bitmap!
		/// </summary>
		/// <returns>
		/// The bitmap loaded from the file system.
		/// </returns>
		/// <param name="name">
		/// The name of the requested bitmap (prefix, path and filename).
		/// </param>
		/// <exception cref="FileNotFoundException">
		/// Is thrown when the bitmap was not found in the file system.
		/// </exception>
		public static Bitmap GetBitmap(string name)
		{
			Bitmap bmp = null;
            if (name.ToUpper().StartsWith("FILE:")) {
				lock (bitmapCache) {
					if (bitmapCache.TryGetValue(name, out bmp))
						return bmp;
                    bmp = (Bitmap)Image.FromFile(StringParser.Parse(name.Substring(5,name.Length-5)));
					bitmapCache[name] = bmp;
				}
			}
			return bmp;
		}
	}
}

