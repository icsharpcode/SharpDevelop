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

    [ComImport, Guid("ED14AA72-78E2-4884-84E2-334293AE5214"), CoClass(typeof(CorSymWriter_deprecatedClass))]
    public interface CorSymWriter_deprecated : ISymUnmanagedWriter
    {
    }
}

#pragma warning restore 108, 1591