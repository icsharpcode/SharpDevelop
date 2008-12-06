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

    [ComImport, Guid("03E26314-4F76-11D3-88C6-006097945418"), InterfaceType((short) 1)]
    public interface ICorDebugNativeFrame : ICorDebugFrame
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
        void GetIP(out uint pnOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetIP([In] uint nOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRegisterSet([MarshalAs(UnmanagedType.Interface)] out ICorDebugRegisterSet ppRegisters);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetLocalRegisterValue([In] CorDebugRegister reg, [In] uint cbSigBlob, [In, ComAliasName("Debugger.Interop.CorDebug.ULONG_PTR")] uint pvSigBlob, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetLocalDoubleRegisterValue([In] CorDebugRegister highWordReg, [In] CorDebugRegister lowWordReg, [In] uint cbSigBlob, [In, ComAliasName("Debugger.Interop.CorDebug.ULONG_PTR")] uint pvSigBlob, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetLocalMemoryValue([In] ulong address, [In] uint cbSigBlob, [In, ComAliasName("Debugger.Interop.CorDebug.ULONG_PTR")] uint pvSigBlob, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetLocalRegisterMemoryValue([In] CorDebugRegister highWordReg, [In] ulong lowWordAddress, [In] uint cbSigBlob, [In, ComAliasName("Debugger.Interop.CorDebug.ULONG_PTR")] uint pvSigBlob, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetLocalMemoryRegisterValue([In] ulong highWordAddress, [In] CorDebugRegister lowWordRegister, [In] uint cbSigBlob, [In, ComAliasName("Debugger.Interop.CorDebug.ULONG_PTR")] uint pvSigBlob, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CanSetIP([In] uint nOffset);
    }
}

#pragma warning restore 108, 1591