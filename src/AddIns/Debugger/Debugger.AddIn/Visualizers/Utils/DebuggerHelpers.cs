// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Linq;
using Debugger.Interop.CorDebug;
using System;
using System.Collections.Generic;
using System.Reflection;
using Debugger;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.Utils
{
	public static class DebuggerHelpers
	{
		/// <summary>
		/// Creates an expression which, when evaluated, creates a List&lt;T&gt; in the debugee
		/// filled with contents of IEnumerable&lt;T&gt; from the debugee.
		/// </summary>
		/// <param name="iEnumerableVariable">Expression for IEnumerable variable in the debugee.</param>
		/// <param name="itemType">
		/// The generic argument of IEnumerable&lt;T&gt; that <paramref name="iEnumerableVariable"/> implements.</param>
		public static Expression CreateDebugListExpression(Expression iEnumerableVariable, DebugType itemType, out DebugType listType)
		{
			// is using itemType.AppDomain ok?
			listType = DebugType.CreateFromType(itemType.AppDomain, typeof(System.Collections.Generic.List<>), itemType);
			var iEnumerableType = DebugType.CreateFromType(itemType.AppDomain, typeof(IEnumerable<>), itemType);
			// explicitely cast the variable to IEnumerable<T>, where T is itemType
			Expression iEnumerableVariableExplicitCast = new CastExpression(iEnumerableType.GetTypeReference() , iEnumerableVariable, CastType.Cast);
			return new ObjectCreateExpression(listType.GetTypeReference(), iEnumerableVariableExplicitCast.SingleItemList());
		}
		
		/// <summary>
		/// Gets underlying address of object in the debuggee.
		/// </summary>
		public static ulong GetObjectAddress(this Value val)
		{
			if (val.IsNull) return 0;
			ICorDebugReferenceValue refVal = val.CorReferenceValue;
			return refVal.GetValue();
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
		private static DebugMethodInfo hashCodeMethod;
		
		/// <summary>
		/// Invokes RuntimeHelpers.GetHashCode on given value, that is a default hashCode ignoring user overrides.
		/// </summary>
		/// <param name="value">Valid value.</param>
		/// <returns>Hash code of the object in the debugee.</returns>
		public static int InvokeDefaultGetHashCode(this Value value)
		{
			if (hashCodeMethod == null || hashCodeMethod.Process.HasExited) {
				DebugType typeRuntimeHelpers = DebugType.CreateFromType(value.AppDomain, typeof(System.Runtime.CompilerServices.RuntimeHelpers));
				hashCodeMethod = (DebugMethodInfo)typeRuntimeHelpers.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Static);
				if (hashCodeMethod == null) {
					throw new DebuggerException("Cannot obtain method System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode");
				}
			}
			Value defaultHashCode = Eval.InvokeMethod(DebuggerHelpers.hashCodeMethod, null, new Value[]{value});
			
			//MethodInfo method = value.Type.GetMember("GetHashCode", BindingFlags.Method | BindingFlags.IncludeSuperType) as MethodInfo;
			//string hashCode = value.InvokeMethod(method, null).AsString;
			return (int)defaultHashCode.PrimitiveValue;
		}
		
		/// <summary>
		/// Formats System.Type to the C# format, that is generic parameters in angle brackets.
		/// </summary>
		public static string FormatNameCSharp(this Type type)
		{
			string typeName = type.Name.CutoffEnd("`");	// get rid of the `n in generic type names
			if (type.IsGenericType) {
				return typeName + "<" + string.Join(", ", type.GetGenericArguments().Select(a => FormatNameCSharp(a))) + ">";
			} else {
				return typeName;
			}
		}
		
		public static Value EvalPermanentReference(this Expression expr)
		{
			return expr.Evaluate(WindowsDebugger.CurrentProcess).GetPermanentReference();
		}
		
		/// <summary>
		/// Evaluates System.Collections.ICollection.Count property on given object.
		/// </summary>
		/// <exception cref="GetValueException">Evaluating System.Collections.ICollection.Count on targetObject failed.</exception>
		public static int GetIListCount(this Expression targetObject)
		{
			Value list = targetObject.Evaluate(WindowsDebugger.CurrentProcess);
			var iCollectionType = list.Type.GetInterface(typeof(System.Collections.ICollection).FullName);
			if (iCollectionType == null)
				throw new GetValueException(targetObject, targetObject.PrettyPrint() + " does not implement System.Collections.ICollection");
			// Do not get string representation since it can be printed in hex
			return (int)list.GetPropertyValue(iCollectionType.GetProperty("Count")).PrimitiveValue;
		}
		
		/// <summary>
		/// Prepends a cast to IList before the given Expression.
		/// </summary>
		public static Expression CastToIList(this Expression expr)
		{
			return new CastExpression(
				new TypeReference(typeof(System.Collections.IList).FullName),
				expr.Parenthesize(),
				CastType.Cast
			);
		}
	}
}
