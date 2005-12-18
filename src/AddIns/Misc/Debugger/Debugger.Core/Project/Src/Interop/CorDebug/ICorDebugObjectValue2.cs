// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Interop.CorDebug
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("49E4A320-4A9B-4ECA-B105-229FB7D5009F"), InterfaceType((short) 1)]
    public interface ICorDebugObjectValue2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetVirtualMethodAndType([In] uint memberRef, [MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction, [MarshalAs(UnmanagedType.Interface)] out ICorDebugType ppType);
    }
}
