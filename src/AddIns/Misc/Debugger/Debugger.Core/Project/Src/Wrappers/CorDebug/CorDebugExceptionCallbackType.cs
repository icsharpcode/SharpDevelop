// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public enum CorDebugExceptionCallbackType : int
	{
		
		DEBUG_EXCEPTION_FIRST_CHANCE = 1,
		
		DEBUG_EXCEPTION_USER_FIRST_CHANCE = 2,
		
		DEBUG_EXCEPTION_CATCH_HANDLER_FOUND = 3,
		
		DEBUG_EXCEPTION_UNHANDLED = 4,
	}
}
