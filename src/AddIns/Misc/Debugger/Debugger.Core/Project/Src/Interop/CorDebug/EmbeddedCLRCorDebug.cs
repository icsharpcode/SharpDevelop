// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Interop.CorDebug
{
    using System.Runtime.InteropServices;

    [ComImport, Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF"), CoClass(typeof(EmbeddedCLRCorDebugClass))]
    public interface EmbeddedCLRCorDebug : ICorDebug
    {
    }
}
