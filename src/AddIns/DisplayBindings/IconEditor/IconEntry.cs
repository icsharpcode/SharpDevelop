// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ICSharpCode.IconEditor
{
	/// <summary>
	/// Describes the type of an icon entry.
	/// </summary>
	public enum IconEntryType
	{
		/// <summary>
		/// Classic icons are drawn by AND-ing the background with the mask image,
		/// then XOR-ing the real image. Classic icons can have the color depths
		/// 1bit (2 colors), 4bit (16 colors) and 8bit (256 color).
		/// Additionally, the mask image provides the 2 "colors" transparent and
		/// invert background. Inverting only some colors of the background is
		/// theoretically possible, but not used in practice.
		/// Background color inversion is mainly used by cursors.
		/// There are also 16bit or 24bit true-color classic icons, though
		/// they don't make sense because they seem to be supported on XP,
		/// in which case you should use real alpha-transparent 32bit icons.
		/// </summary>
		Classic = 0,
		/// <summary>
		/// True color icons were introduced by Windows XP and are not supported
		/// by previous Windows versions. The AND mask is still present but unused;
		/// instead the (former) XOR-part of the image has an alpha channel
		/// allowing partial transparency, e.g. for smooth shadows.
		/// These icons always have a color depth of 32bit.
		/// </summary>
		TrueColor = 1,
		/// <summary>
		/// Compressed icons were introduced by Windows Vista and are not supported
		/// by previous Windows versions. These icons simply contain a complete
		/// .png file in the entries' data.
		/// The .png files can use palette images with a single transparency color
		/// or true color with alpha channel; though usually the compressed format
		/// is only used for the high-resolution 256x256x32 icons in Vista.
		/// </summary>
		Compressed = 2
	}
	
	/// <summary>
	/// A single image in an icon file.
	/// </summary>
	public sealed class IconEntry
	{
		// official icon sizes: 16, 24, 32, 48, 256
		// common hi-quality icon sizes: 64, 98, 128
		// hi-quality for Smartphones: 22, 44
		// static readonly int[] supportedSizes = {16, 22, 24, 44, 32, 48, 64, 96, 128, 256};
		
		int width, height, colorDepth;
		bool isCompressed;
		
		int offsetInFile, sizeInBytes;
		byte[] entryData;
		
		public int Width {
			get {
				return width;
			}
		}
		
		public int Height {
			get {
				return height;
			}
		}
		
		public Size Size {
			get {
				return new Size(width, height);
			}
		}
		
		public int ColorDepth {
			get {
				return colorDepth;
			}
		}
		
		/// <summary>
		/// Gets/Sets the hotspot (cursors only).
		/// </summary>
		public Point Hotspot { get; set; }
		
		public static PixelFormat GetPixelFormat(int colorDepth)
		{
			switch (colorDepth) {
				case 1:
					return PixelFormat.Format1bppIndexed;
				case 4:
					return PixelFormat.Format4bppIndexed;
				case 8:
					return PixelFormat.Format8bppIndexed;
				case 24:
					return PixelFormat.Format24bppRgb;
				case 32:
					return PixelFormat.Format32bppArgb;
				default:
					throw new NotSupportedException();
			}
		}
		
		public IconEntryType Type {
			get {
				if (isCompressed)
					return IconEntryType.Compressed;
				else if (colorDepth == 32)
					return IconEntryType.TrueColor;
				else
					return IconEntryType.Classic;
			}
		}
		
		/// <summary>
		/// Gets the raw data of this image.
		/// For uncompressed entries, this is a ICONIMAGE structure.
		/// For compressed entries, this is a .PNG file.
		/// </summary>
		public Stream GetEntryData()
		{
			return new MemoryStream(entryData, false);
		}
		
		/// <summary>
		/// Gets the data of this image.
		/// For uncompressed entries, this is a .BMP file.
		/// For compressed entries, this is a .PNG file.
		/// </summary>
		public Stream GetImageData()
		{
			const int bmpFileHeaderLength = 14;
			const int positionOfHeightInHeader = 8;
			
			Stream stream = GetEntryData();
			if (isCompressed)
				return stream;
			using (BinaryReader b = new BinaryReader(stream)) {
				int biBitCount;
				int headerSize = CheckBitmapHeader(b, out biBitCount);
				MemoryStream output = new MemoryStream();
				BinaryWriter w = new BinaryWriter(output);
				w.Write((ushort)19778); // "BM" mark
				w.Write(0); // file size, we'll fill it in later
				w.Write(0); // 4 reserved bytes
				w.Write(0); // data start offset, we'll fill it in later
				w.Write(entryData, 0, headerSize); // write header
				output.Position = bmpFileHeaderLength + positionOfHeightInHeader;
				w.Write(height); // write correct height into header
				output.Position = output.Length;
				if (biBitCount <= 8) {
					// copy color table:
					int colorTableSize = 4 * (1 << biBitCount);
					w.Write(b.ReadBytes(colorTableSize));
				}
				output.Position = 10; // fill in data start offset
				w.Write((int)output.Length);
				output.Position = output.Length;
				
				// copy bitmap data:
				w.Write(b.ReadBytes(GetBitmapSize(width, height, biBitCount)));
				
				output.Position = 2; // fill in file size
				w.Write((int)output.Length);
				output.Position = 0;
				return output;
			}
		}
		
		/// <summary>
		/// Gets the data of the image mask.
		/// The result is a monochrome .BMP file where the transparent
		/// image regions are marked as white and the opaque regions
		/// are black. This is used as AND-mask before drawing the main image
		/// with XOR.
		/// </summary>
		/// <exception cref="InvalidOperationException">Image masks are only used in uncompressed icons,
		/// an InvalidOperationException is thrown if you call GetImageMaskData on a compressed icon.</exception>
		public Stream GetMaskImageData()
		{
			if (isCompressed)
				throw new InvalidOperationException("Image masks are only used in uncompressed icons.");
			Stream readStream = GetEntryData();
			using (BinaryReader b = new BinaryReader(readStream)) {
				int biBitCount;
				int headerSize = CheckBitmapHeader(b, out biBitCount);
				MemoryStream output = new MemoryStream();
				BinaryWriter w = new BinaryWriter(output);
				w.Write((ushort)19778); // "BM" mark
				w.Write(0); // file size, we'll fill it in later
				w.Write(0); // 4 reserved bytes
				w.Write(0); // data start offset, we'll fill it in later
				
				w.Write(40); // header size
				w.Write((int)width);
				w.Write((int)height);
				w.Write((short)1); // 1 plane
				w.Write((short)1); // monochrome
				w.Write(0); // no compression
				w.Write(GetBitmapSize(width, height, 1)); // biSizeImage
				w.Write(0); // biXPelsPerMeter, should be zero
				w.Write(0); // biYPelsPerMeter, should be zero
				w.Write(0); // biClrUsed - should be 0 for monochrome bitmaps
				w.Write(0); // no special "important" colors
				
				// write color table:
				
				w.Write(0); // write black into color table
				
				// write white into color table:
				w.Write((byte)255);
				w.Write((byte)255);
				w.Write((byte)255);
				w.Write((byte)0);
				
				output.Position = 10; // fill in data start offset
				w.Write((int)output.Length);
				output.Position = output.Length;
				
				// skip real color table:
				if (biBitCount <= 8) {
					readStream.Position += 4 * (1 << biBitCount);
				}
				
				// skip real bitmap data:
				readStream.Position += GetBitmapSize(width, height, biBitCount);
				
				// copy mask bitmap data:
				w.Write(b.ReadBytes(GetBitmapSize(width, height, 1)));
				
				output.Position = 2; // fill in file size
				w.Write((int)output.Length);
				output.Position = 0;
				return output;
			}
		}
		
		static int GetStride(int width, int bitsPerPixel)
		{
			const int pack = 4; // 4 byte packing
			const int bitPack = pack*8;
			int lineBits = width * bitsPerPixel;
			// divide by bitPack and round up:
			int packUnits = (lineBits + (bitPack - 1)) / bitPack;
			return packUnits * pack;
		}
		
		/// <summary>
		/// Gets the size of the data section of a DIB bitmap with the
		/// specified parameters
		/// </summary>
		static int GetBitmapSize(int width, int height, int bitsPerPixel)
		{
			return GetStride(width, bitsPerPixel) * height;
		}
		
		/// <summary>
		/// Gets the image.
		/// For uncompressed palette images, this returns the XOR part of the entry.
		/// For 32bit images and compressed images, this returns a bitmap with
		/// alpha transparency.
		/// </summary>
		public Bitmap GetImage()
		{
			Stream data = GetImageData();
			if (IsCompressed || ColorDepth != 32) {
				return new Bitmap(data);
			} else {
				// new Bitmap() does not work with alpha-transparent .bmp's
				// Therefore, we have to use our own little bitmap loader
				return AlphaTransparentBitmap.LoadAlphaTransparentBitmap(data);
			}
		}
		
		/// <summary>
		/// Gets the the image mask.
		/// The result is a monochrome bitmap where the transparent
		/// image regions are marked as white and the opaque regions
		/// are black. This is used as AND-mask before drawing the main image
		/// with XOR.
		/// </summary>
		/// <exception cref="InvalidOperationException">Image masks are only used in uncompressed icons,
		/// an InvalidOperationException is thrown if you call GetImageMask on a compressed icon.</exception>
		public Bitmap GetMaskImage()
		{
			var stream = GetMaskImageData();
			try {
				return new Bitmap(stream);
			} catch (ArgumentException) {
				return null;
			}
		}
		
		/// <summary>
		/// Sets the data to be used by the icon.
		/// </summary>
		public void SetEntryData(byte[] entryData)
		{
			if (entryData == null)
				throw new ArgumentNullException("imageData");
			this.entryData = entryData;
			isCompressed = false;
			if (entryData.Length > 8) {
				// PNG Specification, section 5.2:
				// The first eight bytes of a PNG datastream always contain the following (decimal) values:
				// 137 80 78 71 13 10 26 10
				
				if (entryData[0] == 137 &&
				    entryData[1] == 80 &&
				    entryData[2] == 78 &&
				    entryData[3] == 71 &&
				    entryData[4] == 13 &&
				    entryData[5] == 10 &&
				    entryData[6] == 26 &&
				    entryData[7] == 10)
				{
					isCompressed = true;
				}
			}
		}
		
		int CheckBitmapHeader(BinaryReader b, out int biBitCount)
		{
			const int knownHeaderSize = 4*3 + 2*2 + 6*4;
			const int BI_RGB = 0;
			
			int biSize = b.ReadInt32();
			if (biSize < knownHeaderSize)
				throw new InvalidIconException("biSize invalid: " + biSize);
			if (b.ReadInt32() != width)
				throw new InvalidIconException("biWidth invalid");
			int biHeight = b.ReadInt32();
			if (biHeight != 2*height) // double of normal height for AND bitmap
				throw new InvalidIconException("biHeight invalid: " + biHeight);
			if (b.ReadInt16() != 1)
				throw new InvalidIconException("biPlanes invalid");
			biBitCount = b.ReadInt16();
			
			// Do not test biBitCount: there are icons where the colorDepth is saved
			// incorrectly; biBitCount is the real value to use in those cases
			
			int biCompression = b.ReadInt32();
			if (biCompression != BI_RGB)
				throw new InvalidIconException("biCompression invalid");
			
			b.ReadInt32(); // biSizeImage
			b.ReadInt32(); // biXPelsPerMeter
			b.ReadInt32(); // biYPelsPerMeter
			int biClrUsed = b.ReadInt32();
			if (biClrUsed != 0 && biClrUsed != (1 << biBitCount))
				throw new InvalidIconException("biClrUsed invalid");
			
			b.ReadInt32(); // biClrImportant
			
			// skip rest of header:
			b.ReadBytes(biSize - knownHeaderSize);
			return biSize;
		}
		
		/// <summary>
		/// Gets if the entry is compressed.
		/// Compressed entries are PNG files, uncompressed entries
		/// are a special DIB-like format.
		/// </summary>
		public bool IsCompressed {
			get {
				return isCompressed;
			}
		}
		
		internal IconEntry()
		{
		}
		
		public IconEntry(int width, int height, int colorDepth, byte[] imageData)
		{
			this.width = width;
			this.height = height;
			this.colorDepth = colorDepth;
			CheckSize();
			CheckColorDepth();
			SetEntryData(imageData);
		}
		
		public IconEntry(int width, int height, int colorDepth, Bitmap bitmap, bool? storeCompressed = null)
		{
			this.width = width;
			this.height = height;
			this.colorDepth = colorDepth;
			CheckSize();
			CheckColorDepth();
			SetImage(bitmap, storeCompressed);
		}
		
		void CheckSize()
		{
			if (width <= 0 || height <= 0 || width > 256 || height > 256) {
				throw new InvalidIconException("Invalid icon size: " + width + "x" + width);
			}
		}
		
		void CheckColorDepth()
		{
			switch (colorDepth) {
				case 1: // monochrome icon
				case 4: // 16 palette colors
				case 8: // 256 palette colors
				case 32: // XP icon with alpha channel
				case 16: // allowed by the spec, but very uncommon
				case 24: // non-standard, but common
					break;
				default:
					throw new InvalidIconException("Unknown color depth: " + colorDepth);
			}
		}
		
		internal void ReadHeader(BinaryReader r, bool isCursor, ref bool wellFormed)
		{
			width = r.ReadByte();
			height = r.ReadByte();
			// For Vista 256x256 icons:
			if (width == 0) width = 256;
			if (height == 0) height = 256;
			CheckSize();
			byte colorCount = r.ReadByte();
			if (colorCount != 0 && colorCount != 2 && colorCount != 16) {
				throw new InvalidIconException("Invalid color count: " + colorCount);
			}
			byte reserved = r.ReadByte();
			if (reserved != 0 && reserved != 255) {
				// should be 0, but .NET Icon.Save() uses 255
				throw new InvalidIconException("Invalid value for reserved");
			}
			
			if (isCursor) {
				colorDepth = -1;
				this.Hotspot = new Point(r.ReadUInt16(), r.ReadUInt16());
				if (this.Hotspot.X >= width || this.Hotspot.Y >= height)
					throw new InvalidIconException("Hotspot is outside image");
			} else {
				uint planeCount = r.ReadUInt16();
				// planeCount should always be 1, but there are some icons with planeCount = 0
				if (planeCount == 0) {
					wellFormed = false;
				}
				if (planeCount > 1) {
					throw new InvalidIconException("Invalid number of planes: " + planeCount);
				}
				colorDepth = r.ReadUInt16();
				if (colorDepth == 0) {
					if (colorCount == 2)
						colorDepth = 1;
					else if (colorCount == 16)
						colorDepth = 4;
					else if (colorCount == 0)
						colorDepth = 8;
				}
				CheckColorDepth();
			}
			
			sizeInBytes = r.ReadInt32();
			if (sizeInBytes <= 0) {
				throw new InvalidIconException("Invalid entry size: " + sizeInBytes);
			}
			if (sizeInBytes > 10*1024*1024) {
				throw new InvalidIconException("Entry too large: " + sizeInBytes);
			}
			offsetInFile = r.ReadInt32();
			if (offsetInFile <= 0) {
				throw new InvalidIconException("Invalid offset in file: " + offsetInFile);
			}
		}
		
		uint saveOffsetToHeaderPosition;
		
		internal void WriteHeader(Stream stream, bool isCursor, BinaryWriter w)
		{
			w.Write((byte)(width == 256 ? 0 : width));
			w.Write((byte)(height == 256 ? 0 : height));
			w.Write((byte)(colorDepth == 4 ? 16 : 0));
			w.Write((byte)0);
			if (isCursor) {
				w.Write((ushort)Hotspot.X);
				w.Write((ushort)Hotspot.Y);
			} else {
				w.Write((ushort)1);
				w.Write((ushort)colorDepth);
			}
			w.Write((int)entryData.Length);
			saveOffsetToHeaderPosition = (uint)stream.Position;
			w.Write((uint)0);
		}
		
		internal void ReadData(Stream stream, ref bool wellFormed)
		{
			stream.Position = offsetInFile;
			byte[] imageData = new byte[sizeInBytes];
			int pos = 0;
			while (pos < imageData.Length) {
				int c = stream.Read(imageData, pos, imageData.Length - pos);
				if (c == 0)
					throw new InvalidIconException("Unexpected end of stream");
				pos += c;
			}
			SetEntryData(imageData);
			if (isCompressed) {
				if (colorDepth == -1)
					colorDepth = 32; // assume 32-bit colors for .pngs
			} else {
				using (BinaryReader r = new BinaryReader(new MemoryStream(imageData, false))) {
					int biBitCount;
					CheckBitmapHeader(r, out biBitCount);
					if (colorDepth == -1) {
						// missing color depth (cursors)
						colorDepth = biBitCount;
						CheckColorDepth();
					} else if (biBitCount != colorDepth) {
						// inconsistency in header information, fix icon header
						wellFormed = false;
						colorDepth = biBitCount;
						CheckColorDepth();
					}
				}
			}
		}
		
		internal void WriteData(Stream stream)
		{
			uint pos = (uint)stream.Position;
			stream.Position = saveOffsetToHeaderPosition;
			stream.Write(BitConverter.GetBytes(pos), 0, 4);
			stream.Position = pos;
			stream.Write(entryData, 0, entryData.Length);
		}
		
		/// <summary>
		/// Stores the specified bitmap. The bitmap will be resized and
		/// changed to the correct pixel format.
		/// </summary>
		public unsafe void SetImage(Bitmap bitmap, bool? storeCompressed)
		{
			if (bitmap.Width != width || bitmap.Height != height) {
				bitmap = new Bitmap(bitmap, width, height);
			}
			PixelFormat format = GetPixelFormat(colorDepth);
			if (storeCompressed ?? (colorDepth == 32 && (width > 48 || height > 48))) {
				bitmap = Convert(bitmap, format);
				using (MemoryStream ms = new MemoryStream()) {
					bitmap.Save(ms, ImageFormat.Png);
					SetEntryData(ms.ToArray());
				}
			} else {
				// Make clone of bitmap because we're going to modify it
				bitmap = bitmap.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format32bppArgb);
				// Calculate AND mask and set transparent parts to black
				int maskStride = GetStride(width, 1);
				byte[] andMask = new byte[GetBitmapSize(width, height, 1)];
				BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
				try {
					uint* bmpPtr = (uint*)bmpData.Scan0.ToPointer();
					int bmpStride = bmpData.Stride / sizeof(uint);
					for (int y = 0; y < height; y++) {
						uint* linePtr = bmpPtr + (height - 1 - y) * bmpStride;
						for (int x = 0; x < width; x++) {
							if ((linePtr[x] & 0xff000000) < 0x80000000u) {
								// pixel is more than 50% transparent
								// set AND mask to white
								andMask[y * maskStride + (x >> 3)] |= (byte)(0x80 >> (x & 7));
								if (colorDepth < 32) {
									linePtr[x] = 0xff000000; // set to black if target format doesn't support transparency
								}
							}
						}
					}
				} finally {
					bitmap.UnlockBits(bmpData);
				}
				// Convert bitmap
				bitmap = Convert(bitmap, format);
				MemoryStream ms = new MemoryStream();
				BinaryWriter w = new BinaryWriter(ms);
				w.Write(40); // Header Size
				w.Write((int)width);
				w.Write((int)height * 2);
				w.Write((ushort)1); // biPlanes
				w.Write((ushort)colorDepth); // biBitCount
				w.Write(0); // biCompression
				w.Write(0); // biSizeImage
				w.Write(0); // biXPelsPerMeter
				w.Write(0); // biYPelsPerMeter
				w.Write(0); // biClrUsed
				w.Write(0); // biClrImportant
				
				// Color palette:
				if (colorDepth <= 8) {
					Color[] palette = bitmap.Palette.Entries;
					if (palette.Length > (1 << colorDepth))
						throw new InvalidOperationException("Palette has wrong size");
					foreach (Color c in palette) {
						w.Write(c.B);
						w.Write(c.G);
						w.Write(c.R);
						w.Write((byte)0);
					}
					for (int i = palette.Length; i < (1 << colorDepth); i++) {
						w.Write(0);
					}
				}
				// image data
				bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
				try {
					if (bmpData.Stride != GetStride(width, colorDepth))
						throw new InvalidOperationException();
					byte[] lineBuffer = new byte[bmpData.Stride];
					for (int y = height - 1; y >= 0; y--) {
						Marshal.Copy(bmpData.Scan0 + y * bmpData.Stride, lineBuffer, 0, bmpData.Stride);
						w.Write(lineBuffer);
					}
				} finally {
					bitmap.UnlockBits(bmpData);
				}
				w.Write(andMask);
				SetEntryData(ms.ToArray());
			}
		}
		
		static Bitmap Convert(Bitmap bitmap, PixelFormat targetFormat)
		{
			if (bitmap.PixelFormat == targetFormat)
				return bitmap;
			else
				return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), targetFormat);
		}
		
		public override string ToString()
		{
			return string.Format("[IconEntry {0}x{1}x{2}]", this.width, this.height, this.colorDepth);
		}
		
		/// <summary>
		/// Exports the image as ARGB bitmap.
		/// Note: if the bitmap is using the classic style (AND/XOR maps), any pixel
		/// where the AND map isn't 0 is set to fully transparent.
		/// </summary>
		public Bitmap ExportArgbBitmap()
		{
			if (colorDepth == 32) {
				return GetImage();
			} else if (isCompressed) {
				using (Bitmap image = GetImage()) {
					return image.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format32bppArgb);
				}
			} else {
				using (Bitmap mask = GetMaskImage(), image = GetImage()) {
					return AlphaTransparentBitmap.ConvertToAlphaTransparentBitmap(mask, image);
				}
			}
		}
	}
}
