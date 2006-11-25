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

    [ComImport, InterfaceType((short) 1), Guid("2BD956D9-7B07-4BEF-8A98-12AA862417C5"), ComConversionLoss]
    public interface ICorDebugThread2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetActiveFunctions([In] uint cFunctions, out uint pcFunctions, [In, Out] IntPtr pFunctions);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetConnectionID(out uint pdwConnectionId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetTaskID(out ulong pTaskId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetVolatileOSThreadID(out uint pdwTid);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void InterceptCurrentException([In, MarshalAs(UnmanagedType.Interface)] ICorDebugFrame pFrame);
    }
}

#pragma warning restore 108, 1591