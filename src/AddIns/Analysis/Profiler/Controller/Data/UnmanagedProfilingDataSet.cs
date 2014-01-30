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

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// The unmanaged representation of a profiling dataset.
	/// </summary>
	/// <remarks>
	/// All instance members except for <see cref="Dispose"/> are thread-safe.
	/// Accessing a CallTreeNode of a disposed UnmanagedProfilingDataSet is undefined behaviour.
	/// Usually you'll get an ObjectDisposedException, but if you're unlucky, you'll get
	/// a different exception/garbage data/nasal demons.
	/// </remarks>
	public unsafe abstract class UnmanagedProfilingDataSet : IProfilingDataSet, IDisposable
	{
		TargetProcessPointer nativeStartPosition;
		TargetProcessPointer nativeRootFuncInfoPosition;
		bool isDisposed;
		bool isFirst;
		bool is64Bit;
		
		/// <summary>
		/// Gets whether the dataset is using 8-byte (64-bit) or 4-byte (32-bit) pointers.
		/// </summary>
		public bool Is64Bit {
			get { return is64Bit; }
		}
		
		long length;
		IntPtr startPtr;
		
		internal TargetProcessPointer NativeStartPosition {
			get { return nativeStartPosition; }
		}

		
		internal TargetProcessPointer NativeRootFuncInfoPosition {
			get { return nativeRootFuncInfoPosition; }
		}

		/// <summary>
		/// Returns the length of the DataSet.
		/// </summary>
		public long Length {
			get { return length; }
		}
		
		/// <summary>
		/// Returns a pointer to the start of the dataset in memory.
		/// </summary>
		public IntPtr StartPtr {
			get { return startPtr; }
		}

		internal abstract unsafe void *TranslatePointer(TargetProcessPointer ptr);
		
		/// <summary>
		/// Returns the processor frequency of the computer, where the DataSet was created.
		/// </summary>
		public abstract int ProcessorFrequency {
			get;
		}
		
		/// <summary>
		/// Returns a NameMapping for a nameId.
		/// </summary>
		public abstract NameMapping GetMapping(int nameId);
		
		internal UnmanagedProfilingDataSet(TargetProcessPointer nativeStartPosition, TargetProcessPointer nativeRootFuncInfoPosition,
		                                   byte *startPtr, long length, bool isFirst, bool is64Bit)
		{
			this.nativeStartPosition = nativeStartPosition;
			this.nativeRootFuncInfoPosition = nativeRootFuncInfoPosition;
			this.is64Bit = is64Bit;
			this.isFirst = isFirst;
			this.startPtr = new IntPtr(startPtr);
			this.length = length;
		}

		internal unsafe FunctionInfo* GetFunctionInfo(TargetProcessPointer ptr)
		{
			return (FunctionInfo*)TranslatePointer(ptr);
		}
		
		internal unsafe FunctionInfo* GetRootFunctionInfo()
		{
			return GetFunctionInfo(nativeRootFuncInfoPosition);
		}
		
		/// <summary>
		/// Creates a new CallTreeNode from unmanaged data.
		/// </summary>
		public unsafe CallTreeNode RootNode {
			get {
				VerifyAccess();
				if (is64Bit)
					return new UnmanagedCallTreeNode64(
						this,
						GetRootFunctionInfo(),
						null);
				else
					return new UnmanagedCallTreeNode32(
						this,
						GetRootFunctionInfo(),
						null);
			}
		}
		
		// TODO: maybe only in debug builds?
		// [Conditional("DEBUG")]
		
		/// <summary>
		/// Verifies that the data set still exists (unmanaged memory is not disposed).
		/// </summary>
		internal void VerifyAccess()
		{
			if (isDisposed)
				throw new ObjectDisposedException("UnmanagedProfilingDataSet");
		}
		
		/// <summary>
		/// Disposes the dataset.
		/// </summary>
		public virtual void Dispose()
		{
			isDisposed = true;
		}
		
		/// <inheritdoc/>
		public bool IsFirst {
			get {
				return isFirst;
			}
		}
	}
}
