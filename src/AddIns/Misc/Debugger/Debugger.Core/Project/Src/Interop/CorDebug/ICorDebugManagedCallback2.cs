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

    [ComImport, Guid("250E5EEA-DB5C-4C76-B6F3-8C46F12E3203"), InterfaceType((short) 1)]
    public interface ICorDebugManagedCallback2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void FunctionRemapOpportunity([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pOldFunction, [In] IntPtr pNewFunction, [In] uint oldILOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateConnection([In] IntPtr pProcess, [In] uint dwConnectionId, [In] IntPtr pConnName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ChangeConnection([In] IntPtr pProcess, [In] uint dwConnectionId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DestroyConnection([In] IntPtr pProcess, [In] uint dwConnectionId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Exception([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pFrame, [In] uint nOffset, [In] Debugger.Wrappers.CorDebug.CorDebugExceptionCallbackType dwEventType, [In] uint dwFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ExceptionUnwind([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] Debugger.Wrappers.CorDebug.CorDebugExceptionUnwindCallbackType dwEventType, [In] uint dwFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void FunctionRemapComplete([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pFunction);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void MDANotification([In] IntPtr pController, [In] IntPtr pThread, [In] IntPtr pMDA);
    }
}

#pragma warning restore 108, 1591