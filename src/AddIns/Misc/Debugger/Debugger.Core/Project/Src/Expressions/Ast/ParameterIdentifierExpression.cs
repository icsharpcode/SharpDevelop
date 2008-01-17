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
	/// Identifier of a parameter within a givne method.
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
		
		public ParameterIdentifierExpression(MethodInfo method, int index)
		{
			if (method == null) throw new ArgumentNullException("method");
			
			this.method = method;
			this.index = index;
			this.name = method.GetParameterName(index);
		}
		
		public override string Code {
			get {
				return name;
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (context.MethodInfo != method) {
				throw new GetValueException("Method " + method.FullName + " expected, " + context.MethodInfo.FullName + " seen");
			}
			
			return context.GetArgumentValue(index);
		}
	}
}
