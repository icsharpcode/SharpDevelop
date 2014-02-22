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
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace HexEditor.Util
{
	/// <summary>
	/// Manages the data loaded into the hex editor.
	/// </summary>
	public class BufferManager
	{
		/// <summary>
		/// Currently used, but not good for really big files (like 590 MB)
		/// </summary>
		Rope<byte> buffer = new Rope<byte>();
		
		readonly OpenedFile file;
		
		public event EventHandler<EventArgs> BufferChanged;
		
		protected virtual void OnBufferChanged(EventArgs e)
		{
			var handler = BufferChanged;
			if (handler != null)
				handler(this, e);
		}
		
		/// <summary>
		/// Creates a new BufferManager and attaches it to a control.
		/// </summary>
		public BufferManager(OpenedFile file)
		{
			this.file = file;
			var stream = file.GetModel(FileModels.Binary).OpenRead();
			this.LoadTask = Task.Run(() => DoLoad(stream));
		}
		
		public Task LoadTask { get; private set; }
		public IProgressMonitor Progress { get; set; }
		
		bool OnProgressUpdated(double d)
		{
			var progress = Progress;
			if (progress != null) {
				progress.Report(d);
				return !progress.CancellationToken.IsCancellationRequested;
			}
			return true;
		}
		
		const int CHUNK_SIZE = 1024 * 512;
		
		void DoLoad(Stream stream)
		{
			var buffer = new Rope<byte>();
			try {
				double progressInverse = (double)CHUNK_SIZE / stream.Length;
				int count = 0;
				
				byte[] byteBuffer = new byte[CHUNK_SIZE];
				
				using (stream) {
					int read = stream.Read(byteBuffer, 0, CHUNK_SIZE);
					while (read > 0) {
						buffer.AddRange(byteBuffer, 0, read);
						count++;
						if (!OnProgressUpdated(count * progressInverse)) return;
						if (count % 5 == 0) {
							var copy = buffer.Clone();
							SD.MainThread.InvokeAsyncAndForget(
								() => {
									this.buffer = copy;
									OnBufferChanged(EventArgs.Empty);
								}
							);
						}
						read = stream.Read(byteBuffer, 0, CHUNK_SIZE);
					}
				}
			} catch (OutOfMemoryException) {
				SD.MessageService.ShowErrorFormatted("${res:FileUtilityService.FileSizeTooBig}");
			} finally {
				SD.MainThread.InvokeAsyncAndForget(
					() => {
						this.buffer = buffer;
						OnBufferChanged(EventArgs.Empty);
					}
				);
			}
		}
		
		/// <summary>
		/// Writes all data to a stream.
		/// </summary>
		public void Save(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(buffer.ToArray());
			writer.Flush();
		}
		
		/// <summary>
		/// The size of the current buffer.
		/// </summary>
		public int BufferSize {
			get { return buffer.Count; }
		}
		
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
		
		public bool RemoveByte(int offset)
		{
			if ((offset < buffer.Count) & (offset > -1)) {
				buffer.RemoveAt(offset);
				file.MakeDirty(HexEditFileModelProvider.Instance);
				OnBufferChanged(EventArgs.Empty);
				return true;
			}
			return false;
		}
		
		public bool RemoveBytes(int offset, int length)
		{
			if (((offset < buffer.Count) && (offset > -1)) && ((offset + length) <= buffer.Count)) {
				buffer.RemoveRange(offset, length);
				file.MakeDirty(HexEditFileModelProvider.Instance);
				OnBufferChanged(EventArgs.Empty);
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
			file.MakeDirty(HexEditFileModelProvider.Instance);
			OnBufferChanged(EventArgs.Empty);
		}
		
		public void SetByte(int offset, byte @byte, bool overwrite)
		{
			if (overwrite) {
				if (offset > buffer.Count - 1) {
					buffer.Add(@byte);
				} else {
					buffer[offset] = @byte;
				}
			} else {
				buffer.Insert(offset, @byte);
			}
			file.MakeDirty(HexEditFileModelProvider.Instance);
			OnBufferChanged(EventArgs.Empty);
		}
	}
	
	
}
