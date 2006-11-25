// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorDebug
{
    using System.Runtime.InteropServices;

    [ComImport, Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF"), CoClass(typeof(CorDebugClass))]
    public interface CorDebug : ICorDebug
    {
    }
}

#pragma warning restore 108, 1591