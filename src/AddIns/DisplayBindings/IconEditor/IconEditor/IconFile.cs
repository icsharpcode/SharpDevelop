// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;

namespace IconEditor
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
				throw new InvalidIconException("reserved word must be 0");
			ushort type = r.ReadUInt16();
			if (type == 1)
				isCursor = false;
			else if (type == 2)
				isCursor = true;
			else
				throw new InvalidIconException("invalid icon type " + type);
			if (isCursor)
				throw new InvalidIconException("cursors are currently not supported");
			IconEntry[] icons = new IconEntry[r.ReadUInt16()];
			for (int i = 0; i < icons.Length; i++) {
				icons[i] = new IconEntry();
				icons[i].ReadHeader(r, ref wellFormed);
			}
			for (int i = 0; i < icons.Length; i++) {
				icons[i].ReadData(stream, ref wellFormed);
			}
			this.icons = new Collection<IconEntry>(icons);
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
				e.WriteHeader(stream, w);
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
		/// Gets a sorted list of all icon sizes available in this file.
		/// </summary>
		public IList<Size> AvailableSizes {
			get {
				List<Size> r = new List<Size>();
				foreach (IconEntry e in this.Icons) {
					if (r.Contains(e.Size) == false)
						r.Add(e.Size);
				}
				r.Sort(delegate(Size a, Size b) {
				       	int res = a.Width.CompareTo(b.Width);
				       	if (res == 0)
				       		return a.Height.CompareTo(b.Height);
				       	else
				       		return res;
				       });
				return r;
			}
		}
		
		/// <summary>
		/// Gets a sorted list of all color depths available in this file.
		/// </summary>
		public IList<int> AvailableColorDepths {
			get {
				bool[] depths = new bool[33];
				foreach (IconEntry e in this.Icons) {
					depths[e.ColorDepth] = true;
				}
				List<int> r = new List<int>();
				for (int i = 0; i < depths.Length; i++) {
					if (depths[i]) r.Add(i);
				}
				return r;
			}
		}
	}
}
