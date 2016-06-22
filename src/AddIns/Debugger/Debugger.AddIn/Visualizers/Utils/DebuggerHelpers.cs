// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.CSharp;
using Debugger.AddIn.TreeModel;
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
			ParameterizedType iEnumerableType;
			IType itemType;
			if (!iEnumerableValue.Type.ResolveIEnumerableImplementation(out iEnumerableType, out itemType))
				throw new GetValueException("Value is not IEnumerable");
			return ValueNode.CreateListFromIEnumerable(iEnumerableType, iEnumerableValue).GetPermanentReferenceOfHeapValue();
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
		/// Returns true is this type is System.Object.
		/// </summary>
		public static bool IsSystemDotObject(this IType type)
		{
			return type.IsKnownType(KnownTypeCode.Object);
		}
		
		/// <summary>
		/// Returns true if given type is a primitive type, String, or enum.
		/// </summary>
		public static bool IsAtomic(this IType type)
		{
			return TypeSystemExtensions.IsPrimitiveType(type) || type.IsKnownType(KnownTypeCode.String) || type.Kind == TypeKind.Enum;
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
			if (hashCodeMethod == null) {
				hashCodeMethod = findDebuggeeHashCodeMethod(value);
			}
			Value valueHashCode;
			try {
				valueHashCode = Eval.InvokeMethod(WindowsDebugger.EvalThread, hashCodeMethod, null, new Value[]{value});
			} catch(InvalidComObjectException) {
				// debuggee was restarted
				hashCodeMethod = findDebuggeeHashCodeMethod(value);
				valueHashCode = Eval.InvokeMethod(WindowsDebugger.EvalThread, hashCodeMethod, null, new Value[]{value});
			}
			return (int)valueHashCode.PrimitiveValue;
		}
		
		private static IMethod findDebuggeeHashCodeMethod(Value value)
		{
			IType runtimeHelpers =
				value.Type.GetDefinition().Compilation.FindType(
					typeof(System.Runtime.CompilerServices.RuntimeHelpers)
				).GetDefinition();
			hashCodeMethod = runtimeHelpers.GetMethods(m => m.Name == "GetHashCode" && m.Parameters.Count == 1).FirstOrDefault();
			if (hashCodeMethod == null) {
				throw new DebuggerException(
					"Cannot find method System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode().");
			}
			return hashCodeMethod;
		}
		
		/// <summary>
		/// Formats System.Type to the C# format, that is generic parameters in angle brackets.
		/// </summary>
		public static string FormatNameCSharp(this IType type)
		{
			return new CSharpAmbience().ConvertType(type);
		}
		
		public static IEnumerable<IMember> GetFieldsAndNonIndexedProperties(this IType type, GetMemberOptions options = GetMemberOptions.None) {
			IEnumerable<IMember> fields = type.GetFields(f => !f.IsConst, options);
			IEnumerable<IMember> properties = type.GetProperties(p => p.CanGet && p.Parameters.Count == 0, options);
			return fields.Concat(properties);
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
