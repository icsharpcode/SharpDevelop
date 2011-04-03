// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#pragma warning disable 1591

using System;
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
	
	public static class NativeMethods
	{
		[DllImport("kernel32.dll")]
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
		
		public static byte[] ReadProcessMemory(this Process process, out long baseAddress)
		{
			uint handle = process.CorProcess.GetHandle();
			
			var proc = System.Diagnostics.Process.GetProcessById((int)process.Id);
			baseAddress = proc.MainModule.BaseAddress.ToInt64();
			long addr = baseAddress;
			
			byte[] memory = null;
			
			while (true)
			{
				byte[] temp = new byte[1024];
				int outSize;
				bool success = ReadProcessMemory(new IntPtr(handle), new IntPtr(addr), temp, temp.Length, out outSize);
				
				addr += 1024;
				
				if (outSize == 0)
					break;
				
				if (memory == null) {
					memory = new byte[outSize];
					Array.Copy(temp, memory, outSize);
				} else {
					// expand memory
					byte[] newTemp = new byte[memory.Length];
					Array.Copy(memory, newTemp, memory.Length);
					
					memory = new byte[memory.Length + outSize];
					Array.Copy(newTemp, memory, newTemp.Length);
					Array.Copy(temp, 0, memory, newTemp.Length, outSize);
				}
				
				if (!success) // break when we cannot read anymore
					break;
			}
			
			return memory;
		}
	}
}

#pragma warning restore 1591
