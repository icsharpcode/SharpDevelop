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
			Value value = context.GetValue(identifier);
			if (value == null) {
				throw new GetValueException("Identifier " + identifier + " not found");
			}
			return value;
		}
	}
}
