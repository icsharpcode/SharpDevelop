// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Debugger.Interop.CorDebug;

namespace Debugger.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MEMORY_BASIC_INFORMATION
	{
		public IntPtr BaseAddress;
		public IntPtr AllocationBase;
		public uint AllocationProtect;
		public IntPtr RegionSize;
		public uint State;
		public uint Protect;
		public uint Type;
	}
	
	[Flags]
	public enum ProcessAccessFlags : uint
	{
		All = 0x001F0FFF,
		Terminate = 0x00000001,
		CreateThread = 0x00000002,
		VMOperation = 0x00000008,
		VMRead = 0x00000010,
		VMWrite = 0x00000020,
		DupHandle = 0x00000040,
		SetInformation = 0x00000200,
		QueryInformation = 0x00000400,
		Synchronize = 0x00100000
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct SYSTEM_INFO
	{
		internal _PROCESSOR_INFO_UNION uProcessorInfo;
		public uint dwPageSize;
		public IntPtr lpMinimumApplicationAddress;
		public IntPtr lpMaximumApplicationAddress;
		public IntPtr dwActiveProcessorMask;
		public uint dwNumberOfProcessors;
		public uint dwProcessorType;
		public uint dwAllocationGranularity;
		public ushort dwProcessorLevel;
		public ushort dwProcessorRevision;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct _PROCESSOR_INFO_UNION
	{
		[FieldOffset(0)]
		internal uint dwOemId;
		[FieldOffset(0)]
		internal ushort wProcessorArchitecture;
		[FieldOffset(2)]
		internal ushort wReserved;
	}
	
	[Flags]
	public enum AllocationType
	{
		Commit = 0x1000,
		Reserve = 0x2000,
		Decommit = 0x4000,
		Release = 0x8000,
		Reset = 0x80000,
		Physical = 0x400000,
		TopDown = 0x100000,
		WriteWatch = 0x200000,
		LargePages = 0x20000000
	}

	[Flags]
	public enum MemoryProtection
	{
		Execute = 0x10,
		ExecuteRead = 0x20,
		ExecuteReadWrite = 0x40,
		ExecuteWriteCopy = 0x80,
		NoAccess = 0x01,
		ReadOnly = 0x02,
		ReadWrite = 0x04,
		WriteCopy = 0x08,
		GuardModifierflag = 0x100,
		NoCacheModifierflag = 0x200,
		WriteCombineModifierflag = 0x400
	}
	
	public static class NativeMethods
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
		public static extern Debugger.Interop.CorDebug.ICorDebug CreateDebuggingInterfaceFromVersion(int debuggerVersion, string debuggeeVersion);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
		public static extern int GetCORVersion([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName, Int32 cchBuffer, out Int32 dwLength);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
		public static extern int GetRequestedRuntimeVersion(string exeFilename, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pVersion, Int32 cchBuffer, out Int32 dwLength);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualQueryEx(IntPtr hProcess,
		                                         IntPtr lpAddress,
		                                         out MEMORY_BASIC_INFORMATION lpBuffer,
		                                         uint dwLength);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
		                                           UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			[Out] byte[] lpBuffer,
			int dwSize,
			out int lpNumberOfBytesRead
		);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

		public static List<Tuple<long, long>> GetVirtualMemoryAddresses(this Process process)
		{
			var result = new List<Tuple<long, long>>();
			SYSTEM_INFO sysinfo = new SYSTEM_INFO();
			GetSystemInfo(out sysinfo);
			
			long address = 0;
			MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();
			IntPtr openedProcess = IntPtr.Zero;
			try {
				openedProcess = OpenProcess(ProcessAccessFlags.All, false, (int)process.Id);
				
				while (address < sysinfo.lpMaximumApplicationAddress.ToInt64())
				{
					try {
						if (!VirtualQueryEx(openedProcess, new IntPtr(address), out m, (uint)Marshal.SizeOf(m)))
							continue;
					} finally {
						// next address
						address = m.BaseAddress.ToInt64() + m.RegionSize.ToInt64();
					}
					
					result.Add(new Tuple<long, long>(m.BaseAddress.ToInt64(), m.RegionSize.ToInt64()));
				}
			} finally {
				if (openedProcess != IntPtr.Zero)
					CloseHandle(openedProcess);
			}
			
			return result;
		}
		
		public static byte[] ReadProcessMemory(this Process process, long startAddress, long size)
		{
			IntPtr openedProcess = IntPtr.Zero;
			byte[] temp = null;
			try {
				temp = new byte[size];
				openedProcess = OpenProcess(ProcessAccessFlags.All, false, (int)process.Id);

				int outSize;
				bool success = ReadProcessMemory(openedProcess, new IntPtr(startAddress), temp, temp.Length, out outSize);
				
				if (!success || outSize == 0) {
					var proc = System.Diagnostics.Process.GetProcessById((int)process.Id);
					return process.ReadProcessMemory(proc.MainModule.BaseAddress.ToInt64(), (long)4096);
				}
			} catch {
				return null;
			} finally {
				if (openedProcess != IntPtr.Zero)
					CloseHandle(openedProcess);
			}
			
			return temp;
		}
	}
}

#pragma warning restore 1591
