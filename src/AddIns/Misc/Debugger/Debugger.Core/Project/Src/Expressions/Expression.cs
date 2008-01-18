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
		static Dictionary<Expression, Value> expressionCache;
		static DebugeeState expressionCache_debuggerState;
		static Thread expressionCache_thread;
		static int expressionCache_stackDepth;
		
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
		
		Value GetFromCache(StackFrame context)
		{
			if (expressionCache == null ||
				expressionCache_debuggerState != context.Process.DebugeeState ||
				expressionCache_thread != context.Thread ||
				expressionCache_stackDepth != context.Depth)
			{
				expressionCache = new Dictionary<Expression, Value>();
				expressionCache_debuggerState = context.Process.DebugeeState;
				expressionCache_thread = context.Thread;
				expressionCache_stackDepth = context.Depth;
				context.Process.TraceMessage("Expression cache cleared");
			}
			if (expressionCache.ContainsKey(this)) {
				Value cachedResult = expressionCache[this];
				if (!cachedResult.HasExpired) {
					return cachedResult;
				}
			}
			return null;
		}
		
		public Value Evaluate(StackFrame context)
		{
			if (context == null) throw new ArgumentNullException("context");
			if (context.HasExpired) throw new DebuggerException("Context is expired StackFrame");
			
			Value result;
			
			result = GetFromCache(context);
			if (result != null) {
				context.Process.TraceMessage(string.Format("Cached:    {0,-12} ({1})", this.Code, this.GetType().Name));
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
			
			context.Process.TraceMessage(string.Format("Evaluated: {0,-12} ({1}) ({2} ms)", this.Code, this.GetType().Name, (end - start).TotalMilliseconds));
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
