// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ICSharpCode.Profiler.Interprocess
{
	// aliases for WIN32 types
	using WORD = System.Int16;
	using DWORD = System.Int32;
	using ULONG = System.UInt32;
	using LPVOID = System.IntPtr;
	using DWORD_PTR = System.IntPtr;
	using HANDLE = System.IntPtr;
	
	/// <summary>
	/// Represents a memory mapped file.
	/// </summary>
	public unsafe sealed class MemoryMappedFile : IDisposable
	{
		#region NativeMethods
		static class NativeMethods
		{
			[DllImport("kernel32", SetLastError = true, CharSet=CharSet.Unicode)]
			public static extern HANDLE CreateFileMapping(
				HANDLE hFile, IntPtr lpAttributes, int flProtect,
				uint dwMaximumSizeLow, uint dwMaximumSizeHigh,
				string lpName);
			
			[DllImport("kernel32", SetLastError = true)]
			public static extern LPVOID MapViewOfFile(
				HANDLE hFileMappingObject, DWORD dwDesiredAccess, uint dwFileOffsetHigh,
				uint dwFileOffsetLow, IntPtr dwNumBytesToMap);
			
			[DllImport("kernel32", SetLastError = true, CharSet=CharSet.Unicode)]
			public static extern HANDLE OpenFileMapping(DWORD dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);
			
			[DllImport("kernel32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool UnmapViewOfFile(LPVOID lpBaseAddress);
			
			[DllImport("kernel32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool CloseHandle(HANDLE handle);
			
			[DllImport("kernel32", SetLastError = true)]
			public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);
		}
		#endregion
		
		#region System Information
		struct SYSTEM_INFO
		{
			public WORD wProcessorArchitecture;
			public WORD wReserved;
			public DWORD dwPageSize;
			public LPVOID lpMinimumApplicationAddress;
			public LPVOID lpMaximumApplicationAddress;
			public DWORD_PTR dwActiveProcessorMask;
			public DWORD dwNumberOfProcessors;
			public DWORD dwProcessorType;
			public DWORD dwAllocationGranularity;
			public WORD wProcessorLevel;
			public WORD wProcessorRevision;
		}
		
		static volatile int systemPageSize = -1;
		static volatile int systemAllocationGranularity = -1;
		
		static SYSTEM_INFO GetSystemInfo()
		{
			SYSTEM_INFO info;
			NativeMethods.GetSystemInfo(out info);
			systemPageSize = info.dwPageSize;
			systemAllocationGranularity = info.dwAllocationGranularity;
			return info;
		}
		
		/// <summary>
		/// The page size and the granularity of page protection and commitment.
		/// </summary>
		public static int SystemPageSize {
			get {
				if (systemPageSize == -1) {
					return GetSystemInfo().dwPageSize;
				} else {
					return systemPageSize;
				}
			}
		}
		
		/// <summary>
		/// The granularity for the starting address at which virtual memory can be allocated.
		/// </summary>
		public static int SystemAllocationGranularity {
			get {
				if (systemAllocationGranularity == -1) {
					return GetSystemInfo().dwAllocationGranularity;
				} else {
					return systemAllocationGranularity;
				}
			}
		}
		#endregion
		
		const int PAGE_READONLY = 2;
		const int PAGE_READWRITE = 4;
		const int FILE_MAP_ALL_ACCESS = 0x001f;
		const int FILE_MAP_READ = 0x0004;
		const int FILE_MAP_WRITE = 0x0002;
		static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		
		/// <summary>
		/// Opens a file on disk as memory mapped file.
		/// </summary>
		public static MemoryMappedFile Open(string fileName, FileAccess access, FileShare share)
		{
			return Open(new FileStream(fileName, FileMode.Open, access, share));
		}
		
		/// <summary>
		/// Opens a file on disk as memory mapped file.
		/// </summary>
		public static MemoryMappedFile Open(FileStream fileStream)
		{
			if (fileStream == null)
				throw new ArgumentNullException("fileStream");
			long size = fileStream.Length;
			IntPtr memoryFileHandle = NativeMethods.CreateFileMapping(
				fileStream.SafeFileHandle.DangerousGetHandle(),
				IntPtr.Zero,
				fileStream.CanWrite ? PAGE_READWRITE : PAGE_READONLY,
				(uint)(size >> 32),
				(uint)(size & 0xFFFFFFFF),
				null);
			if (memoryFileHandle == IntPtr.Zero)
				throw new IOException("Creating Memory Mapped File failed.", GetLastError());
			return new MemoryMappedFile(memoryFileHandle, fileStream.CanWrite) { baseFileStream = fileStream };
		}
		
		/// <summary>
		/// Creates a new file mapping backed by the pagefile.
		/// </summary>
		/// <param name="name">The name of the file mapping object to be created, or null to create a nameless object.</param>
		/// <param name="size">The maximal size of the memory mapped file.</param>
		/// <returns>MemoryMappedFile object representing the file mapping</returns>
		public static MemoryMappedFile CreateSharedMemory(string name, long size)
		{
			if (size < 0)
				throw new ArgumentOutOfRangeException("size");
			
			IntPtr memoryFileHandle = NativeMethods.CreateFileMapping(
				INVALID_HANDLE_VALUE, // back shared memory in pagefile
				IntPtr.Zero,
				PAGE_READWRITE,
				(uint)(size >> 32),
				(uint)(size & 0xFFFFFFFF),
				name);
			if (memoryFileHandle == IntPtr.Zero)
				throw new IOException("Creating Memory Mapped File failed.", GetLastError());
			return new MemoryMappedFile(memoryFileHandle, true);
		}
		
		/// <summary>
		/// Opens an existing named file mapping.
		/// </summary>
		/// <param name="name">The name of the file mapping object to be opened.</param>
		/// <returns>MemoryMappedFile object representing the file mapping</returns>
		public static MemoryMappedFile Open(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			IntPtr memoryFileHandle = NativeMethods.OpenFileMapping(FILE_MAP_ALL_ACCESS, true, name);
			if (memoryFileHandle == IntPtr.Zero)
				throw new IOException("Opening Memory Mapped File failed.", GetLastError());
			return new MemoryMappedFile(memoryFileHandle, true);
		}
		
		static Exception GetLastError()
		{
			return Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
		}
		
		IntPtr memoryFileHandle;
		FileStream baseFileStream;
		bool canWrite;
		
		/// <summary>
		/// Creates a MemoryMappedFile object from a file mapping handle.
		/// </summary>
		public MemoryMappedFile(IntPtr memoryFileHandle, bool isWritable)
		{
			this.memoryFileHandle = memoryFileHandle;
			this.canWrite = isWritable;
		}
		/// <summary>
		/// Maps a section of the file into virtual memory.
		/// </summary>
		/// <param name="offset">The start position in the file from where to start mapping.</param>
		/// <param name="count">The number of bytes to map into virtual memory.</param>
		/// <returns>An UnmanagedMemory object representing the mapped memory.
		/// Dispose the UnmanagedMemory object to unmap the view.</returns>
		/// <remarks>This method may map more than the specified range
		/// when offset is not a multiple of SystemAllocationGranularity.
		/// In that case, the returned start pointer is an offset into the view,
		/// and is not aligned if offset is not aligned.</remarks>
		public UnmanagedMemory MapView(long offset, long count)
		{
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", offset, "Value must be non-negative");
			if (count <= 0)
				throw new ArgumentOutOfRangeException("count", count, "Value must be positive");
			int granularityOffset = (int)(offset % SystemAllocationGranularity);
			offset -= granularityOffset; // align offset
			IntPtr viewStart = NativeMethods.MapViewOfFile(
				memoryFileHandle,
				canWrite ? FILE_MAP_ALL_ACCESS : FILE_MAP_READ,
				(uint)(offset >> 32),
				(uint)(offset & 0xFFFFFFFF),
				new IntPtr(count + granularityOffset));
			if (viewStart == IntPtr.Zero)
				throw new IOException("Mapping view failed.", GetLastError());
			return new ViewMemory(viewStart, granularityOffset, count);
		}
		
		sealed class ViewMemory : UnmanagedMemory
		{
			readonly IntPtr viewStart;
			AtomicBoolean isDisposed;
			
			public ViewMemory(IntPtr viewStart, int granularityOffset, long length)
				: base(new IntPtr(viewStart.ToInt64() + granularityOffset), length)
			{
				this.viewStart = viewStart;
				GC.AddMemoryPressure(length);
			}
			
			public override void Dispose()
			{
				base.Dispose();
				
				if (isDisposed.Set()) {
					NativeMethods.UnmapViewOfFile(viewStart);
					GC.SuppressFinalize(this);
					GC.RemoveMemoryPressure(Length);
				}
			}
			
			~ViewMemory()
			{
				Dispose();
			}
		}
		
		#region Close
		/// <summary>
		/// Closes and disposes the memory used by the file mapping.
		/// </summary>
		public void Close()
		{
			Dispose(true);
		}
		
		/// <summary>
		/// Disposes the memery used by the file mapping.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
		
		void Dispose(bool disposing)
		{
			// thread-safe dispose implementation
			IntPtr handle = Interlocked.Exchange(ref memoryFileHandle, IntPtr.Zero);
			if (handle != IntPtr.Zero) {
				NativeMethods.CloseHandle(handle);
			}
			if (disposing) {
				if (baseFileStream != null)
					baseFileStream.Dispose();
				GC.SuppressFinalize(this);
			}
		}
		
		/// <summary>
		/// Disposes the memery used by the file mapping.
		/// </summary>
		~MemoryMappedFile()
		{
			Dispose(false);
		}
		#endregion
	}
}
