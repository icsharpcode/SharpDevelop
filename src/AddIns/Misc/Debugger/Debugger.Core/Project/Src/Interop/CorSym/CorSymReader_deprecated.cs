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

    [ComImport, Guid("B4CE6286-2A6B-3712-A3B7-1EE1DAD467B5"), CoClass(typeof(CorSymReader_deprecatedClass))]
    public interface CorSymReader_deprecated : ISymUnmanagedReader
    {
    }
}

#pragma warning restore 108, 1591