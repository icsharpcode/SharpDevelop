// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Interop.CorSym
{
    using System.Runtime.InteropServices;

    [ComImport, CoClass(typeof(CorSymWriter_SxSClass)), Guid("ED14AA72-78E2-4884-84E2-334293AE5214")]
    public interface CorSymWriter_SxS : ISymUnmanagedWriter
    {
    }
}

