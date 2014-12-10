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

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Debugger.MetaData;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn
{
	public static class Extensions
	{
		public static ResolveResult ToResolveResult(this Value value, StackFrame context)
		{
			return new ValueResolveResult(value);
		}
	}
	
	public class ValueResolveResult : ResolveResult
	{
		Value value;
		
		public ValueResolveResult(Value value) : base(value.Type)
		{
			this.value = value;
		}
		
		public Value Value {
			get { return value; }
		}
		
		public override bool IsCompileTimeConstant {
			get { return value.Type.IsKnownType(KnownTypeCode.String) || value.Type.IsPrimitiveType(); }
		}
		
		public override object ConstantValue {
			get { return value.PrimitiveValue; }
		}
	}
	
	public class ExpressionEvaluationVisitor
	{
		StackFrame context;
		ICompilation debuggerTypeSystem;
		Thread evalThread;
		bool allowMethodInvoke;
		bool allowSetValue;
		
		public ExpressionEvaluationVisitor(StackFrame context, Thread evalThread, ICompilation debuggerTypeSystem,
		                                   bool allowMethodInvoke = false, bool allowSetValue = false)
		{
			if (evalThread == null)
				throw new ArgumentNullException("evalThread");
			if (context == null)
				throw new ArgumentNullException("context");
			if (debuggerTypeSystem == null)
				throw new ArgumentNullException("debuggerTypeSystem");
			this.context = context;
			this.debuggerTypeSystem = debuggerTypeSystem;
			this.evalThread = evalThread;
			this.allowMethodInvoke = allowMethodInvoke;
			this.allowSetValue = allowSetValue;
		}
		
		/// <summary>
		/// Imports a type into the debugger's type system, and into the current generic context.
		/// </summary>
		IType Import(IType type)
		{
			IType importedType = debuggerTypeSystem.Import(type);
			if (importedType != null)
				return importedType.AcceptVisitor(context.MethodInfo.Substitution);
			else
				return null;
		}
		
		/// <summary>
		/// Imports a type into the debugger's type system, and into the current generic context.
		/// </summary>
		IMember Import(IMember member)
		{
			IMember importedMember = debuggerTypeSystem.Import(member);
			if (importedMember != null)
				return importedMember.Specialize(context.MethodInfo.Substitution);
			else
				return null;
		}
		
		public Value Convert(ResolveResult result)
		{
			if (result.IsCompileTimeConstant && !result.IsError) {
				var type = Import(result.Type);
				if (type == null)
					throw new GetValueException("Error: cannot find '{0}'.", result.Type.FullName);
				return Eval.CreateValue(evalThread, result.ConstantValue, type);
			}
			return Visit((dynamic)result);
		}
		
		Value Visit(ResolveResult result)
		{
			if (result is ErrorResolveResult) {
				var err = (ErrorResolveResult)result;
				if (!string.IsNullOrWhiteSpace(err.Message))
					throw new GetValueException(err.Message);
			}
			
			if (result.IsError)
				throw new GetValueException("Unknown error");
			if (result.ConstantValue == null && result.Type.Equals(SpecialType.NullType))
				return Eval.CreateValue(evalThread, null);
			throw new GetValueException("Unsupported language construct: " + result.GetType().Name);
		}
		
		Value Visit(ValueResolveResult result)
		{
			return result.Value;
		}
		
		Value Visit(ThisResolveResult result)
		{
			return context.GetThisValue(true);
		}
		
		Value Visit(MemberResolveResult result)
		{
			var importedMember = Import(result.Member);
			if (importedMember == null)
				throw new GetValueException("Member not found!");
			Value target = null;
			if (!importedMember.IsStatic) {
				if (importedMember.DeclaringType.Equals(context.MethodInfo.DeclaringType) && result.TargetResult == null)
					target = context.GetThisValue(true);
				else if (result.TargetResult == null)
					throw new GetValueException("An object reference is required for the non-static field, method, or property '" + importedMember.FullName + "'");
				else
					target = Convert(result.TargetResult);
			}
			if (!allowMethodInvoke && (importedMember is IMethod))
				throw new InvalidOperationException("Method invocation not allowed in the current context!");
			Value val = Value.GetMemberValue(evalThread, target, importedMember);
			if (val == null)
				throw new GetValueException("Member not found!");
			return val;
		}
		
		Value Visit(OperatorResolveResult result)
		{
			switch (result.OperatorType) {
				case ExpressionType.Assign:
					if (!allowSetValue)
						throw new GetValueException("Setting values is not allowed in the current context!");
					Debug.Assert(result.Operands.Count == 2);
					return VisitAssignment((dynamic)result.Operands[0], (dynamic)result.Operands[1]);
				case ExpressionType.Add:
					return VisitBinaryOperator(result, BinaryOperatorType.Add, false);
				case ExpressionType.AddChecked:
					return VisitBinaryOperator(result, BinaryOperatorType.Add, true);
				case ExpressionType.Subtract:
					return VisitBinaryOperator(result, BinaryOperatorType.Subtract, false);
				case ExpressionType.SubtractChecked:
					return VisitBinaryOperator(result, BinaryOperatorType.Subtract, true);
				case ExpressionType.Multiply:
					return VisitBinaryOperator(result, BinaryOperatorType.Multiply, false);
				case ExpressionType.MultiplyChecked:
					return VisitBinaryOperator(result, BinaryOperatorType.Multiply, true);
				case ExpressionType.Divide:
					return VisitBinaryOperator(result, BinaryOperatorType.Divide, false);
				case ExpressionType.Modulo:
					return VisitBinaryOperator(result, BinaryOperatorType.Modulus, false);

				case ExpressionType.And:
					return VisitBinaryOperator(result, BinaryOperatorType.BitwiseAnd);
				case ExpressionType.AndAlso:
					return VisitConditionalOperator(result, BinaryOperatorType.BitwiseAnd);
				case ExpressionType.Or:
					return VisitBinaryOperator(result, BinaryOperatorType.BitwiseOr);
				case ExpressionType.OrElse:
					return VisitConditionalOperator(result, BinaryOperatorType.BitwiseOr);
				case ExpressionType.ExclusiveOr:
					return VisitBinaryOperator(result, BinaryOperatorType.ExclusiveOr);
				case ExpressionType.Not:
					return VisitUnaryOperator(result, UnaryOperatorType.Not);
//				case ExpressionType.OnesComplement:
					

				case ExpressionType.LeftShift:
					return VisitBinaryOperator(result, BinaryOperatorType.ShiftLeft);
				case ExpressionType.RightShift:
					return VisitBinaryOperator(result, BinaryOperatorType.ShiftRight);
					
				case ExpressionType.Equal:
					return VisitBinaryOperator(result, BinaryOperatorType.Equality);
				case ExpressionType.NotEqual:
					return VisitBinaryOperator(result, BinaryOperatorType.InEquality);
				case ExpressionType.GreaterThan:
					return VisitBinaryOperator(result, BinaryOperatorType.GreaterThan);
				case ExpressionType.GreaterThanOrEqual:
					return VisitBinaryOperator(result, BinaryOperatorType.GreaterThanOrEqual);
				case ExpressionType.LessThan:
					return VisitBinaryOperator(result, BinaryOperatorType.LessThan);
				case ExpressionType.LessThanOrEqual:
					return VisitBinaryOperator(result, BinaryOperatorType.LessThanOrEqual);
				case ExpressionType.Conditional:
					return VisitTernaryOperator(result);
				default:
					throw new GetValueException("Unsupported operator: " + result.OperatorType);
			}
		}
		
		Value VisitTernaryOperator(OperatorResolveResult result)
		{
			Debug.Assert(result.Operands.Count == 3);
			var condition = Convert(result.Operands[0]);
			if (!condition.Type.IsKnownType(KnownTypeCode.Boolean))
				throw new GetValueException("Boolean expression expected!");
			if ((bool)condition.PrimitiveValue)
				return Convert(result.Operands[1]);
			return Convert(result.Operands[2]);
		}
		
		Value VisitAssignment(ResolveResult lhs, ResolveResult rhs)
		{
			throw new GetValueException("Assignment not supported!");
		}
		
		Value VisitAssignment(LocalResolveResult lhs, ResolveResult rhs)
		{
			var value = Convert(rhs);
			if (lhs.IsParameter)
				context.GetArgumentValue(lhs.Variable.Name).SetValue(evalThread, value);
			else
				context.GetLocalVariableValue(lhs.Variable.Name).SetValue(evalThread, value);
			return value;
		}
		
		Value VisitUnaryOperator(OperatorResolveResult result, UnaryOperatorType operatorType, bool checkForOverflow = false)
		{
			Debug.Assert(result.Operands.Count == 1);
			var operand = Convert(result.Operands[0]).ToResolveResult(context);
			CSharpResolver resolver = new CSharpResolver(debuggerTypeSystem).WithCheckForOverflow(checkForOverflow);
			var val = resolver.ResolveUnaryOperator(operatorType, operand);
			if (val.IsCompileTimeConstant)
				return Convert(val);
			throw new GetValueException("Operator {0} is not supported for {1}!", operatorType, new CSharpAmbience().ConvertType(operand.Type));
		}
		
		Value VisitBinaryOperator(OperatorResolveResult result, BinaryOperatorType operatorType, bool checkForOverflow = false)
		{
			Debug.Assert(result.Operands.Count == 2);
			Debug.Assert(operatorType != BinaryOperatorType.ConditionalAnd && operatorType != BinaryOperatorType.ConditionalOr && operatorType != BinaryOperatorType.NullCoalescing);
			var lhs = Convert(result.Operands[0]).GetPermanentReference(evalThread);
			var rhs = Convert(result.Operands[1]).GetPermanentReference(evalThread);
			if (result.UserDefinedOperatorMethod != null)
				return InvokeMethod(null, result.UserDefinedOperatorMethod, lhs, rhs);
			var lhsRR = lhs.ToResolveResult(context);
			var rhsRR = rhs.ToResolveResult(context);
			CSharpResolver resolver = new CSharpResolver(debuggerTypeSystem).WithCheckForOverflow(checkForOverflow);
			var val = resolver.ResolveBinaryOperator(operatorType, lhsRR, rhsRR);
			if (val.IsCompileTimeConstant)
				return Convert(val);
			switch (operatorType) {
				case BinaryOperatorType.Equality:
				case BinaryOperatorType.InEquality:
					bool equality = (operatorType == BinaryOperatorType.Equality);
					if (lhsRR.Type.IsKnownType(KnownTypeCode.String) && rhsRR.Type.IsKnownType(KnownTypeCode.String)) {
						if (lhs.IsNull || rhs.IsNull) {
							bool bothNull = lhs.IsNull && rhs.IsNull;
							return Eval.CreateValue(evalThread, equality ? bothNull : !bothNull);
						} else {
							bool equal = (string)lhs.PrimitiveValue == (string)rhs.PrimitiveValue;
							return Eval.CreateValue(evalThread, equality ? equal : !equal);
						}
					}
					if (lhsRR.Type.IsReferenceType != false && rhsRR.Type.IsReferenceType != false) {
						// Reference comparison
						if (lhs.IsNull || rhs.IsNull) {
							bool bothNull = lhs.IsNull && rhs.IsNull;
							return Eval.CreateValue(evalThread, equality ? bothNull : !bothNull);
						} else {
							bool equal = lhs.Address == rhs.Address;
							return Eval.CreateValue(evalThread, equality ? equal : !equal);
						}
					}
					goto default;
				case BinaryOperatorType.Add:
					if (lhsRR.Type.IsKnownType(KnownTypeCode.String) || rhsRR.Type.IsKnownType(KnownTypeCode.String)) {
						var method = debuggerTypeSystem.FindType(KnownTypeCode.String)
							.GetMethods(m => m.Name == "Concat" && m.Parameters.Count == 2)
							.Single(m => m.Parameters.All(p => p.Type.IsKnownType(KnownTypeCode.Object)));
						return InvokeMethod(null, method, lhs, rhs);
					}
					goto default;
				default:
					throw new GetValueException("Operator {0} is not supported for {1} and {2}!", operatorType, new CSharpAmbience().ConvertType(lhsRR.Type), new CSharpAmbience().ConvertType(rhsRR.Type));
			}
		}
		
		Value VisitConditionalOperator(OperatorResolveResult result, BinaryOperatorType bitwiseOperatorType)
		{
			Debug.Assert(result.Operands.Count == 2);
			var lhs = Convert(result.Operands[0]).GetPermanentReference(evalThread);
			CSharpResolver resolver = new CSharpResolver(debuggerTypeSystem);
			Value condVal;
			if (bitwiseOperatorType == BinaryOperatorType.BitwiseAnd)
				condVal = Convert(resolver.ResolveConditionFalse(lhs.ToResolveResult(context)));
			else
				condVal = Convert(resolver.ResolveCondition(lhs.ToResolveResult(context)));
			if ((bool)condVal.PrimitiveValue)
				return lhs;
			var rhs = Convert(result.Operands[1]);
			var val = resolver.ResolveBinaryOperator(bitwiseOperatorType, lhs.ToResolveResult(context), rhs.ToResolveResult(context));
			return Convert(val);
		}
		
		/// <remarks>
		/// See $7.10.10 of C# 4 Spec for details.
		/// </remarks>
		Value Visit(TypeIsResolveResult result)
		{
			var importedType = NullableType.GetUnderlyingType(Import(result.TargetType));
			var val = Convert(result.Input);
			var conversions = CSharpConversions.Get(debuggerTypeSystem);
			bool evalResult = false;
			if (!val.IsNull) {
				IType inputType = NullableType.GetUnderlyingType(val.Type);
				if (inputType.Equals(importedType))
					evalResult = true;
				else if (conversions.IsImplicitReferenceConversion(inputType, importedType))
					evalResult = true;
				else if (conversions.IsBoxingConversion(inputType, importedType))
					evalResult = true;
			}
			return Eval.CreateValue(evalThread, evalResult);
		}
		
		Value Visit(TypeOfResolveResult result)
		{
			var type = Import(result.ReferencedType);
			if (type == null)
				throw new GetValueException("Error: cannot find '{0}'.", result.ReferencedType.FullName);
			return Eval.TypeOf(evalThread, type);
		}
		
		Value Visit(TypeResolveResult result)
		{
			throw new GetValueException("Error: Types not supported.");
		}
		
		Value Visit(UnknownMemberResolveResult result)
		{
			throw new GetValueException("Error: Member '{0}' not found.", result.MemberName);
		}
		
		Value Visit(UnknownIdentifierResolveResult result)
		{
			throw new GetValueException("Error: Identifier '{0}' not declared.", result.Identifier);
		}
		
		Value Visit(ArrayAccessResolveResult result)
		{
			var val = Convert(result.Array).GetPermanentReference(evalThread);
			return val.GetArrayElement(result.Indexes.Select(rr => (uint)(int)Convert(rr).PrimitiveValue).ToArray());
		}
		
		Value Visit(ArrayCreateResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		Value Visit(ConversionResolveResult result)
		{
			if (result.IsError)
				throw new GetValueException("Cannot convert from '{0}' to '{1}'.", new CSharpAmbience().ConvertType(result.Input.Type), new CSharpAmbience().ConvertType(result.Type));
			var val = Convert(result.Input);
			if (result.Conversion.IsBoxingConversion)
				return val;
			if (result.Conversion.IsIdentityConversion)
				return val;
			if (result.Conversion.IsNumericConversion) {
				var convVal = CSharpPrimitiveCast.Cast(ReflectionHelper.GetTypeCode(result.Type), val.PrimitiveValue, false);
				return Eval.CreateValue(evalThread, convVal);
			}
			if (result.Conversion.IsUserDefined)
				return InvokeMethod(null, result.Conversion.Method, val);
			if (result.Conversion.IsReferenceConversion && result.Conversion.IsImplicit)
				return val;
			if (result.Conversion.IsNullLiteralConversion)
				return val;
			throw new GetValueException("conversion '{0}' not implemented!", result.Conversion);
		}
		
		Value Visit(LocalResolveResult result)
		{
			if (result.IsParameter)
				return context.GetArgumentValue(result.Variable.Name);
			return context.GetLocalVariableValue(result.Variable.Name);
		}
		
		Value Visit(AmbiguousMemberResolveResult result)
		{
			throw new GetValueException("Ambiguous member: " + result.Member.FullName);
		}
		
		Value Visit(InvocationResolveResult result)
		{
			// InvokeMethod() will import the member, so work in the original compilation to find the method to invoke:
			IMethod usedMethod;
			if (result.Member is IProperty) {
				var prop = (IProperty)result.Member;
				if (!prop.CanGet)
					throw new GetValueException("Indexer does not have a getter.");
				usedMethod = prop.Getter;
			} else if (result.Member is IMethod) {
				if (!allowMethodInvoke)
					throw new InvalidOperationException("Method invocation not allowed in the current context!");
				usedMethod = (IMethod)result.Member;
			} else
				throw new GetValueException("Invoked member must be a method or property");
			Value target = null;
			if (!usedMethod.IsStatic && !usedMethod.IsConstructor)
				target = Convert(result.TargetResult).GetPermanentReference(evalThread);
			return InvokeMethod(target, usedMethod, result.Arguments.Select(rr => Convert(rr).GetPermanentReference(evalThread)).ToArray());
		}
		
		Value Visit(NamespaceResolveResult result)
		{
			throw new GetValueException("Namespace not supported!");
		}
		
		Value InvokeMethod(Value thisValue, IMethod method, params Value[] arguments)
		{
			method = Import(method) as IMethod;
			if (method == null)
				throw new GetValueException("Method not found!");
			return Value.InvokeMethod(evalThread, thisValue, method, arguments);
		}
		
		public static string FormatValue(Thread evalThread, Value val)
		{
			if (val.IsNull) {
				return "null";
			} else if (val.Type.Kind == TypeKind.Array) {
				StringBuilder sb = new StringBuilder();
				sb.Append(new CSharpAmbience().ConvertType(val.Type));
				sb.Append(" {");
				bool first = true;
				int size = val.ArrayLength;
				for (int i = 0; i < size; i++) {
					if (!first) sb.Append(", ");
					first = false;
					sb.Append(FormatValue(evalThread, val.GetElementAtPosition(i)));
				}
				sb.Append("}");
				return sb.ToString();
			} else if (val.Type.GetAllBaseTypeDefinitions().Any(def => def.IsKnownType(KnownTypeCode.ICollection) || def.IsKnownType(KnownTypeCode.ICollectionOfT))) {
				var countProp = val.Type.GetProperties(p => p.Name == "Count" && !p.IsExplicitInterfaceImplementation).SingleOrDefault();
				if (countProp != null) {
					StringBuilder sb = new StringBuilder();
					sb.Append(new CSharpAmbience().ConvertType(val.Type));
					val = val.GetPermanentReference(evalThread);
					int count = (int)val.GetMemberValue(evalThread, countProp).PrimitiveValue;
					var itemProperty = val.Type.GetProperties(p => p.IsIndexer && p.Name == "Item" && !p.IsExplicitInterfaceImplementation).SingleOrDefault();
					if (itemProperty != null) {
						sb.Append(" {");
						for (int i = 0; i < count; i++) {
							if (i > 0)
								sb.Append(", ");
							Value item = val.GetPropertyValue(evalThread, itemProperty, Eval.CreateValue(evalThread, i));
							sb.Append(FormatValue(evalThread, item));
						}
						sb.Append("}");
					} else {
						sb.AppendFormat(" ({0} elements)", count);
					}
					return sb.ToString();
				}
			} else if (val.Type.IsKnownType(KnownTypeCode.String) || val.Type.IsPrimitiveType()) {
				return TextWriterTokenWriter.PrintPrimitiveValue(val.PrimitiveValue);
			}
			
			return val.InvokeToString(evalThread);
		}
		
	}
	
	public class ResolveResultPrettyPrinter
	{
		ResolveResultPrettyPrinter()
		{
		}
		
		public static string Print(ResolveResult result)
		{
			try {
				return new ResolveResultPrettyPrinter().PrintInternal(result);
			} catch (NotImplementedException ex) {
				SD.Log.Warn(ex);
				return "";
			}
		}
		
		string PrintInternal(ResolveResult result)
		{
			if (result == null)
				return "";
			if (result.IsError)
				return "{Error}";
			return Visit((dynamic)result);
		}
		
		string Visit(ResolveResult result)
		{
			return "Not supported: " + result.GetType().Name;
		}
		
//		string Visit(ValueResolveResult result)
//		{
//			throw new NotImplementedException();
//		}
		
		string Visit(ThisResolveResult result)
		{
			return "this";
		}
		
		string Visit(MemberResolveResult result)
		{
			string text = PrintInternal(result.TargetResult);
			if (!string.IsNullOrWhiteSpace(text))
				text += ".";
			return text + result.Member.Name;
		}
		
		string Visit(OperatorResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		string Visit(TypeIsResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		string Visit(TypeOfResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		string Visit(TypeResolveResult result)
		{
			return new CSharpAmbience().ConvertType(result.Type);
		}
		
		string Visit(UnknownMemberResolveResult result)
		{
			return result.MemberName;
		}
		
		string Visit(UnknownIdentifierResolveResult result)
		{
			return result.Identifier;
		}
		
		string Visit(ArrayAccessResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		string Visit(ArrayCreateResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		string Visit(ConversionResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		string Visit(LocalResolveResult result)
		{
			if (result.IsParameter)
				return result.Variable.Name;
			return result.Variable.Name;
		}
		
		string Visit(AmbiguousMemberResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		string Visit(InvocationResolveResult result)
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append(PrintInternal(result.TargetResult));
			sb.Append('.');
			sb.Append(result.Member.Name);
			
			sb.Append(result.Member is IProperty ? "[" : "(");
			
			bool first = true;
			foreach (var p in result.Member.Parameters) {
				if (first) first = false;
				else sb.Append(", ");
				sb.Append(p.Name);
			}
			
			sb.Append(result.Member is IProperty ? "]" : ")");
			
			return sb.ToString();
		}
		
		string Visit(NamespaceResolveResult result)
		{
			return "namespace " + result.NamespaceName;
		}
	}
}
