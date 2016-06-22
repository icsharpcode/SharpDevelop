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

namespace ICSharpCode.Profiler.Interprocess
{
	/// <summary>
	/// Describes a chunk of unmanaged memory combined with a deallocation strategy.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification="We don't need a finalizer in this class - derived classes should add the finalizer if they use unmanaged resources")]
	public unsafe class UnmanagedMemory : IDisposable
	{
		readonly IntPtr start;
		readonly long length;
		
		/// <summary>
		/// A byte pointer pointing at the start of the unmanaged memory.
		/// </summary>
		[CLSCompliant(false)]
		public byte* Pointer {
			get { return (byte*)start.ToPointer(); }
		}
		
		/// <summary>
		/// An IntPtr pointing at the start of the unmanaged memory. 
		/// </summary>
		public IntPtr Start {
			get { return start; }
		}
		
		/// <summary>
		/// The size of the the unmanaged memory.
		/// </summary>
		public long Length {
			get { return length; }
		}
		
		/// <summary>
		/// Creates a new UnmanagedMemory instance with the specified pointer and length values.
		/// Disposing such a UnmanagedMemory instance has no effect.
		/// </summary>
		public UnmanagedMemory(IntPtr start, long length)
		{
			if (length < 0)
				throw new ArgumentOutOfRangeException("length");
			this.start = start;
			this.length = length;
		}
		
		/// <summary>
		/// Frees the unmanaged memory.
		/// This causes pointers inside this UnmanagedMemory to become invalid (including
		/// the UnmanagedMemoryStream).
		/// </summary>
		public virtual void Dispose()
		{
		}
		
		/// <summary>
		/// Creates a stream for reading or writing to the unmanaged memory.
		/// </summary>
		/// <returns></returns>
		[CLSCompliant(false)]
		public UnmanagedMemoryStream CreateStream()
		{
			return new UnmanagedMemoryStream(Pointer, Length, Length, FileAccess.ReadWrite);
		}
	}
}
