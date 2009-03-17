// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using HKEY = System.UIntPtr;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Provides access to 32-bit Windows Registry on 32-bit systems and 32-bit and 64-bit views of the Windows Registry on 64-bit systems.
	/// </summary>
	public class ExtendedRegistry
	{
		static class NativeMethods {
//			internal static readonly UIntPtr HKEY_CLASSES_ROOT = new UIntPtr(0x80000000);
			internal static readonly UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001);
			internal static readonly UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002);
//			internal static readonly UIntPtr HKEY_USERS = new UIntPtr(0x80000003);
//			internal static readonly UIntPtr HKEY_PERFORMANCE_DATA = new UIntPtr(0x80000004);
//			internal static readonly UIntPtr HKEY_CURRENT_CONFIG = new UIntPtr(0x80000005);
//			internal static readonly UIntPtr HKEY_DYN_DATA = new UIntPtr(0x80000006);
			
//			[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
//			internal static extern int RegOpenKeyEx(HKEY hKey, string subKey, uint options, RegSAM samDesired, out HKEY hKeyResult);
			
			[DllImport("advapi32.dll")]
			internal static extern int RegCloseKey(HKEY hKey);
			
//			[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
//			internal static extern int RegQueryValueEx(HKEY hkey, string valueName, IntPtr reserved, out uint type, IntPtr buffer, ref uint bufferLength);
			
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
			internal static extern int RegCreateKeyEx(HKEY hkey, string subKey, uint reserved, string lpClass, uint options, RegSAM samDesired, IntPtr securityAttributes, out HKEY result, out uint disposition);
			
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
			internal static extern int RegDeleteKeyEx(HKEY hKey, string subKey, RegSAM samDesired, uint reserved);
			
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
			internal static extern int RegDeleteKey(HKEY hKey, string subKey);
			
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
			internal static extern int RegSetValueEx(HKEY hkey, string valueName, uint reserved, uint type, IntPtr data, uint dataLength);
			
			[DllImport("kernel32")]
			internal static extern IntPtr GetCurrentProcess();
			
			[DllImport("kernel32", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);
			
			[Flags]
			internal enum RegOption
			{
				NonVolatile = 0x0,
				Volatile = 0x1,
				CreateLink = 0x2,
				BackupRestore = 0x4,
				OpenLink = 0x8
			}
			
			[Flags]
			internal enum RegSAM
			{
				QueryValue = 0x0001,
				SetValue = 0x0002,
				CreateSubKey = 0x0004,
				EnumerateSubKeys = 0x0008,
				Notify = 0x0010,
				CreateLink = 0x0020,
				WOW64_32Key = 0x0200,
				WOW64_64Key = 0x0100,
				Read = 0x00020019,
				Write = 0x00020006,
				Execute = 0x00020019,
				AllAccess = 0x000f003f
			}
			
			internal const uint REG_SZ = 0x1;
			
			internal enum RegResult
			{
				CreatedNewKey = 0x00000001,
				OpenedExistingKey = 0x00000002
			}
		}
		
		UIntPtr rootKey;
		NativeMethods.RegSAM keyType;
		
		ExtendedRegistry(UIntPtr rootKey, NativeMethods.RegSAM keyType) {
			this.rootKey = rootKey;
			this.keyType = keyType;
		}
		
		/// <summary>
		/// Sets a value in a key. if the key does not exist it is created.
		/// </summary>
		/// <param name="subkey">The key, where to insert the value.</param>
		/// <param name="valueName">The name of the value, use null for "(Default)".</param>
		/// <param name="value">The data of the value.</param>
		public void SetValue(string subkey, string valueName, string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if (subkey == null)
				throw new ArgumentNullException("subkey");
			
			UIntPtr keyPtr;
			uint dummy;
			IntPtr ptr = IntPtr.Zero;
			int errorCode = NativeMethods.RegCreateKeyEx(rootKey, subkey, 0x0, null, 0x0, NativeMethods.RegSAM.CreateSubKey | NativeMethods.RegSAM.SetValue | keyType, IntPtr.Zero, out keyPtr, out dummy);
			try {
				if (errorCode != 0)
					throw ExceptionForError(errorCode);
				ptr = Marshal.StringToHGlobalUni(value);
				errorCode = NativeMethods.RegSetValueEx(keyPtr, valueName, 0, NativeMethods.REG_SZ, ptr, (uint)((value.Length + 1) * 2));
				if (errorCode != 0)
					throw ExceptionForError(errorCode);
			} finally {
				if (ptr != null)
					Marshal.FreeHGlobal(ptr);
				errorCode = NativeMethods.RegCloseKey(keyPtr);
			}
			// check error code from RegCloseKey (but only if there was no other exception)
			if (errorCode != 0)
				throw ExceptionForError(errorCode);
		}
		
		static Exception ExceptionForError(int code)
		{
			switch (code) {
				case 5:
					return new UnauthorizedAccessException();
				default:
					return new Win32Exception(code);
			}
		}
		
		/// <summary>
		/// Deletes a key in the Windows Registry. The key must have no sub keys. All values of the key are deleted.
		/// </summary>
		/// <param name="subkey">The subkey to delete.</param>
		public void DeleteSubkey(string subkey)
		{
			if (subkey == null)
				throw new ArgumentNullException("subkey");
			int retVal = (Environment.OSVersion.Version >= new Version(5,2))
				? NativeMethods.RegDeleteKeyEx(rootKey, subkey, keyType, 0)
				: NativeMethods.RegDeleteKey(rootKey, subkey);
			if (retVal != 0 && retVal != 2) // 2=key not found
				throw ExceptionForError(retVal);
		}
		
		/// <summary>
		/// Gets the 32-bit view of HKEY_CURRENT_USER.
		/// </summary>
		public static ExtendedRegistry CurrentUser32 {
			get { return new ExtendedRegistry(ExtendedRegistry.NativeMethods.HKEY_CURRENT_USER, NativeMethods.RegSAM.WOW64_32Key); }
		}
		
		/// <summary>
		/// Gets the 32-bit view of HKEY_LOCAL_MACHINE.
		/// </summary>
		public static ExtendedRegistry LocalMachine32 {
			get { return new ExtendedRegistry(ExtendedRegistry.NativeMethods.HKEY_LOCAL_MACHINE, NativeMethods.RegSAM.WOW64_32Key); }
		}
		
		/// <summary>
		/// Gets the 64-bit view of HKEY_CURRENT_USER.
		/// </summary>
		public static ExtendedRegistry CurrentUser64 {
			get { return new ExtendedRegistry(ExtendedRegistry.NativeMethods.HKEY_CURRENT_USER, NativeMethods.RegSAM.WOW64_64Key); }
		}
		
		/// <summary>
		/// Gets the 64-bit view of HKEY_LOCAL_MACHINE.
		/// </summary>
		public static ExtendedRegistry LocalMachine64 {
			get { return new ExtendedRegistry(ExtendedRegistry.NativeMethods.HKEY_LOCAL_MACHINE, NativeMethods.RegSAM.WOW64_64Key); }
		}
		
		/// <summary>
		/// Gets whether the OS is 64-bit Windows.
		/// </summary>
		public static bool Is64BitWindows {
			get {
				// this is a 64-bit process -> Windows is 64-bit
				if (IntPtr.Size == 8)
					return true;
				
				// this is a 32-bit process -> we need to check whether we run in Wow64 emulation
				bool is64Bit;
				if (NativeMethods.IsWow64Process(NativeMethods.GetCurrentProcess(), out is64Bit)) {
					return is64Bit;
				} else {
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
		}
	}
}
