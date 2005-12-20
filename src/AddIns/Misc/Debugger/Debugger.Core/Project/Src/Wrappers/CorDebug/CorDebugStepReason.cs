// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public enum CorDebugStepReason : int
	{
		
		STEP_NORMAL = 0,
		
		STEP_RETURN = 1,
		
		STEP_CALL = 2,
		
		STEP_EXCEPTION_FILTER = 3,
		
		STEP_EXCEPTION_HANDLER = 4,
		
		STEP_INTERCEPT = 5,
		
		STEP_EXIT = 6,
	}
}
