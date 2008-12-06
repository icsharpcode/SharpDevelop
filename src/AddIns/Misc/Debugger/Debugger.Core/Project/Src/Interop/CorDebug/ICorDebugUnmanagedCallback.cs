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

    [ComImport, Guid("5263E909-8CB5-11D3-BD2F-0000F80849BD"), InterfaceType((short) 1)]
    public interface ICorDebugUnmanagedCallback
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DebugEvent([In, ComAliasName("Debugger.Interop.CorDebug.ULONG_PTR")] uint pDebugEvent, [In] int fOutOfBand);
    }
}

#pragma warning restore 108, 1591