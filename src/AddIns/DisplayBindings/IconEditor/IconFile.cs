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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ICSharpCode.IconEditor
{
	/// <summary>
	/// Icon class supporting XP and Vista icons.
	/// </summary>
	public sealed class IconFile
	{
		bool isCursor;
		bool wellFormed = true;
		Collection<IconEntry> icons;
		
		/// <summary>
		/// Gets if the file format was well formed.
		/// IconFile normally throws exceptions on invalid icon files, but some
		/// mistakes are common. In such cases, IconFile continues to load the file
		/// but sets WellFormed to false.
		/// </summary>
		public bool WellFormed {
			get {
				return wellFormed;
			}
		}
		
		/// <summary>
		/// Gets/Sets if the cursor flag is set in the icon file.
		/// </summary>
		public bool IsCursor {
			get {
				return isCursor;
			}
			set {
				isCursor = value;
			}
		}
		
		/// <summary>
		/// Gets the list of icons in this file.
		/// </summary>
		public Collection<IconEntry> Icons {
			get {
				return icons;
			}
		}
		
		/// <summary>
		/// Creates an empty icon file.
		/// </summary>
		public IconFile()
		{
			icons = new Collection<IconEntry>();
		}
		
		/// <summary>
		/// Loads an icon file from a stream.
		/// </summary>
		public IconFile(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanRead)
				throw new ArgumentException("The stream must be readable", "stream");
			if (!stream.CanSeek)
				throw new ArgumentException("The stream must be seekable", "stream");
			LoadIcon(stream);
		}
		
		/// <summary>
		/// Loads an icon file from a file.
		/// </summary>
		public IconFile(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
				LoadIcon(fs);
			}
		}
		
		void LoadIcon(Stream stream)
		{
			BinaryReader r = new BinaryReader(stream);
			if (r.ReadUInt16() != 0)
				throw new InvalidIconException("This is not a valid .ico file.");
			ushort type = r.ReadUInt16();
			if (type == 1)
				isCursor = false;
			else if (type == 2)
				isCursor = true;
			else
				throw new InvalidIconException("This is not a valid .ico file.");
			IconEntry[] icons = new IconEntry[r.ReadUInt16()];
			for (int i = 0; i < icons.Length; i++) {
				icons[i] = new IconEntry();
				icons[i].ReadHeader(r, isCursor, ref wellFormed);
			}
			for (int i = 0; i < icons.Length; i++) {
				icons[i].ReadData(stream, ref wellFormed);
			}
			// need to use List<T> so that the collection can be resized
			this.icons = new Collection<IconEntry>(new List<IconEntry>(icons));
		}
		
		/// <summary>
		/// Save the icon to the specified file.
		/// </summary>
		public void Save(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
				Save(fs);
			}
		}
		
		/// <summary>
		/// Save the icon to the specified stream.
		/// </summary>
		public void Save(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanWrite)
				throw new ArgumentException("The stream must be writeable", "stream");
			if (!stream.CanSeek)
				throw new ArgumentException("The stream must be seekable", "stream");
			BinaryWriter w = new BinaryWriter(stream);
			w.Write((ushort)0); // reserved
			w.Write((ushort)(isCursor ? 2 : 1));
			w.Write((ushort)icons.Count);
			foreach (IconEntry e in icons) {
				e.WriteHeader(stream, isCursor, w);
			}
			foreach (IconEntry e in icons) {
				e.WriteData(stream);
			}
		}
		
		/// <summary>
		/// Gets the icon entry with the specified width and height.
		/// </summary>
		/// <param name="size">Width and height of the entry to get.</param>
		/// <param name="bestSupported">The "best" entry type allowed to get.
		/// Use Compressed to get all entries; TrueColor to get all except
		/// the Vista-only compressed entries; and Classic to get only the
		/// classic icons that run on all Windows versions.
		/// </param>
		/// <returns>Gets the entry matching the size. If multiple supported
		/// entries are available, the only with the highest color depth is returned.
		/// If no matching entry is found, null is returned.</returns>
		public IconEntry GetEntry(Size size, IconEntryType bestSupported)
		{
			IconEntry best = null;
			foreach (IconEntry e in this.Icons) {
				if (e.Size == size && e.Type <= bestSupported) {
					if (best == null || best.ColorDepth < e.ColorDepth) {
						best = e;
					}
				}
			}
			return best;
		}
		
		/// <summary>
		/// Adds the specified entry
		/// </summary>
		public void AddEntry(IconEntry entry)
		{
			if (entry == null)
				throw new ArgumentNullException("entry");
			// replace matching existing entry:
			for (int i = 0; i < icons.Count; i++) {
				if (icons[i].Width == entry.Width && icons[i].Height == entry.Height && icons[i].ColorDepth == entry.ColorDepth) {
					icons[i] = entry;
					return;
				}
			}
			icons.Add(entry);
		}
		
		public void RemoveEntry(int width, int height, int colorDepth)
		{
			for (int i = 0; i < icons.Count; i++) {
				if (icons[i].Width == width && icons[i].Height == height && icons[i].ColorDepth == colorDepth) {
					icons.RemoveAt(i);
					break;
				}
			}
		}
		
		/// <summary>
		/// Gets a sorted list of all icon sizes available in this file.
		/// </summary>
		public IEnumerable<Size> AvailableSizes {
			get {
				return this.Icons.Select(e => e.Size).Distinct().OrderBy(s => s.Width).ThenBy(s => s.Height);
			}
		}
		
		/// <summary>
		/// Gets a sorted list of all color depths available in this file.
		/// </summary>
		public IEnumerable<int> AvailableColorDepths {
			get {
				return this.Icons.Select(e => e.ColorDepth).Distinct().OrderBy(v => v);
			}
		}
	}
}
