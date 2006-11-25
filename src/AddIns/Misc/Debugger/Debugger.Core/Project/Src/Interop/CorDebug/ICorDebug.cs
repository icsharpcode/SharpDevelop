// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorDebug
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF"), InterfaceType((short) 1)]
    public interface ICorDebug
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Initialize();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Terminate();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetManagedHandler([In, MarshalAs(UnmanagedType.Interface)] ICorDebugManagedCallback pCallback);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetUnmanagedHandler([In, MarshalAs(UnmanagedType.Interface)] ICorDebugUnmanagedCallback pCallback);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateProcess([In, MarshalAs(UnmanagedType.LPWStr)] string lpApplicationName, [In, MarshalAs(UnmanagedType.LPWStr)] string lpCommandLine, [In] ref _SECURITY_ATTRIBUTES lpProcessAttributes, [In] ref _SECURITY_ATTRIBUTES lpThreadAttributes, [In] int bInheritHandles, [In] uint dwCreationFlags, [In] IntPtr lpEnvironment, [In, MarshalAs(UnmanagedType.LPWStr)] string lpCurrentDirectory, [In, ComAliasName("Debugger.Interop.CorDebug.ULONG_PTR")] uint lpStartupInfo, [In, ComAliasName("Debugger.Interop.CorDebug.ULONG_PTR")] uint lpProcessInformation, [In] CorDebugCreateProcessFlags debuggingFlags, [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DebugActiveProcess([In] uint id, [In] int win32Attach, [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateProcesses([MarshalAs(UnmanagedType.Interface)] out ICorDebugProcessEnum ppProcess);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetProcess([In] uint dwProcessId, [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CanLaunchOrAttach([In] uint dwProcessId, [In] int win32DebuggingEnabled);
    }
}

#pragma warning restore 108, 1591