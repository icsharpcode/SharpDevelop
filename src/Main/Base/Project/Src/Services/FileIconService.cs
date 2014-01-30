// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This Class handles bitmap resources
	/// for the application which were loaded from the filesystem.
	/// </summary>
	public static class FileIconService
	{
		static Dictionary<string, Bitmap> bitmapCache = new Dictionary<string, Bitmap>();
		
		/// <summary>
		/// Returns a bitmap from the file system. Placeholders like ${SharpDevelopBinPath}
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
					string fileName = StringParser.Parse(name.Substring(5, name.Length - 5));
					bmp = (Bitmap)Image.FromFile(fileName);
					bitmapCache[name] = bmp;
				}
			}
			return bmp;
		}
	}
}
