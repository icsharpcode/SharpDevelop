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

    [ComImport, Guid("CC7BCB0B-8A68-11D2-983C-0000F808342D"), ComConversionLoss, InterfaceType((short) 1)]
    public interface ICorDebugRegisterSet
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRegistersAvailable(out ulong pAvailable);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRegisters([In] ulong mask, [In] uint regCount, [Out] IntPtr regBuffer);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetRegisters([In] ulong mask, [In] uint regCount, [In] ref ulong regBuffer);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetThreadContext([In] uint contextSize, [In, Out] IntPtr context);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetThreadContext([In] uint contextSize, [In] IntPtr context);
    }
}

#pragma warning restore 108, 1591