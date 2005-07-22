// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace DebuggerLibrary
{
	public enum ExceptionType
	{
		DEBUG_EXCEPTION_FIRST_CHANCE = 1,
		DEBUG_EXCEPTION_UNHANDLED = 4,
		DEBUG_EXCEPTION_USER_FIRST_CHANCE = 2,
		DEBUG_EXCEPTION_CATCH_HANDLER_FOUND = 3,
	}
}
