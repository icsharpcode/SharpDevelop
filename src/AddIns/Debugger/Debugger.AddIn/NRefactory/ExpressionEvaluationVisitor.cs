// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn
{
	public enum SupportedLanguage
	{
		CSharp,
		VBNet
	}
	
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
		
		public ExpressionEvaluationVisitor(StackFrame context, Thread evalThread, ICompilation debuggerTypeSystem)
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
		}
		
		public Value Convert(ResolveResult result)
		{
			if (result.IsCompileTimeConstant && !result.IsError)
				return Eval.CreateValue(evalThread, result.ConstantValue);
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
			var importedMember = debuggerTypeSystem.Import(result.Member);
			if (importedMember == null)
				throw new GetValueException("Member not found!");
			Value target = null;
			if (!importedMember.IsStatic)
				target = Convert(result.TargetResult);
			Value val = Value.GetMemberValue(evalThread, target, importedMember);
			if (val == null)
				throw new GetValueException("Member not found!");
			return val;
		}
		
		Value Visit(OperatorResolveResult result)
		{
			switch (result.OperatorType) {
				case ExpressionType.Assign:
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
			throw new InvalidOperationException();
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
			if (operatorType == BinaryOperatorType.Add &&
			    (lhsRR.Type.IsKnownType(KnownTypeCode.String) || rhsRR.Type.IsKnownType(KnownTypeCode.String))) {
				var method = debuggerTypeSystem.FindType(KnownTypeCode.String)
					.GetMethods(m => m.Name == "Concat" && m.Parameters.Count == 2)
					.Single(m => m.Parameters.All(p => p.Type.IsKnownType(KnownTypeCode.Object)));
				return InvokeMethod(null, method, lhs, rhs);
			}
			throw new InvalidOperationException();
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
		
		/// <remark
		/// See $7.10.10 of C# 4 Spec for details.
		/// </remarks>
		Value Visit(TypeIsResolveResult result)
		{
			var importedType = NullableType.GetUnderlyingType(debuggerTypeSystem.Import(result.TargetType));
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
			return Eval.NewObjectNoConstructor(evalThread, debuggerTypeSystem.Import(result.ReferencedType));
		}
		
		Value Visit(TypeResolveResult result)
		{
			throw new GetValueException("Types not supported!");
		}
		
		Value Visit(UnknownMemberResolveResult result)
		{
			throw new GetValueException("Member not found!");
		}
		
		Value Visit(UnknownIdentifierResolveResult result)
		{
			throw new GetValueException("Identifier not found!");
		}
		
		Value Visit(ArrayAccessResolveResult result)
		{
			var val = Convert(result.Array);
			return val.GetArrayElement(result.Indexes.Select(rr => (int)Convert(rr).PrimitiveValue).ToArray());
		}
		
		Value Visit(ArrayCreateResolveResult result)
		{
			throw new NotImplementedException();
		}
		
		Value Visit(ConversionResolveResult result)
		{
			var val = Convert(result.Input);
			if (result.Conversion.IsBoxingConversion)
				return val;
			else if (result.Conversion.IsIdentityConversion)
				return val;
			else if (result.Conversion.IsNumericConversion) {
				var convVal = CSharpPrimitiveCast.Cast(ReflectionHelper.GetTypeCode(result.Type), val.PrimitiveValue, false);
				return Eval.CreateValue(evalThread, convVal);
			} else if (result.Conversion.IsUserDefined)
				return InvokeMethod(null, result.Conversion.Method, val);
			else if (result.Conversion.IsReferenceConversion && result.Conversion.IsImplicit) {
				return val;
			} else
				throw new NotImplementedException();
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
			var importedMember = debuggerTypeSystem.Import(result.Member);
			if (importedMember == null)
				throw new GetValueException("Member not found!");
			IMethod usedMethod;
			if (importedMember is IProperty) {
				var prop = (IProperty)importedMember;
				if (!prop.CanGet)
					throw new GetValueException("Indexer does not have a getter.");
				usedMethod = prop.Getter;
			} else if (importedMember is IMethod) {
				usedMethod = (IMethod)importedMember;
			} else
				throw new GetValueException("Invoked member must be a method or property");
			Value target = null;
			if (!usedMethod.IsStatic)
				target = Convert(result.TargetResult);
			return InvokeMethod(target, usedMethod, result.Arguments.Select(rr => Convert(rr)).ToArray());
		}
		
		Value Visit(NamespaceResolveResult result)
		{
			throw new GetValueException("Namespace not supported!");
		}
		
		Value InvokeMethod(Value thisValue, IMethod method, params Value[] arguments)
		{
			method = debuggerTypeSystem.Import(method);
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
				sb.Append(val.Type.Name);
				sb.Append(" {");
				bool first = true;
				foreach(Value item in val.GetArrayElements()) {
					if (!first) sb.Append(", ");
					first = false;
					sb.Append(FormatValue(evalThread, item));
				}
				sb.Append("}");
				return sb.ToString();
			} else if (val.Type.GetAllBaseTypeDefinitions().Any(def => def.IsKnownType(KnownTypeCode.ICollection))) {
				StringBuilder sb = new StringBuilder();
				sb.Append(val.Type.Name);
				sb.Append(" {");
				val = val.GetPermanentReference(evalThread);
				var countProp = val.Type.GetProperties(p => p.Name == "Count" && !p.IsExplicitInterfaceImplementation).Single();
				int count = (int)val.GetMemberValue(evalThread, countProp).PrimitiveValue;
				for(int i = 0; i < count; i++) {
					if (i > 0) sb.Append(", ");
					var itemProperty = val.Type.GetProperties(p => p.IsIndexer && p.Name == "Item" && !p.IsExplicitInterfaceImplementation).Single();
					Value item = val.GetPropertyValue(evalThread, itemProperty, Eval.CreateValue(evalThread, i));
					sb.Append(FormatValue(evalThread, item));
				}
				sb.Append("}");
				return sb.ToString();
			} else if (val.Type.IsKnownType(KnownTypeCode.Char)) {
				return "'" + CSharpOutputVisitor.ConvertChar((char)val.PrimitiveValue) + "'";
			} else if (val.Type.IsKnownType(KnownTypeCode.String)) {
				return "\"" + CSharpOutputVisitor.ConvertString((string)val.PrimitiveValue) + "\"";
			} else if (val.Type.IsPrimitiveType()) {
				return CSharpOutputVisitor.PrintPrimitiveValue(val.PrimitiveValue);
			} else {
				return val.InvokeToString(evalThread);
			}
		}
		
	}
}
