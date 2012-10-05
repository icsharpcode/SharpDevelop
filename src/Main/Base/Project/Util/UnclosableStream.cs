// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
