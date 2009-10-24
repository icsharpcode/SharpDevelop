// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using Debugger.Wrappers.MetaData;
using ICSharpCode.NRefactory.Ast;
using Mono.Cecil.Signatures;

namespace Debugger.MetaData
{
	public class DebugLocalVariableInfo
	{
		ValueGetter getter;
		
		public string Name { get; internal set; }
		public DebugType Type { get; private set; }
		public bool IsThis { get; internal set; }
		public bool IsCaptured { get; internal set; }
		
		public DebugLocalVariableInfo(string name, DebugType type, ValueGetter getter)
		{
			this.Name = name;
			this.Type = type;
			this.getter = getter;
		}
		
		public Value GetValue(StackFrame context)
		{
			return getter(context);
		}
		
		public override string ToString()
		{
			return this.Type.ToString() + " " + this.Name;
		}
	}
}
