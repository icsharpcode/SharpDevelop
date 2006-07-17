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
		
		public override object GetValue(string name)
		{
			System.Diagnostics.Debugger.Log(0xB00, "DebugeeInterpreterContext.BeforeGetValue", name);
			return base.GetValue(name);
		}
		
		public object GetValueInternal(string name)
		{
			return base.GetValue(name);
		}
		
		public override Type Lookup(string name)
		{
			return base.Lookup(name);
		}
		
		public override void SetLastValue(object val)
		{
			base.SetLastValue(val);
		}
		
		public override object SetValue(string name, object val)
		{
			object ret = base.SetValue(name, val);
			System.Diagnostics.Debugger.Log(0xB00, "DebugeeInterpreterContext.AfterSetValue", name);
			return ret;
		}
		
		public object SetValueInternal(string name, object val)
		{
			return base.SetValue(name, val);
		}
	}
}
