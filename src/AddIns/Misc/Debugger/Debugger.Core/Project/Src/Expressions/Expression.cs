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
	public abstract partial class Expression: DebuggerObject, IComparable<Expression>
	{
		static Dictionary<Expression, Value> expressionCache;
		static DebuggeeState expressionCache_debuggerState;
		static Thread expressionCache_thread;
		
		public abstract string Code {
			get;
		}
		
		public virtual string CodeTail {
			get {
				return this.Code;
			}
		}
		
		public int CompareTo(Expression other)
		{
			return this.CodeTail.CompareTo(other.CodeTail);
		}
		
		public override string ToString()
		{
			return this.Code;
		}
		
		Value GetFromCache(StackFrame context)
		{
			if (expressionCache == null ||
				expressionCache_debuggerState != context.Process.DebuggeeState ||
				expressionCache_thread != context.Thread)
			{
				expressionCache = new Dictionary<Expression, Value>();
				expressionCache_debuggerState = context.Process.DebuggeeState;
				expressionCache_thread = context.Thread;
				context.Process.TraceMessage("Expression cache cleared");
			}
			if (expressionCache.ContainsKey(this)) {
				Value cachedResult = expressionCache[this];
				if (!cachedResult.IsInvalid) {
					return cachedResult;
				}
			}
			return null;
		}
		
		public Value Evaluate(Process context)
		{
			if (context.SelectedStackFrame != null) {
				return Evaluate(context.SelectedStackFrame);
			} else if (context.SelectedThread.MostRecentStackFrame != null ) {
				return Evaluate(context.SelectedThread.MostRecentStackFrame);
			} else {
				// This can happen when needed 'dll' is missing.  This causes an exception dialog to be shown even before the applicaiton starts
				throw new GetValueException("Can not evaluate because the process has no managed stack frames");
			}
		}
		
		public Value Evaluate(StackFrame context)
		{
			if (context == null) throw new ArgumentNullException("context");
			if (context.IsInvalid) throw new DebuggerException("The context is no longer valid");
			
			Value result;
			
			result = GetFromCache(context);
			if (result != null) {
				if (context.Process.Options.Verbose) {
					context.Process.TraceMessage(string.Format("Cached:    {0,-12} ({1})", this.Code, this.GetType().Name));
				}
				return result;
			}
			
			DateTime start = Debugger.Util.HighPrecisionTimer.Now;
			try {
				result = EvaluateInternal(context);
			} catch (GetValueException e) {
				if (e.Expression == null) {
					e.Expression = this;
				}
				throw;
			}
			DateTime end = Debugger.Util.HighPrecisionTimer.Now;
			expressionCache[this] = result;
			
			if (context.Process.Options.Verbose) {
				context.Process.TraceMessage(string.Format("Evaluated: {0,-12} ({1}) ({2} ms)", this.Code, this.GetType().Name, (end - start).TotalMilliseconds));
			}
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
