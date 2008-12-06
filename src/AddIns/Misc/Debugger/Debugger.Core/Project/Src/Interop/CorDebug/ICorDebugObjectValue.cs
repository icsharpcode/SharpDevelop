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

    [ComImport, Guid("18AD3D6E-B7D2-11D2-BD04-0000F80849BD"), InterfaceType((short) 1)]
    public interface ICorDebugObjectValue : ICorDebugValue
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetType(out uint pType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSize(out uint pSize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetAddress(out ulong pAddress);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetClass([MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldValue([In, MarshalAs(UnmanagedType.Interface)] ICorDebugClass pClass, [In] uint fieldDef, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetVirtualMethod([In] uint memberRef, [MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetContext([MarshalAs(UnmanagedType.Interface)] out ICorDebugContext ppContext);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsValueClass(out int pbIsValueClass);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetManagedCopy([MarshalAs(UnmanagedType.IUnknown)] out object ppObject);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetFromManagedCopy([In, MarshalAs(UnmanagedType.IUnknown)] object pObject);
    }
}

#pragma warning restore 108, 1591