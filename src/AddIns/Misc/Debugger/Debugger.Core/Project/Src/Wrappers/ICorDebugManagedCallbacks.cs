// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	public interface ICorDebugManagedCallbacks: Debugger.Interop.CorDebug.ICorDebugManagedCallback, Debugger.Interop.CorDebug.ICorDebugManagedCallback2
	{
		
	}
}

#pragma warning restore 1591
