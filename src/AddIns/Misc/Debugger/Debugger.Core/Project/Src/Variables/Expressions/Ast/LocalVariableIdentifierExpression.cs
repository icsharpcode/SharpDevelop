// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorSym;

namespace Debugger.Expressions
{
	/// <summary>
	/// Identifier of a local variable
	/// </summary>
	public class LocalVariableIdentifierExpression: Expression
	{
		MethodInfo method;
		ISymUnmanagedVariable symVar;
		
		public MethodInfo Method {
			get { return method; }
		}
		
		public ISymUnmanagedVariable SymVar {
			get { return symVar; }
		}
		
		public LocalVariableIdentifierExpression(MethodInfo method, ISymUnmanagedVariable symVar)
		{
			this.method = method;
			this.symVar = symVar;
		}
		
		public override string Code {
			get {
				return this.SymVar.Name;
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (context.MethodInfo != this.Method) {
				throw new ExpressionEvaluateException(this, "Method " + this.Method.FullName + " expected, " + context.MethodInfo.FullName + " seen");
			}
			return context.GetLocalVariableValue(this.SymVar);
		}
	}
}
