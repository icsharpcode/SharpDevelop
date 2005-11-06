// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Interop.CorDebug
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("B008EA8D-7AB1-43F7-BB20-FBB5A04038AE"), InterfaceType((short) 1)]
    public interface ICorDebugClass2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetParameterizedType([In] uint elementType, [In] uint nTypeArgs, [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugType ppTypeArgs, [MarshalAs(UnmanagedType.Interface)] out ICorDebugType ppType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetJMCStatus([In] int bIsJustMyCode);
    }
}
