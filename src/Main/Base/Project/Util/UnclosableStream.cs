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

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Wraps another stream. Closing this stream does not close the base stream.
	/// </summary>
	public class UnclosableStream : Stream
	{
		Stream baseStream;
		
		public UnclosableStream(Stream baseStream)
		{
			if (baseStream == null)
				throw new ArgumentNullException("baseStream");
			this.baseStream = baseStream;
		}
		
		public override bool CanRead {
			get { return baseStream.CanRead; }
		}
		
		public override bool CanSeek {
			get { return baseStream.CanSeek; }
		}
		
		public override bool CanWrite {
			get { return baseStream.CanWrite; }
		}
		
		public override long Length {
			get { return baseStream.Length; }
		}
		
		public override long Position {
			get { return baseStream.Position; }
			set { baseStream.Position = value; }
		}
		
		public override void Flush()
		{
			baseStream.Flush();
		}
		
		public override long Seek(long offset, SeekOrigin origin)
		{
			return baseStream.Seek(offset, origin);
		}
		
		public override void SetLength(long value)
		{
			baseStream.SetLength(value);
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			return baseStream.Read(buffer, offset, count);
		}
		
		public override void Write(byte[] buffer, int offset, int count)
		{
			baseStream.Write(buffer, offset, count);
		}
		
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return baseStream.BeginRead(buffer, offset, count, callback, state);
		}
		
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return baseStream.BeginWrite(buffer, offset, count, callback, state);
		}
		
		public override bool CanTimeout {
			get { return baseStream.CanTimeout; }
		}
		
		public override int EndRead(IAsyncResult asyncResult)
		{
			return baseStream.EndRead(asyncResult);
		}
		
		public override void EndWrite(IAsyncResult asyncResult)
		{
			baseStream.EndWrite(asyncResult);
		}
		
		public override int ReadByte()
		{
			return baseStream.ReadByte();
		}
		
		public override int ReadTimeout {
			get { return baseStream.ReadTimeout; }
			set { baseStream.ReadTimeout = value; }
		}
		
		public override void WriteByte(byte value)
		{
			baseStream.WriteByte(value);
		}
		
		public override int WriteTimeout {
			get { return baseStream.WriteTimeout; }
			set { baseStream.WriteTimeout = value; }
		}
	}
}
