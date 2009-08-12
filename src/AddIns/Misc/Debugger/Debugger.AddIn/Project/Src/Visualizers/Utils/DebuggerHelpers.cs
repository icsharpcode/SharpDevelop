// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using Debugger.MetaData;
using Debugger.Wrappers.CorDebug;
using ICSharpCode.SharpDevelop.Services;
using Expression = ICSharpCode.NRefactory.Ast.Expression;

namespace Debugger.AddIn.Visualizers.Utils
{
	public static class DebuggerHelpers
	{
		/// <summary>
		/// Gets underlying address of object in the debuggee.
		/// </summary>
		public static ulong GetObjectAddress(this Value val)
		{
			ICorDebugReferenceValue refVal = val.CorValue.CastTo<ICorDebugReferenceValue>();
			return refVal.Value;
		}
		
		/// <summary>
		/// Returns true if this type is enum.
		/// </summary>
		public static bool IsEnum(this DebugType type)
		{
			return (type.BaseType != null) && (type.BaseType.FullName == "System.Enum");
		}
		
		/// <summary>
		/// Returns true is this type is just System.Object.
		/// </summary>
		public static bool IsSystemDotObject(this DebugType type)
		{
			return type.FullName == "System.Object";
		}
		
		/// <summary>
		/// Evaluates expression and gets underlying address of object in the debuggee.
		/// </summary>
		public static ulong GetObjectAddress(this Expression expr)
		{
			return expr.Evaluate(WindowsDebugger.CurrentProcess).GetObjectAddress();
		}
		
		/// <summary>
		/// System.Runtime.CompilerServices.GetHashCode method, for obtaining non-overriden hash codes from debuggee.
		/// </summary>
		private static MethodInfo hashCodeMethod;
		
		/// <summary>
		/// Invokes RuntimeHelpers.GetHashCode on given value, that is a default hashCode ignoring user overrides.
		/// </summary>
		/// <param name="value">Valid value.</param>
		/// <returns>Hash code of the object in the debugee.</returns>
		public static int InvokeDefaultGetHashCode(this Value value)
		{
			if (DebuggerHelpers.hashCodeMethod == null)
			{
				DebugType typeRuntimeHelpers = DebugType.CreateFromType(value.AppDomain, typeof(System.Runtime.CompilerServices.RuntimeHelpers));
				DebuggerHelpers.hashCodeMethod = typeRuntimeHelpers.GetMember("GetHashCode", BindingFlags.Public | BindingFlags.Static | BindingFlags.Method | BindingFlags.IncludeSuperType) as MethodInfo;
				if (DebuggerHelpers.hashCodeMethod == null)
				{
					throw new DebuggerException("Cannot obtain method System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode");
				}
			}
			
			// David: I had hard time finding out how to invoke static method.
			// value.InvokeMethod is nice for instance methods.
			// what about MethodInfo.Invoke() ?
			// also, there could be an overload that takes 1 parameter instead of array
			string defaultHashCode = Eval.InvokeMethod(DebuggerHelpers.hashCodeMethod, null, new Value[]{value}).AsString;
			
			//MethodInfo method = value.Type.GetMember("GetHashCode", BindingFlags.Method | BindingFlags.IncludeSuperType) as MethodInfo;
			//string hashCode = value.InvokeMethod(method, null).AsString;
			return int.Parse(defaultHashCode);
		}
		
		public static Value EvalPermanentReference(this Expression expr)
		{
			return expr.Evaluate(WindowsDebugger.CurrentProcess).GetPermanentReference();
		}
		
		public static bool IsNull(this Expression expr)
		{
			return expr.Evaluate(WindowsDebugger.CurrentProcess).IsNull;
		}
		
		public static DebugType GetType(this Expression expr)
		{
			return expr.Evaluate(WindowsDebugger.CurrentProcess).Type;
		}
	}
}
