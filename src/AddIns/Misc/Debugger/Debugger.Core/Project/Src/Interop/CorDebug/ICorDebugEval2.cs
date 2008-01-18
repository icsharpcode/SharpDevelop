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

    [ComImport, Guid("FB0D9CE7-BE66-4683-9D32-A42A04E2FD91"), InterfaceType((short) 1)]
    public interface ICorDebugEval2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CallParameterizedFunction([In, MarshalAs(UnmanagedType.Interface)] ICorDebugFunction pFunction, [In] uint nTypeArgs, [In, MarshalAs(UnmanagedType.LPArray)] ICorDebugType[] ppTypeArgs, [In] uint nArgs, [In, MarshalAs(UnmanagedType.LPArray)] ICorDebugValue[] ppArgs);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateValueForType([In, MarshalAs(UnmanagedType.Interface)] ICorDebugType pType, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void NewParameterizedObject([In, MarshalAs(UnmanagedType.Interface)] ICorDebugFunction pConstructor, [In] uint nTypeArgs, [In, MarshalAs(UnmanagedType.LPArray)] ICorDebugType[] ppTypeArgs, [In] uint nArgs, [In, MarshalAs(UnmanagedType.LPArray)] ICorDebugValue[] ppArgs);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void NewParameterizedObjectNoConstructor([In, MarshalAs(UnmanagedType.Interface)] ICorDebugClass pClass, [In] uint nTypeArgs, [In, MarshalAs(UnmanagedType.LPArray)] ICorDebugType[] ppTypeArgs);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void NewParameterizedArray([In, MarshalAs(UnmanagedType.Interface)] ICorDebugType pElementType, [In] uint rank, [In] ref uint dims, [In] ref uint lowBounds);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void NewStringWithLength([In, MarshalAs(UnmanagedType.LPWStr)] string @string, [In] uint uiLength);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void RudeAbort();
    }
}

#pragma warning restore 108, 1591