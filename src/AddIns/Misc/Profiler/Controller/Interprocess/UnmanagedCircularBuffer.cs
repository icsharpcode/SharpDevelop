// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ICSharpCode.Profiler.Interprocess
{
	/// <summary>
	/// A circular buffer residing in unmanaged memory.
	/// This class can be used to create communication streams in shared memory.
	/// </summary>
	/// <remarks>
	/// The performance isn't perfect, there's a problem that sometimes causes transmission with a fast writer to
	/// go like this:
	/// UnmanagedCircularBuffer: write amount 33 (max-1)
	/// UnmanagedCircularBuffer: write amount 1
	/// UnmanagedCircularBuffer: write amount 33 (max-1)
	/// UnmanagedCircularBuffer: write amount 1
	/// UnmanagedCircularBuffer: write amount 33 (max-1)
	/// UnmanagedCircularBuffer: write amount 1
	/// UnmanagedCircularBuffer: write amount 9
	/// Assume the buffer is full, and the reader read until the buffer wrap-around position.
	/// Then the writer will write to wrap-around - 1 (because we don't allow a completely full buffer - we couldn't
	/// distinguish it from a completely empty buffer). Then the reader will read some more bytes.
	/// Then the writer will write just one byte (to the wrap-around position).
	/// Either writer or reader should learn to read/write over the wrap-around position in a single operation
	/// to prevent 1 byte-writes from occurring so frequently.
	/// But this is only a problem when transmitting large amounts of data through a small buffer -
	/// making the buffer large enough also solves the problem.
	/// </remarks>
	public unsafe sealed class UnmanagedCircularBuffer : IDisposable
	{
		[StructLayout(LayoutKind.Sequential)]
		struct UnmanagedCircularBufferHeader
		{
			public int Magic;       // Verification value - always '~CB1'
			public int TotalLength; // Length including the header
			public volatile int startOffset; // only the reader is writing to this variable
			public volatile int endOffset; // only the writer is writing to this variable
			public int NonEmptyEventName;
			public int NonFullEventName;
		}
		
		UnmanagedCircularBufferHeader *header;
		byte* data;
		int dataLength;
		EventWaitHandle nonEmptyEvent;
		EventWaitHandle nonFullEvent;
		
		/// <summary>
		/// Gets the size of the buffer header.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
		public static readonly int SynchronizationOverheadSize = sizeof(UnmanagedCircularBufferHeader);
		
		#region Construction
		private UnmanagedCircularBuffer(IntPtr bufferPointer, int bufferLength)
		{
			// we need space for the header and at least 2 bytes of data (because the buffer can never be completely filled)
			if (bufferLength < sizeof(UnmanagedCircularBufferHeader) + 2)
				throw new ArgumentOutOfRangeException("bufferLength", "bufferLength is too small to hold the header");
			
			this.header = (UnmanagedCircularBufferHeader*)bufferPointer.ToPointer();
			this.data = (byte*)(header + 1);
			this.dataLength = bufferLength - sizeof(UnmanagedCircularBufferHeader);
		}
		
		[ThreadStatic] static Random rnd;
		
		/// <summary>
		/// Creates a new circular buffer.
		/// </summary>
		/// <param name="buffer">Position where the circular buffer should be created</param>
		/// <param name="bufferLength">Length of the circular buffer. Not the whole length is available
		/// for data, some space will be used for synchronization.</param>
		/// <returns>NativeCircularBuffer object representing the circular buffer</returns>
		public static UnmanagedCircularBuffer Create(IntPtr buffer, int bufferLength)
		{
			UnmanagedCircularBuffer b = new UnmanagedCircularBuffer(buffer, bufferLength);
			b.header->Magic = 0x7e434231; // '~CB1'
			b.header->TotalLength = bufferLength;
			b.header->startOffset = 0;
			b.header->endOffset = 0;
			b.nonEmptyEvent = CreateNew(ref b.header->NonEmptyEventName, false);
			b.nonFullEvent = CreateNew(ref b.header->NonFullEventName, true);
			Thread.MemoryBarrier(); // ensure NonEmptyEventName and NonFullEventName are flushed to RAM
			return b;
		}
		
		/// <summary>
		/// Creates a new AutoResetEvent using the random number generator.
		/// </summary>
		static EventWaitHandle CreateNew(ref int eventName, bool initialValue)
		{
			if (rnd == null)
				rnd = new Random();
			EventWaitHandle ev = null;
			while (ev == null) {
				eventName = rnd.Next();
				bool createdNew;
				ev = new EventWaitHandle(initialValue, EventResetMode.AutoReset, GetEventName(eventName), out createdNew);
				if (!createdNew) {
					Debug.WriteLine("Collision on name creation");
					ev.Close();
					ev = null;
				}
			}
			
			return ev;
		}
		
		/// <summary>
		/// Opens on existing circular buffer.
		/// </summary>
		/// <param name="buffer">Position of the circular buffer</param>
		/// <returns>NativeCircularBuffer object representing the circular buffer</returns>
		public static UnmanagedCircularBuffer Open(IntPtr buffer)
		{
			UnmanagedCircularBufferHeader* header = (UnmanagedCircularBufferHeader*)buffer.ToPointer();
			if (header->Magic != 0x7e434231) {
				throw new InvalidDataException("Can not open cicular buffer. Invalid header.");
			}
			int totalLength = header->TotalLength;
			UnmanagedCircularBuffer b = new UnmanagedCircularBuffer(buffer, totalLength);
			b.nonEmptyEvent = EventWaitHandle.OpenExisting(GetEventName(b.header->NonEmptyEventName));
			b.nonFullEvent = EventWaitHandle.OpenExisting(GetEventName(b.header->NonFullEventName));
			return b;
		}
		
		static string GetEventName(int eventName)
		{
			return "Local\\Profiler.Event." + eventName.ToString("d", CultureInfo.InvariantCulture.NumberFormat);
		}
		#endregion
		
		#region Instance methods
		/// <summary>
		/// Constructs a reading stream for the circular buffer. A buffer may have only one reader!
		/// </summary>
		public Stream CreateReadingStream()
		{
			return new ReadingStream(this);
		}
		
		/// <summary>
		/// Constructs a reading stream for the circular buffer. A buffer may have only one writer!
		/// </summary>
		public Stream CreateWritingStream()
		{
			return new WritingStream(this);
		}
		
		readonly object closeLock = new object();
		bool isClosed;
		
		/// <summary>
		/// Closes the circular buffer.
		/// </summary>
		public void Close()
		{
			lock (closeLock) {
				if (isClosed)
					return;
				isClosed = true;
				nonEmptyEvent.Set();
				Monitor.Wait(closeLock);
			}
			nonEmptyEvent.Close();
			nonFullEvent.Close();
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		void IDisposable.Dispose()
		{
			Close();
		}
		#endregion
		
		#region BaseStream for reader and writer
		abstract class BaseStream : Stream
		{
			protected UnmanagedCircularBufferHeader *header;
			protected UnmanagedCircularBuffer circularBuffer;
			
			public BaseStream(UnmanagedCircularBuffer circularBuffer)
			{
				this.circularBuffer = circularBuffer;
				this.header = circularBuffer.header;
			}
			
			public override bool CanSeek {
				get { return false; }
			}
			
			public override bool CanRead {
				get { return false; }
			}
			
			public override bool CanWrite {
				get { return false; }
			}
			
			public override long Length {
				get { return circularBuffer.dataLength; }
			}
			
			public override long Position {
				get { throw new NotSupportedException(); }
				set { throw new NotSupportedException(); }
			}
			
			public override void Flush()
			{
				Thread.MemoryBarrier();
			}
			
			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}
			
			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}
			
			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}
			
			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}
		}
		#endregion
		
		#region Reading
		/// <summary>
		/// Implements a stream reading a circular buffer in unmanaged memory.
		/// </summary>
		sealed class ReadingStream : BaseStream
		{
			/// <summary>
			/// Constructs a UnmanagedCircularBufferReadingStream
			/// </summary>
			public ReadingStream(UnmanagedCircularBuffer circularBuffer)
				: base(circularBuffer)
			{
				this.ReadTimeout = Timeout.Infinite;
			}
			
			public override int Read(byte[] buffer, int offset, int count)
			{
				if (buffer == null)
					throw new ArgumentNullException("buffer");
				if (count < 0)
					throw new ArgumentOutOfRangeException("count", "count is negative");
				if (offset < 0 || offset + count > buffer.Length)
					throw new ArgumentOutOfRangeException("offset", offset, "offset is out of range");
				
				// The code below requires reading at least one byte
				if (count == 0)
					return 0;
				
				// read startOffset and endOffset from shared memory
				int startOffset = header->startOffset;
				int endOffset = header->endOffset;
				// wait until there's data
				while (startOffset == endOffset) {
					lock (circularBuffer.closeLock) {
						if (this.circularBuffer.isClosed) {
							Monitor.Pulse(circularBuffer.closeLock);
							return 0;
						}
					}
					
					if (!circularBuffer.nonEmptyEvent.WaitOne(ReadTimeout, false))
						throw new TimeoutException();
					
					// the writer should have changed the end offset
					endOffset = header->endOffset;
				}
				
				if (endOffset < startOffset) {
					// data wraps over the buffer end => read only until buffer end
					endOffset = circularBuffer.dataLength; // change only our local copy of endOffset
				}
				int readCount = Math.Min(count, endOffset - startOffset);
				Debug.Assert(readCount > 0); // we must be reading something
				
				Marshal.Copy(new IntPtr(circularBuffer.data + startOffset), buffer, offset, readCount);
				
				startOffset += readCount; 	   // advance startOffset
				if (startOffset == circularBuffer.dataLength) // wrap around startOffset if required
					startOffset = 0;
				Debug.Assert(startOffset < circularBuffer.dataLength);
				header->startOffset = startOffset; // write back startOffset to shared memory
				circularBuffer.nonFullEvent.Set(); // we read something, so the buffer is not full anymore
				return readCount;
			}
			
			public override bool CanRead {
				get { return true; }
			}
			
			public override int ReadTimeout { get; set; }
		}
		#endregion

		#region Writing
		/// <summary>
		/// Implements a stream writing to a circular buffer in unmanaged memory.
		/// </summary>
		sealed class WritingStream : BaseStream
		{
			/// <summary>
			/// Constructs a NativeCircularBufferWritingStream
			/// </summary>
			public WritingStream(UnmanagedCircularBuffer circularBuffer)
				: base(circularBuffer)
			{
				this.WriteTimeout = Timeout.Infinite;
			}
			
			public override void Write(byte[] buffer, int offset, int count)
			{
				if (buffer == null)
					throw new ArgumentNullException("buffer");
				if (count < 0)
					throw new ArgumentOutOfRangeException("count", "count is negative");
				if (offset < 0 || offset + count > buffer.Length)
					throw new ArgumentOutOfRangeException("offset", offset, "offset is out of range");
				
				while (count > 0) {
					int r = WriteInternal(buffer, offset, count);
					offset += r;
					count -= r;
				}
			}
			
			int WriteInternal(byte[] buffer, int offset, int count)
			{
				// read startOffset and endOffset from shared memory
				int startOffset = header->startOffset;
				int endOffset = header->endOffset;
				// wait until there's room
				while (NextOffset(endOffset) == startOffset) {
					if (!circularBuffer.nonFullEvent.WaitOne(WriteTimeout, false))
						throw new TimeoutException();
					// the reader should have changed the start offset
					startOffset = header->startOffset;
				}
				int writeEndOffset;
				if (startOffset <= endOffset) {
					// free space wraps over buffer end
					if (startOffset == 0)
						writeEndOffset = circularBuffer.dataLength - 1; // one byte must always be left free
					else
						writeEndOffset = circularBuffer.dataLength;
				} else {
					writeEndOffset = startOffset - 1; // one byte must be left free to distinguish between empty and full buffer
				}
				int writeCount = Math.Min(count, writeEndOffset - endOffset);
				Debug.Assert(writeCount > 0);
				
				Debug.WriteLine("UnmanagedCircularBuffer: write amount " + writeCount);
				
				Marshal.Copy(buffer, offset, new IntPtr(circularBuffer.data + endOffset), writeCount);
				endOffset += writeCount; // advance endOffset
				if (endOffset == circularBuffer.dataLength) // wrap around startOffset if required
					endOffset = 0;
				Debug.Assert(endOffset < circularBuffer.dataLength);
				header->endOffset = endOffset; // write back endOffset to shared memory
				circularBuffer.nonEmptyEvent.Set(); // we wrote something, so the buffer is not empty anymore
				return writeCount;
			}
			
			int NextOffset(int offset)
			{
				offset += 1;
				if (offset == circularBuffer.dataLength)
					return 0;
				else
					return offset;
			}
			
			public override int WriteTimeout { get; set; }
			
			public override bool CanWrite {
				get { return true; }
			}
		}
		#endregion
	}
}
