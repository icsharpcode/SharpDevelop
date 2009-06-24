// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.NRefactory.Visitors;
using System;
using System.Collections.Generic;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn
{
	public class EvaluateAstVisitor: NotImplementedAstVisitor
	{
		StackFrame context;
		
		public StackFrame Context {
			get { return context; }
		}
		
		public EvaluateAstVisitor(StackFrame context)
		{
			this.context = context;
		}
		
		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			// Calculate right first so that left does not get invalidated by its calculation
			Value right = ((Value)assignmentExpression.Right.AcceptVisitor(this, null)).GetPermanentReference();
			Value left = (Value)assignmentExpression.Left.AcceptVisitor(this, null);
			if (!left.IsReference && left.Type.FullName != right.Type.FullName) {
				throw new GetValueException(string.Format("Type {0} expected, {1} seen", left.Type.FullName, right.Type.FullName));
			}
			left.SetValue(right);
			return right;
		}
		
		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			foreach(INode statement in blockStatement.Children) {
				statement.AcceptVisitor(this, null);
			}
			return null;
		}
		
		public override object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			return null;
		}
		
		public override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			expressionStatement.Expression.AcceptVisitor(this, null);
			return null;
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string identifier = identifierExpression.Identifier;
			
			Value arg = context.GetArgumentValue(identifier);
			if (arg != null) return arg;
			
			Value local = context.GetLocalVariableValue(identifier);
			if (local != null) return local;
			
			if (!context.MethodInfo.IsStatic) {
				Value member = context.GetThisValue().GetMemberValue(identifier);
				if (member != null) return member;
			} else {
				MemberInfo memberInfo = context.MethodInfo.DeclaringType.GetMember(identifier);
				if (memberInfo != null && memberInfo.IsStatic) {
					return Value.GetMemberValue(null, memberInfo, null);
				}
			}
			
			throw new GetValueException("Identifier \"" + identifier + "\" not found in this context");
		}
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			List<Value> indexes = new List<Value>();
			foreach(Expression indexExpr in indexerExpression.Indexes) {
				Value indexValue = ((Value)indexExpr.AcceptVisitor(this, null)).GetPermanentReference();
				indexes.Add(indexValue);
			}
			
			Value target = (Value)indexerExpression.TargetObject.AcceptVisitor(this, null);
			
			if (target.Type.IsArray) {
				List<int> intIndexes = new List<int>();
				foreach(Value index in indexes) {
					if (!index.Type.IsInteger) throw new GetValueException("Integer expected for indexer");
					intIndexes.Add((int)index.PrimitiveValue);
				}
				return target.GetArrayElement(intIndexes.ToArray());
			}
			
			PropertyInfo pi = target.Type.GetProperty("Item");
			if (pi == null) throw new GetValueException("The object does not have an indexer property");
			return target.GetPropertyValue(pi, indexes.ToArray());
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Value target;
			string methodName;
			MemberReferenceExpression memberRef = invocationExpression.TargetObject as MemberReferenceExpression;
			if (memberRef != null) {
				target = ((Value)memberRef.TargetObject.AcceptVisitor(this, null)).GetPermanentReference();
				methodName = memberRef.MemberName;
			} else {
				IdentifierExpression ident = invocationExpression.TargetObject as IdentifierExpression;
				if (ident != null) {
					target = context.GetThisValue();
					methodName = ident.Identifier;
				} else {
					throw new GetValueException("Member reference expected for method invocation");
				}
			}
			List<Value> args = new List<Value>();
			foreach(Expression expr in invocationExpression.Arguments) {
				args.Add(((Value)expr.AcceptVisitor(this, null)).GetPermanentReference());
			}
			MethodInfo method = target.Type.GetMember(methodName, BindingFlags.Method | BindingFlags.IncludeSuperType) as MethodInfo;
			if (method == null) {
				throw new GetValueException("Method " + methodName + " not found");
			}
			return target.InvokeMethod(method, args.ToArray());
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			Value target = (Value)memberReferenceExpression.TargetObject.AcceptVisitor(this, null);
			Value member = target.GetMemberValue(memberReferenceExpression.MemberName);
			if (member != null) {
				return member;
			} else {
				throw new GetValueException("Member \"" + memberReferenceExpression.MemberName + "\" not found");
			}
		}
		
		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return parenthesizedExpression.Expression.AcceptVisitor(this, null);
		}
		
		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			return Eval.CreateValue(context.Process, primitiveExpression.Value);
		}
		
		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return context.GetThisValue();
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			Value left = ((Value)binaryOperatorExpression.Left.AcceptVisitor(this, null)).GetPermanentReference();
			Value right = ((Value)binaryOperatorExpression.Right.AcceptVisitor(this, null)).GetPermanentReference();
			
			object result = VisitBinaryOperatorExpressionInternal(left, right, binaryOperatorExpression.Op);
			// Conver long to int if possible
			if (result is long && int.MinValue <= (long)result && (long)result <= int.MaxValue) result = (int)(long)result;
			return Eval.CreateValue(context.Process, result);
		}
		
		public object VisitBinaryOperatorExpressionInternal(Value leftValue, Value rightValue, BinaryOperatorType op)
		{
			object left = leftValue.Type.IsPrimitive ? leftValue.PrimitiveValue : null;
			object right = rightValue.Type.IsPrimitive ? rightValue.PrimitiveValue : null;
			
			// Both are classes - do reference comparison
			if (left == null && right == null) {
				if (leftValue.IsNull || rightValue.IsNull) {
					return leftValue.IsNull && rightValue.IsNull;
				} else {
					// TODO: Make sure this works for byrefs and arrays
					return leftValue.Address == rightValue.Address;
				}
			}
			
			if (left == null && right != null) {
				throw new GetValueException("The left side of binary operation is not primitive value");
			}
			
			if (left != null && right == null) {
				throw new GetValueException("The right side of binary operation is not primitive value");
			}
			
			// Both are primitive - do value the operation
			if (left != null && right != null) {
				try {
					// Note the order of these tests is significant (eg "5" + 6:  "56" or 11?)
					
					// String operation
					if (left is string || right is string) {
						string a = Convert.ToString(left);
						string b = Convert.ToString(right);
						switch (op) {
							case BinaryOperatorType.Equality:            return a == b;
							case BinaryOperatorType.InEquality:          return a != b;
							case BinaryOperatorType.Add:                 return a + b;
							case BinaryOperatorType.ReferenceEquality:   return a == b;
							case BinaryOperatorType.ReferenceInequality: return a != b;
							case BinaryOperatorType.NullCoalescing:      return a ?? b;
						}
					}
					
					// Bool operation
					if (left is bool || right is bool) {
						bool a = Convert.ToBoolean(left);
						bool b = Convert.ToBoolean(right);
						switch (op) {
							case BinaryOperatorType.BitwiseAnd:  return a & b;
							case BinaryOperatorType.BitwiseOr:   return a | b;
							case BinaryOperatorType.LogicalAnd:  return a && b;
							case BinaryOperatorType.LogicalOr:   return a || b;
							case BinaryOperatorType.ExclusiveOr: return a ^ b;
							
							case BinaryOperatorType.Equality:           return a == b;
							case BinaryOperatorType.InEquality:         return a != b;
							
							case BinaryOperatorType.ReferenceEquality:   return a == b;
							case BinaryOperatorType.ReferenceInequality: return a != b;
						}
					}
					
					// Float operation
					if (left is double || left is float || right is double || right is float) {
						double a = Convert.ToDouble(left);
						double b = Convert.ToDouble(right);
						switch (op) {
							case BinaryOperatorType.GreaterThan:        return a > b;
							case BinaryOperatorType.GreaterThanOrEqual: return a >= b;
							case BinaryOperatorType.Equality:           return a == b;
							case BinaryOperatorType.InEquality:         return a != b;
							case BinaryOperatorType.LessThan:           return a < b;
							case BinaryOperatorType.LessThanOrEqual:    return a <= b;
							
							case BinaryOperatorType.Add:           return a + b;
							case BinaryOperatorType.Subtract:      return a - b;
							case BinaryOperatorType.Multiply:      return a * b;
							case BinaryOperatorType.Divide:        return a / b;
							case BinaryOperatorType.Modulus:       return a % b;
							case BinaryOperatorType.Concat:        return a + b;
							
							case BinaryOperatorType.ReferenceEquality:   return a == b;
							case BinaryOperatorType.ReferenceInequality: return a != b;
						}
					}
					
					// Integer operation
					if (left is byte || left is sbyte || left is int || left is uint || left is long || left is ulong ||
					    right is byte || right is sbyte || right is int || right is uint || right is long || right is ulong) {
						long a = Convert.ToInt64(left);
						long b = Convert.ToInt64(right);
						switch (op) {
							case BinaryOperatorType.BitwiseAnd:  return a & b;
							case BinaryOperatorType.BitwiseOr:   return a | b;
							case BinaryOperatorType.ExclusiveOr: return a ^ b;
							
							case BinaryOperatorType.GreaterThan:        return a > b;
							case BinaryOperatorType.GreaterThanOrEqual: return a >= b;
							case BinaryOperatorType.Equality:           return a == b;
							case BinaryOperatorType.InEquality:         return a != b;
							case BinaryOperatorType.LessThan:           return a < b;
							case BinaryOperatorType.LessThanOrEqual:    return a <= b;
							
							case BinaryOperatorType.Add:           return a + b;
							case BinaryOperatorType.Subtract:      return a - b;
							case BinaryOperatorType.Multiply:      return a * b;
							case BinaryOperatorType.Divide:        return a / b;
							case BinaryOperatorType.Modulus:       return a % b;
							case BinaryOperatorType.Concat:        return a + b;
							case BinaryOperatorType.ShiftLeft:     return a << Convert.ToInt32(b);
							case BinaryOperatorType.ShiftRight:    return a >> Convert.ToInt32(b);
							
							case BinaryOperatorType.ReferenceEquality:   return a == b;
							case BinaryOperatorType.ReferenceInequality: return a != b;
						}
					}
					
					// Char operation
					if (left is char || right is char) {
						char a = Convert.ToChar(left);
						char b = Convert.ToChar(right);
						switch (op) {
							case BinaryOperatorType.BitwiseAnd:  return a & b;
							case BinaryOperatorType.BitwiseOr:   return a | b;
							case BinaryOperatorType.ExclusiveOr: return a ^ b;
							
							case BinaryOperatorType.GreaterThan:        return a > b;
							case BinaryOperatorType.GreaterThanOrEqual: return a >= b;
							case BinaryOperatorType.Equality:           return a == b;
							case BinaryOperatorType.InEquality:         return a != b;
							case BinaryOperatorType.LessThan:           return a < b;
							case BinaryOperatorType.LessThanOrEqual:    return a <= b;
							
							case BinaryOperatorType.Add:           return a + b;
							case BinaryOperatorType.Subtract:      return a - b;
							case BinaryOperatorType.Multiply:      return a * b;
							case BinaryOperatorType.Divide:        return a / b;
							case BinaryOperatorType.Modulus:       return a % b;
							case BinaryOperatorType.Concat:        return a + b;
							case BinaryOperatorType.ShiftLeft:     return a << b;
							case BinaryOperatorType.ShiftRight:    return a >> b;
							
							case BinaryOperatorType.ReferenceEquality:   return a == b;
							case BinaryOperatorType.ReferenceInequality: return a != b;
						}
					}
				} catch(FormatException e) {
					throw new GetValueException("Conversion error: " + e.Message);
				} catch(InvalidCastException e) {
					throw new GetValueException("Conversion error: " + e.Message);
				} catch(OverflowException e) {
					throw new GetValueException("Conversion error: " + e.Message);
				}
			}
			
			throw new DebuggerException("Unreachable code");
		}
	}
}
