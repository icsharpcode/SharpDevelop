// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.WinForms;

namespace ICSharpCode.SharpDevelop
{
	public class ImageSourceImage : IImage
	{
		readonly ImageSource imageSource;
		Bitmap bitmap;
		Icon icon;
		
		public ImageSourceImage(ImageSource imageSource)
		{
			if (imageSource == null)
				throw new ArgumentNullException("imageSource");
			this.imageSource = imageSource;
		}
		
		public ImageSource ImageSource {
			get { return imageSource; }
		}
		
		public Bitmap Bitmap {
			get {
				return LazyInitializer.EnsureInitialized(ref bitmap, () => GetBitmap(imageSource));
			}
		}
		
		static Bitmap GetBitmap(ImageSource imageSource)
		{
			if (imageSource is BitmapSource)
				return GetBitmap((BitmapSource)imageSource);
			else if (imageSource is DrawingImage)
				return GetBitmap((DrawingImage)imageSource);
			else
				throw new NotSupportedException();
		}
		
		static Bitmap GetBitmap(BitmapSource bitmapSource)
		{
			var format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
			Bitmap bmp = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, format);
			BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, format);
			bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
			bmp.UnlockBits(data);
			return bmp;
		}
		
		static Bitmap GetBitmap(DrawingImage drawingImage)
		{
			var drawingGroup = drawingImage.Drawing as DrawingGroup;
			if (drawingGroup == null)
				throw new NotSupportedException();
			Rect imageRect = Rect.Empty;
			foreach (var child in drawingGroup.Children) {
				var childImage = child as ImageDrawing;
				if (childImage == null)
					throw new NotSupportedException();
				imageRect.Union(childImage.Rect);
			}
			Bitmap bmp = new Bitmap((int)imageRect.Width, (int)imageRect.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			using (var g = Graphics.FromImage(bmp)) {
				foreach (ImageDrawing child in drawingGroup.Children) {
					g.DrawImage(GetBitmap(child.ImageSource), child.Rect.ToSystemDrawing());
				}
			}
			return bmp;
		}
		
		public Icon Icon {
			get {
				return LazyInitializer.EnsureInitialized(ref icon, () => SD.WinForms.BitmapToIcon(this.Bitmap));
			}
		}
	}
}
