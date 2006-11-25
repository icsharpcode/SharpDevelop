// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorSym
{
    using System.Runtime.InteropServices;

    [ComImport, CoClass(typeof(CorSymBinder_SxSClass)), Guid("AA544D42-28CB-11D3-BD22-0000F80849BD")]
    public interface CorSymBinder_SxS : ISymUnmanagedBinder
    {
    }
}

#pragma warning restore 108, 1591