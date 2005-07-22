// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace DebuggerLibrary
{
	public class DebuggerException: System.Exception
	{
		public DebuggerException() {}
		public DebuggerException(string message): base(message) {}
		public DebuggerException(string message, System.Exception inner): base(message, inner) {}
	}

	public class BadSignatureException: DebuggerException {}	
}
