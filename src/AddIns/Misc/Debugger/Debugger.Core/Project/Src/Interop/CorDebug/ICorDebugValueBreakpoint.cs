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

    [ComImport, Guid("CC7BCAEB-8A68-11D2-983C-0000F808342D"), InterfaceType((short) 1)]
    public interface ICorDebugValueBreakpoint : ICorDebugBreakpoint
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Activate([In] int bActive);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsActive(out int pbActive);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetValue([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
    }
}

#pragma warning restore 108, 1591