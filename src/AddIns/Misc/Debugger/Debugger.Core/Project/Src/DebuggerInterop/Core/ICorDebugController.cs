// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace DebuggerInterop.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("3D6F5F62-7538-11D3-8D5B-00104B35E7EF"), InterfaceType((short) 1)]
    public interface ICorDebugController
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Stop([In] uint dwTimeoutIgnored);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Continue([In] int fIsOutOfBand);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsRunning(out int pbRunning);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void HasQueuedCallbacks([In, MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread, out int pbQueued);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumerateThreads([MarshalAs(UnmanagedType.Interface)] out ICorDebugThreadEnum ppThreads);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetAllThreadsDebugState([In] CorDebugThreadState state, [In, MarshalAs(UnmanagedType.Interface)] ICorDebugThread pExceptThisThread);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Detach();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Terminate([In] uint exitCode);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CanCommitChanges([In] uint cSnapshots, [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugEditAndContinueSnapshot pSnapshots, [MarshalAs(UnmanagedType.Interface)] out ICorDebugErrorInfoEnum pError);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CommitChanges([In] uint cSnapshots, [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugEditAndContinueSnapshot pSnapshots, [MarshalAs(UnmanagedType.Interface)] out ICorDebugErrorInfoEnum pError);
    }
}
