// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger;

namespace Debugger.Expressions
{
	public class SimpleIdentifierExpression: Expression
	{
		string identifier;
		
		public string Identifier {
			get { return identifier; }
		}
		
		public SimpleIdentifierExpression(string identifier)
		{
			this.identifier = identifier;
		}
		
		public override string Code {
			get {
				return identifier;
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (identifier == "this") {
				return context.GetThisValue();
			}
			
			Value arg = context.GetArgumentValue(identifier);
			if (arg != null) return arg;
			
			Value local = context.GetLocalVariableValue(identifier);
			if (local != null) return local;
			
			if (!context.MethodInfo.IsStatic) {
				Value member = context.GetThisValue().GetMemberValue(identifier);
				if (member != null) return member;
			}
			
			throw new GetValueException("Identifier " + identifier + " not found");
		}
	}
}
