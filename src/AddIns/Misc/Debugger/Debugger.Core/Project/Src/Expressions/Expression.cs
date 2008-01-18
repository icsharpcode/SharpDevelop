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
		
		public virtual string CodeTail {
			get {
				return this.Code;
			}
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
		
		protected static int GetArrayHashCode<T>(T[] array)
		{
			int hashCode = 0;
			unchecked {
				if (array != null) {
					foreach(T element in array) {
						 hashCode += element.GetHashCode(); 
					}
				}
			}
			return hashCode;
		}
		
		protected static bool ArrayEquals<T>(T[] a, T[] b)
		{
			if (a == null && b == null) return true;
			if (a != null && b == null) return false;
			if (a == null && b != null) return false;
			// Both not null
			if (a.Length != b.Length) return false;
			for(int i = 0; i < a.Length; i++) {
				if (!a[i].Equals(b[i])) return false;
			}
			return true;
		}
	}
}
