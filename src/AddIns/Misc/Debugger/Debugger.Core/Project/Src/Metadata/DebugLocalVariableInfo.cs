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
	public class DebugLocalVariableInfo: System.Reflection.LocalVariableInfo
	{
		ValueGetter getter;
		int localIndex;
		DebugType localType;
		
		public override int LocalIndex {
			get { return localIndex; }
		}
		
		public override Type LocalType {
			get { return localType; }
		}
		
		public override bool IsPinned {
			get { throw new NotSupportedException(); }
		}
		
		public string Name { get; internal set; }
		public bool IsThis { get; internal set; }
		public bool IsCaptured { get; internal set; }
		
		public DebugLocalVariableInfo(string name, int localIndex, DebugType localType, ValueGetter getter)
		{
			this.Name = name;
			this.localIndex = localIndex;
			this.localType = localType;
			this.getter = getter;
		}
		
		public Value GetValue(StackFrame context)
		{
			return getter(context);
		}
		
		public override string ToString()
		{
			string msg = this.LocalType + " " + this.Name;
			if (IsCaptured)
				msg += " (captured)";
			return msg;
		}
	}
}
