// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Linq;
using Debugger.AddIn.Visualizers.Graph;
using Debugger.Interop.CorDebug;
using System;
using System.Collections.Generic;
using System.Reflection;
using Debugger;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.Utils
{
	public static class DebuggerHelpers
	{
		// Object graph visualizer: collection support temp disabled (porting to new NRefactory).
		/*
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
		}*/
		
		/// <summary>
		/// Evaluates 'new List&lt;T&gt;(iEnumerableValue)' in the debuggee.
		/// </summary>
		public static Value CreateListFromIEnumerable(Value iEnumerableValue)
		{
			IType iEnumerableType, itemType;
			if (!iEnumerableValue.Type.ResolveIEnumerableImplementation(out iEnumerableType, out itemType))
				throw new GetValueException("Value is not IEnumerable");
			// FIXME    	
			IType listType = DebugType.CreateFromType(iEnumerableValue.AppDomain, typeof(System.Collections.Generic.List<>), itemType);
			DebugConstructorInfo ctor = listType.GetConstructor(BindingFlags.Default, null, CallingConventions.Any, new System.Type[] { iEnumerableType }, null);
			if (ctor == null)
				throw new DebuggerException("List<T> constructor not found");
			
			// Keep reference since we do not want to keep reenumerating it
			return Value.InvokeMethod(WindowsDebugger.EvalThread, null, ctor.MethodInfo, new Value[] { iEnumerableValue }).GetPermanentReferenceOfHeapValue();
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
		public static bool IsEnum(this IType type)
		{
			return type.DirectBaseTypes.Select(t => t.FullName).Contains("System.Enum");
		}
		
		/// <summary>
		/// Returns true is this type is System.Object.
		/// </summary>
		public static bool IsSystemDotObject(this IType type)
		{
			return type.FullName == "System.Object";
		}
		
		/// <summary>
		/// Checks whether given type is a primitive type, String, or enum.
		/// </summary>
		public static bool IsAtomic(this IType type)
		{
			return TypeSystemExtensions.IsPrimitiveType(type) || type.FullName == "System.String" || type.Kind == TypeKind.Enum;
		}
		
		/// <summary>
		/// System.Runtime.CompilerServices.GetHashCode method, for obtaining non-overriden hash codes from debuggee.
		/// </summary>
		private static IMethod hashCodeMethod;
		/// <summary>
		/// Invokes RuntimeHelpers.GetHashCode on given value, that is a default hashCode ignoring user overrides.
		/// </summary>
		/// <param name="value">Valid value.</param>
		/// <returns>Hash code of the object in the debugee.</returns>
		public static int InvokeDefaultGetHashCode(this Value value)
		{
			// TODO reimplement check for Process.HasExited (debuggee restarted)
			if (hashCodeMethod == null /*|| hashCodeMethod.Process.HasExited*/) {
				IType runtimeHelpers = DebugType.CreateFromType(value.AppDomain, typeof(System.Runtime.CompilerServices.RuntimeHelpers));
				hashCodeMethod = runtimeHelpers.GetMethods(m => m.FullName == "GetHashCode").FirstOrDefault();
				if (hashCodeMethod == null) {
					throw new DebuggerException(
						"Cannot find method System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode().");
				}
			}
			Value defaultHashCode = Eval.InvokeMethod(WindowsDebugger.EvalThread, DebuggerHelpers.hashCodeMethod, null, new Value[]{value});
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
		
		// Object graph visualizer: collection support temp disabled (porting to new NRefactory).
		/*
		/// <summary>
		/// Evaluates 'System.Collections.ICollection.Count' on given Value.
		/// </summary>
		/// <exception cref="GetValueException">Evaluating System.Collections.ICollection.Count on targetObject failed.</exception>
		public static int GetIListCount(this Value list)
		{
			var iCollectionType = list.Type.GetInterface(typeof(System.Collections.ICollection).FullName);
			if (iCollectionType == null)
				throw new GetValueException("Object does not implement System.Collections.ICollection");
			// Do not get string representation since it can be printed in hex
			return (int)list.GetPropertyValue(WindowsDebugger.EvalThread, iCollectionType.GetProperty("Count")).PrimitiveValue;
		}*/
		
		// Object graph visualizer: collection support temp disabled (porting to new NRefactory).
		/*
		/// <summary>
		/// Evaluates 'System.Collection.IList.Item(i)' on given Value.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static Value GetIListItem(this Value target, int index)
		{
			var iListType = target.Type.GetInterface(typeof(System.Collections.IList).FullName);
			if (iListType == null)
				throw new GetValueException("Object does not implement System.Collections.IList");
			DebugPropertyInfo indexerProperty = (DebugPropertyInfo)iListType.GetProperty("Item");
			if (indexerProperty == null)
				throw new GetValueException("The object does not have an indexer property");
			return target.GetPropertyValue(WindowsDebugger.EvalThread, indexerProperty, Eval.CreateValue(WindowsDebugger.EvalThread, index));
		}*/
		
		// Object graph visualizer: collection support temp disabled (porting to new NRefactory).
		/*
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
		}*/
	}
}
