// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace IconEditor
{
	/// <summary>
	/// .NET does not support alpha-transparent .bmp files.
	/// This means we have to use this workaround to load or save them.
	/// </summary>
	public static class AlphaTransparentBitmap
	{
		/// <summary>
		/// Loads an alpha-transparent bitmap from the specified stream.
		/// Only valid 32bit bitmaps are supported, everything else will throw an exception!
		/// </summary>
		public unsafe static Bitmap LoadAlphaTransparentBitmap(Stream stream)
		{
			const int knownHeaderSize = 4*3 + 2*2 + 4;
			const int MAXSIZE = ushort.MaxValue;
			
			using (BinaryReader r = new BinaryReader(stream)) {
				if (r.ReadUInt16() != 19778)
					throw new ArgumentException("The specified file is not a bitmap!");
				r.ReadInt32(); // ignore file size
				r.ReadInt32(); // ignore reserved bytes
				r.ReadInt32(); // ignore data start offset
				
				int biSize = r.ReadInt32();
				if (biSize <= knownHeaderSize)
					throw new ArgumentException("biSize invalid: " + biSize);
				if (biSize > 2048) // upper limit for header size
					throw new ArgumentException("biSize too high: " + biSize);
				int width = r.ReadInt32();
				int height = r.ReadInt32();
				if (width < 0 || height < 0)
					throw new ArgumentException("width and height must be >= 0");
				if (width > MAXSIZE || height > MAXSIZE)
					throw new ArgumentException("width and height must be < " + ushort.MaxValue);
				if (r.ReadInt16() != 1)
					throw new ArgumentException("biPlanes invalid");
				if (r.ReadInt16() != 32)
					throw new ArgumentException("Only 32bit bitmaps are supported!");
				if (r.ReadInt32() != 0)
					throw new ArgumentException("Only uncompressed bitmaps are supported!");
				
				// skip rest of header:
				r.ReadBytes(biSize - knownHeaderSize);
				
				// 32bit bitmaps don't have a color table, so the data section starts here immediately
				Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
				BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				try {
					if (bmpData.Stride != width * 4)
						throw new InvalidOperationException("expected 32bit bitmapdata");
					int byteCount = bmpData.Stride * bmpData.Height;
					uint* startPos = (uint*)bmpData.Scan0.ToPointer();
					uint* endPos = (uint*)((byte*)bmpData.Scan0.ToPointer() + byteCount);
					// .bmp files store the bitmap upside down, so we have to mirror it
					uint* tmpPos = startPos;
					startPos = endPos - width; // start of last line
					endPos = tmpPos - width; // start of (-1)st line
					for (uint* lineStart = startPos; lineStart != endPos; lineStart -= width) {
						uint* lineEnd = lineStart + width;
						for (uint* pos = lineStart; pos != lineEnd; pos++) {
							*pos = r.ReadUInt32();
						}
					}
				} finally {
					bmp.UnlockBits(bmpData);
				}
				return bmp;
			}
		}
	}
}
