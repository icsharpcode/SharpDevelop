// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2285 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using Ast = ICSharpCode.NRefactory.Ast;
using Debugger.Wrappers.CorSym;

namespace Debugger
{
	public partial class Expression: DebuggerObject
	{
		public Value Evaluate(StackFrame context)
		{
			if (context == null) throw new ArgumentNullException("context");
			
			EvaluateAstVisitor astVisitor = new EvaluateAstVisitor(context);
			Value result = (Value)this.AbstractSynatxTree.AcceptVisitor(astVisitor, null);
			context.Process.TraceMessage("Evaluated " + this.Code);
			return result;
		}
	}
}
