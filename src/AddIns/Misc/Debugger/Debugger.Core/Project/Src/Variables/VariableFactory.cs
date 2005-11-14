// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public static class VariableFactory
	{
		internal static Variable CreateVariable(NDebugger debugger, ICorDebugValue corValue, string name)
		{
			Value val = ValueFactory.CreateValue(debugger, corValue);
			return CreateVariable(val, name);
		}
		
		public static Variable CreateVariable(Value val, string name)
		{
			return new Variable(val.Debugger, val, name);
		}
	}
}
