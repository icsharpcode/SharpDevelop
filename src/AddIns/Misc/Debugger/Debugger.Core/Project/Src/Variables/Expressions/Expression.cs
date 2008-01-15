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
	/// Represents a piece of code that can be evaluated.
	/// For example "a[15] + 15".
	/// </summary>
	public abstract partial class Expression: DebuggerObject
	{
		public abstract string Code {
			get;
		}
		
		public override string ToString()
		{
			return this.Code;
		}
		
		public Value Evaluate(StackFrame context)
		{
			if (context == null) throw new ArgumentNullException("context");
			if (context.HasExpired) throw new DebuggerException("Context is expired StackFrame");
			
			Value result;
			try {
				result = EvaluateInternal(context);
			} catch (GetValueException e) {
				if (e.Expression == null) {
					e.Expression = this;
				}
				throw;
			}
			
			context.Process.TraceMessage("Evaluated " + this.GetType().Name + ": "+ this.Code);
			return result;
		}
		
		protected abstract Value EvaluateInternal(StackFrame context);
	}
}
