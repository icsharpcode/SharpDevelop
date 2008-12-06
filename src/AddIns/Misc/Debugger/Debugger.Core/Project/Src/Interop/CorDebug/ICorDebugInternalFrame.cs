// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorDebug
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("B92CC7F7-9D2D-45C4-BC2B-621FCC9DFBF4"), InterfaceType((short) 1)]
    public interface ICorDebugInternalFrame : ICorDebugFrame
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetChain([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFunction([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFunctionToken(out uint pToken);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetStackRange(out ulong pStart, out ulong pEnd);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCaller([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCallee([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateStepper([MarshalAs(UnmanagedType.Interface)] out ICorDebugStepper ppStepper);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFrameType(out CorDebugInternalFrameType pType);
    }
}

#pragma warning restore 108, 1591