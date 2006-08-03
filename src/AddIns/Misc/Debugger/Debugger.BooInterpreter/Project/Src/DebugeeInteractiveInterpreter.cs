// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Boo.Lang.Interpreter;

namespace Debugger
{
	public class DebugeeInteractiveInterpreter: InteractiveInterpreter
	{
		public DebugeeInteractiveInterpreter()
		{
		}
		
		public override void Declare(string name, Type type)
		{
			base.Declare(name, type);
		}
		
		public object localVariable;
		
		void DoCommand(string command, string param)
		{
			System.Diagnostics.Debugger.Log(0xB00, command, param);
		}
		
		public override object GetValue(string name)
		{
			DoCommand("DebugeeInterpreterContext.BeforeGetValue", name);
			object locVar = localVariable;
			if (locVar != null) {
				localVariable = null;
				return locVar;
			} else {
				return base.GetValue(name);
			}
		}
		
		public override object SetValue(string name, object val)
		{
			localVariable = val;
			DoCommand("DebugeeInterpreterContext.BeforeSetValue", name);
			localVariable = null;
			return base.SetValue(name, val);
		}
		
		public override Type Lookup(string name)
		{
			return base.Lookup(name);
		}
		
		public override void SetLastValue(object val)
		{
			base.SetLastValue(val);
		}
	}
}
