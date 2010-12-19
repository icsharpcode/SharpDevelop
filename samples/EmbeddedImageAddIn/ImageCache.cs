// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;

namespace EmbeddedImageAddIn
{
	public static class ImageCache
	{
		static readonly Dictionary<FileName, WeakReference> imageCache = new Dictionary<FileName, WeakReference>();
		
		public static ImageSource GetImage(FileName fileName)
		{
			lock (imageCache) {
				WeakReference wr;
				ImageSource image;
				// retrieve image from cache, if possible
				if (imageCache.TryGetValue(fileName, out wr)) {
					image = (ImageSource)wr.Target;
					if (image != null)
						return image;
				}
				// load image:
				image = LoadBitmap(fileName);
				if (image != null)
					imageCache[fileName] = new WeakReference(image);
				// clean up cache:
				List<FileName> entriesToRemove = (from p in imageCache where !p.Value.IsAlive select p.Key).ToList();
				foreach (var entry in entriesToRemove)
					imageCache.Remove(entry);
				return image;
			}
		}
		
		static BitmapImage LoadBitmap(string fileName)
		{
			try {
				if (File.Exists(fileName)) {
					BitmapImage bitmap = new BitmapImage(new Uri(fileName));
					bitmap.Freeze();
					return bitmap;
				}
			} catch (ArgumentException) {
				// invalid filename syntax
			} catch (IOException) {
				// other IO error
			}
			return null;
		}
	}
}
