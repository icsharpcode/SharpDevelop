// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Expressions
{
	/// <summary>
	/// Identifier of a method parameter
	/// </summary>
	public class ParameterIdentifierExpression: Expression
	{
		MethodInfo method;
		int index;
		string name;
		
		public MethodInfo Method {
			get { return method; }
		}
		
		public int Index {
			get { return index; }
		}
		
		public string Name {
			get { return name; }
		}
		
		public ParameterIdentifierExpression(MethodInfo method, int index, string name)
		{
			this.method = method;
			this.index = index;
			this.name = name;
		}
		
		public override string Code {
			get {
				return this.Name;
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (context.MethodInfo != this.Method) {
				throw new ExpressionEvaluateException(this, "Method " + this.Method.FullName + " expected, " + context.MethodInfo.FullName + " seen");
			}
			return context.GetArgument(this.Index);
		}
	}
}
