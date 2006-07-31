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
				output.Position = 14 + 8; // position of biHeight in header
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
				w.Write(0); // biSizeImage, should be zero
				w.Write(0); // biXPelsPerMeter, should be zero
				w.Write(0); // biYPelsPerMeter, should be zero
				w.Write(0); // biClrUsed - calculate color count using bitCount
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
		
		/// <summary>
		/// Gets the size of the data section of a DIB bitmap with the
		/// specified parameters
		/// </summary>
		static int GetBitmapSize(int width, int height, int bitsPerPixel)
		{
			const int bitPack = 4*8; // 4 byte packing
			int lineBits = width * bitsPerPixel;
			// expand size to multiple of 4 bytes
			int rem = lineBits % bitPack;
			if (rem != 0) {
				lineBits += (bitPack - rem);
			}
			return lineBits / 8 * height;
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
			return new Bitmap(GetMaskImageData());
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
			if (sizeInBytes > 8) {
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
			const int knownHeaderSize = 4*3 + 2*2 + 4;
			const int BI_RGB = 0;
			
			int biSize = b.ReadInt32();
			if (biSize <= knownHeaderSize)
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
			
			//if (biBitCount != colorDepth)
			//	throw new InvalidIconException("biBitCount invalid: " + biBitCount);
			int compression = b.ReadInt32();
			if (compression != BI_RGB)
				throw new InvalidIconException("biCompression invalid");
			
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
		
		internal void ReadHeader(BinaryReader r, ref bool wellFormed)
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
			if (r.ReadByte() != 0) {
				throw new InvalidIconException("Invalid value for reserved");
			}
			
			uint planeCount = r.ReadUInt16();
			// placeCount should always be 1, but there are some icons with planeCount = 0
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
		
		internal void WriteHeader(Stream stream, BinaryWriter w)
		{
			w.Write((byte)(width == 256 ? 0 : width));
			w.Write((byte)(height == 256 ? 0 : height));
			w.Write((byte)(colorDepth == 4 ? 16 : 0));
			w.Write((byte)0);
			w.Write((ushort)1);
			w.Write((ushort)colorDepth);
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
			if (isCompressed == false) {
				using (BinaryReader r = new BinaryReader(new MemoryStream(imageData, false))) {
					int biBitCount;
					CheckBitmapHeader(r, out biBitCount);
					if (biBitCount != colorDepth) {
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
		public void SetImage(Bitmap bitmap, bool storeCompressed)
		{
			if (this.Type == IconEntryType.Classic)
				throw new InvalidOperationException("Cannot use SetImage on classic entries");
			if (bitmap.Width != width || bitmap.Height != height) {
				bitmap = new Bitmap(bitmap, width, height);
			}
			PixelFormat expected;
			switch (colorDepth) {
				case 1:
					expected = PixelFormat.Format1bppIndexed;
					break;
				case 4:
					expected = PixelFormat.Format4bppIndexed;
					break;
				case 8:
					expected = PixelFormat.Format8bppIndexed;
					break;
				case 24:
					expected = PixelFormat.Format24bppRgb;
					break;
				case 32:
					expected = PixelFormat.Format32bppArgb;
					break;
				default:
					throw new NotSupportedException();
			}
			if (bitmap.PixelFormat != expected) {
				if (expected == PixelFormat.Format32bppArgb) {
					bitmap = new Bitmap(bitmap, width, height);
				} else {
					throw new NotImplementedException();
				}
			}
			if (storeCompressed) {
				using (MemoryStream ms = new MemoryStream()) {
					bitmap.Save(ms, ImageFormat.Png);
					SetEntryData(ms.ToArray());
				}
			} else {
				throw new NotImplementedException();
			}
		}
		
		public override string ToString()
		{
			return string.Format("[IconEntry {0}x{1}x{2}]", this.width, this.height, this.colorDepth);
		}
	}
}
