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
	/// Identifier of a local variable within a given method.
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
			if (method == null) throw new ArgumentNullException("method");
			if (symVar == null) throw new ArgumentNullException("symVar");
			
			this.method = method;
			this.symVar = symVar;
		}
		
		public override string Code {
			get {
				return symVar.Name;
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (context.MethodInfo != method) {
				throw new GetValueException("Method " + method.FullName + " expected, " + context.MethodInfo.FullName + " seen");
			}
			
			return context.GetLocalVariableValue(symVar);
		}
	}
}
