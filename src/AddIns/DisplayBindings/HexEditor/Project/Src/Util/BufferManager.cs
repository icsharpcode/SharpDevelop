// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;

namespace HexEditor.Util
{
	/// <summary>
	/// Manages the data loaded into the hex editor.
	/// </summary>
	public class BufferManager
	{
		Editor parent;
		OpenedFile currentFile;
		
		StreamedList data;
		
		/// <summary>
		/// Creates a new BufferManager and attaches it to a control.
		/// </summary>
		/// <param name="parent">The parent control to attach to.</param>
		public BufferManager(Editor parent)
		{
			this.parent = parent;
			this.data = new StreamedList();
		}
		
		/// <summary>
		/// Loads the data from a stream.
		/// </summary>
		public void Load(OpenedFile file, Stream stream)
		{
			this.currentFile = file;
			
			this.data.Clear();
			this.data = new StreamedList();
			
			stream.Position = 0;
			byte[] bytes = new byte[512000];
			
			if (File.Exists(currentFile.FileName)) {
				int count = 0;
				do {
					count = stream.Read(bytes, 0, bytes.Length);
					if (count > 0) {
						this.data.AddRange(bytes, count);
						// Maybe not a good solution, but easier than implementing a complete thread
						// A thread would need to reopen the file in the thread
						// because it is closed after the CreateContentForFile() has
						// finished.
						Application.DoEvents();
						GC.Collect();
					}
				} while (count > 0);
			} else {
				MessageService.ShowError(new FileNotFoundException("The file " + currentFile.FileName + " doesn't exist!", currentFile.FileName), "The file " + currentFile.FileName + " doesn't exist!");
			}
			
			this.parent.Invalidate();
		}
		
		/// <summary>
		/// Writes all data to a stream.
		/// </summary>
		public void Save(OpenedFile file, Stream stream)
		{
			this.data.ConcatenateTo(stream);
		}
		
		/// <summary>
		/// The size of the current buffer.
		/// </summary>
		public int BufferSize {
			get {
				return this.data.Count;
			}
		}

		#region Methods
		public byte[] GetBytes(int start, int count)
		{
			if (this.BufferSize == 0) return new byte[] {};
			if (start < 0) start = 0;
			if (start >= this.BufferSize) start = this.BufferSize;
			if (count < 1) count = 1;
			if (count >= (this.BufferSize - start)) count = (this.BufferSize - start);
			
			return this.data.GetRange(start, count);
		}

		public byte GetByte(int offset)
		{
			if (this.BufferSize == 0) return 0;
			if (offset < 0) offset = 0;
			if (offset >= this.BufferSize) offset = this.BufferSize;
			
			return this.data[offset];
		}
		
		public bool RemoveByte(int offset)
		{
			if ((offset < this.BufferSize) & (offset > -1)) {
				data.RemoveAt(offset);
				return true;
			}
			return false;
		}
		
		public bool RemoveBytes(int offset, int length)
		{
			if (((offset < this.BufferSize) && (offset > -1)) && ((offset + length) <= this.BufferSize)) {
				if ((offset == 0) && (length == data.Count)) {
					this.data.Clear();
					this.data = new StreamedList();
				} else {
					this.data.RemoveRange(offset, length);
				}
				return true;
			}
			return false;
		}
		
		public void SetBytes(int start, byte[] bytes, bool overwrite)
		{
			if (overwrite) {
				foreach (byte b in bytes)
				{
					data[start] = b;
					start++;
				}
			} else {
				foreach (byte b in bytes)
				{
					data.Insert(start, b);
					start++;
				}
			}
		}
		
		public void SetByte(int position, byte @byte, bool overwrite)
		{
			if (overwrite) {
				data[position] = @byte;
			} else {
				if (position == data.Count) {
					data.Add(@byte);
				} else {
					data.Insert(position, @byte);
				}
			}
		}
		#endregion
	}
}

