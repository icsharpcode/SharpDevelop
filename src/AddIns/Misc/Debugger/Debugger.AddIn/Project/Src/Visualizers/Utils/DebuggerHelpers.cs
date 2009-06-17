// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger.AddIn.Visualizers.Utils
{
	public static class DebuggerHelpers
	{
		/// <summary>
		/// Gets underlying address of object in the debuggee.
		/// </summary>
		public static ulong GetObjectAddress(this Value val)
		{
			ICorDebugReferenceValue refVal = val.CorValue.CastTo<ICorDebugReferenceValue>();
			return refVal.Value;
		}
	}
}
