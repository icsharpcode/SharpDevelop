// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace HexEditor.Util
{
	/// <summary>
	/// Manages the data loaded into the hex editor.
	/// </summary>
	public class BufferManager
	{
		internal Control parent;
		OpenedFile currentFile;
		Stream stream;
		
		/// <summary>
		/// Currently used, but not good for really big files (like 590 MB)
		/// </summary>
		Rope<byte> buffer;
		
		/// <summary>
		/// Creates a new BufferManager and attaches it to a control.
		/// </summary>
		/// <param name="parent">The parent control to attach to.</param>
		public BufferManager(Control parent)
		{
			this.parent = parent;
			
			this.buffer = new Rope<byte>();
		}
		
		/// <summary>
		/// Cleares the whole buffer.
		/// </summary>
		public void Clear()
		{
			this.buffer.Clear();
			parent.Invalidate();
			GC.Collect();
		}
		
		/// <summary>
		/// Loads the data from a stream.
		/// </summary>
		public void Load(OpenedFile file, Stream stream)
		{
			this.currentFile = file;
			this.stream = stream;
			this.buffer.Clear();
			
			((Editor)this.parent).Enabled = false;
			
			if (File.Exists(currentFile.FileName)) {
				try {
					BinaryReader reader = new BinaryReader(this.stream, System.Text.Encoding.Default);
					
					while (reader.PeekChar() != -1) {
						this.buffer.AddRange(reader.ReadBytes(524288));
						UpdateProgress((int)(this.buffer.Count / (double)reader.BaseStream.Length) * 100);
					}
					
					reader.Close();
				} catch (OutOfMemoryException) {
					MessageService.ShowErrorFormatted("${res:FileUtilityService.FileSizeTooBig}");
				}
			} else {
				MessageService.ShowErrorFormatted("${res:Fileutility.CantFindFileError}", currentFile.FileName);
			}
			
			this.parent.Invalidate();
			
			UpdateProgress(100);
			
			if (this.parent.InvokeRequired)
				this.parent.Invoke(new MethodInvoker(
					delegate() {this.parent.Cursor = Cursors.Default;}
				));
			else {this.parent.Cursor = Cursors.Default;}
			
			
			((Editor)this.parent).LoadingFinished();
			
			((Editor)this.parent).Enabled = true;
			
			this.parent.Invalidate();
		}
		
		/// <summary>
		/// Writes all data to a stream.
		/// </summary>
		public void Save(OpenedFile file, Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(this.buffer.ToArray());
			writer.Flush();
		}
		
		/// <summary>
		/// Used for threading to update the processbars and stuff.
		/// </summary>
		/// <param name="percentage">The current percentage of the process</param>
		private void UpdateProgress(int percentage)
		{
			Editor c = (Editor)this.parent;
			
			Application.DoEvents();
			
			if (c.ProgressBar != null) {
				if (percentage >= 100) {
					if (c.InvokeRequired)
						c.Invoke(new MethodInvoker(
							delegate() {c.ProgressBar.Value = 100; c.ProgressBar.Visible = false;}
						));
					else {
						c.ProgressBar.Value = 100;
						c.ProgressBar.Visible = false; }
				} else {
					if (c.InvokeRequired)
						c.Invoke(new MethodInvoker(
							delegate() {c.ProgressBar.Value = percentage; c.ProgressBar.Visible = true;}
						));
					else { c.ProgressBar.Value = percentage; c.ProgressBar.Visible = true; }
				}
			}
		}
		
		/// <summary>
		/// Returns the current buffer as a byte[].
		/// </summary>
		public byte[] Buffer {
			get {
				if (buffer == null) return new byte[0];
				return buffer.ToArray();
			}
		}
		
		/// <summary>
		/// The size of the current buffer.
		/// </summary>
		public int BufferSize {
			get { return buffer.Count; }
		}

		#region Methods
		
		public byte[] GetBytes(int start, int count)
		{
			if (buffer.Count == 0) return new byte[] {};
			if (start < 0) start = 0;
			if (start >= buffer.Count) start = buffer.Count;
			if (count < 1) count = 1;
			if (count >= (buffer.Count - start)) count = (buffer.Count - start);
			return buffer.GetRange(start, count).ToArray();
		}

		public byte GetByte(int offset)
		{
			if (buffer.Count == 0) return 0;
			if (offset < 0) offset = 0;
			if (offset >= buffer.Count) offset = buffer.Count - 1;
			return (byte)buffer[offset];
		}
		
		public bool DeleteByte(int offset)
		{
			if ((offset < buffer.Count) & (offset > -1)) {
				buffer.RemoveAt(offset);
				return true;
			}
			return false;
		}
		
		public bool RemoveByte(int offset)
		{
			if ((offset < buffer.Count) & (offset > -1)) {
				buffer.RemoveAt(offset);
				return true;
			}
			return false;
		}
		
		public bool RemoveBytes(int offset, int length)
		{
			if (((offset < buffer.Count) && (offset > -1)) && ((offset + length) <= buffer.Count)) {
				buffer.RemoveRange(offset, length);
				return true;
			}
			return false;
		}
		
		public void SetBytes(int start, byte[] bytes, bool overwrite)
		{
			if (overwrite) {
				if (bytes.Length > buffer.Count) buffer.AddRange(new byte[bytes.Length - buffer.Count]);
				for (int i = start; i < start + bytes.Length; i++)
					buffer[i] = bytes[i - start];
			} else {
				buffer.InsertRange(start, bytes);
			}
		}
		
		public void SetByte(int position, byte @byte, bool overwrite)
		{
			if (overwrite) {
				if (position > buffer.Count - 1) {
					buffer.Add(@byte);
				} else {
					buffer[position] = @byte;
				}
			} else {
				buffer.Insert(position, @byte);
			}
		}
		#endregion
	}
}
