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
	/// A placeholder expression which can not be evaluated.
	/// </summary>
	public class EmptyExpression: Expression
	{
		public override string Code {
			get {
				return "?";
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			throw new GetValueException("Empty expression");
		}
	}
}
