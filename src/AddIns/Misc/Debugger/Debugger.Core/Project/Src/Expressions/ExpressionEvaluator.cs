// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.NRefactory.PrettyPrinter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debugger.MetaData;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace Debugger
{
	public class ExpressionEvaluator: NotImplementedAstVisitor
	{
		/// <summary> Evaluate given expression.  If you have expression tree already, use overloads of this method.</summary>
		/// <returns> Returned value or null for statements </returns>
		public static Value Evaluate(string code, SupportedLanguage language, StackFrame context)
		{
			SnippetParser parser = new SnippetParser(language);
			INode astRoot = parser.Parse(code);
			if (parser.Errors.Count > 0) {
				throw new GetValueException(parser.Errors.ErrorOutput);
			}
			if (parser.SnippetType != SnippetType.Expression && parser.SnippetType != SnippetType.Statements) {
				throw new GetValueException("Code must be expression or statement");
			}
			return Evaluate(astRoot, context);
		}
		
		public static Value Evaluate(INode code, Process context)
		{
			if (context.SelectedStackFrame != null) {
				return Evaluate(code, context.SelectedStackFrame);
			} else if (context.SelectedThread.MostRecentStackFrame != null ) {
				return Evaluate(code, context.SelectedThread.MostRecentStackFrame);
			} else {
				// This can happen when needed 'dll' is missing.  This causes an exception dialog to be shown even before the applicaiton starts
				throw new GetValueException("Can not evaluate because the process has no managed stack frames");
			}
		}
		
		public static Value Evaluate(INode code, StackFrame context)
		{
			if (context == null) throw new ArgumentNullException("context");
			if (context.IsInvalid) throw new DebuggerException("The context is no longer valid");
			
			return new ExpressionEvaluator(context).Evaluate(code, false);
		}
		
		public static string FormatValue(Value val)
		{
			if (val == null) {
				return null;
			} if (val.IsNull) {
				return "null";
			} else if (val.Type.IsArray) {
				StringBuilder sb = new StringBuilder();
				sb.Append(val.Type.Name);
				sb.Append(" {");
				bool first = true;
				foreach(Value item in val.GetArrayElements()) {
					if (!first) sb.Append(", ");
					first = false;
					sb.Append(FormatValue(item));
				}
				sb.Append("}");
				return sb.ToString();
			} else if (val.Type.GetInterface(typeof(ICollection).FullName) != null) {
				StringBuilder sb = new StringBuilder();
				sb.Append(val.Type.Name);
				sb.Append(" {");
				val = val.GetPermanentReference();
				int count = (int)val.GetMemberValue("Count").PrimitiveValue;
				for(int i = 0; i < count; i++) {
					if (i > 0) sb.Append(", ");
					PropertyInfo itemProperty = val.Type.GetProperty("Item");
					Value item = val.GetPropertyValue(itemProperty, Eval.CreateValue(val.AppDomain, i));
					sb.Append(FormatValue(item));
				}
				sb.Append("}");
				return sb.ToString();
			} else if (val.Type.IsPrimitive) {
				return val.PrimitiveValue.ToString();
			} else {
				return val.InvokeToString();
			}
		}
		
		public Value Evaluate(INode expression)
		{
			return Evaluate(expression, true);
		}
		
		public Value Evaluate(INode expression, bool permRef)
		{
			// Try to get the value from cache
			// (the cache is cleared when the process is resumed)
			Value val;
			if (context.Process.CachedExpressions.TryGetValue(expression, out val)) {
				if (val == null || !val.IsInvalid) {
					return val;
				}
			}
			
			Stopwatch watch = new Stopwatch();
			watch.Start();
			try {
				val = (Value)expression.AcceptVisitor(this, null);
				if (val != null && permRef)
					val = val.GetPermanentReference();
			} catch (GetValueException e) {
				e.Expression = expression;
				throw;
			} catch (NotImplementedException e) {
				throw new GetValueException(expression, "Language feature not implemented: " + e.Message);
			} finally {
				watch.Stop();
				context.Process.TraceMessage("Evaluated: {0} in {1} ms total", expression.PrettyPrint(), watch.ElapsedMilliseconds);
			}
			
			if (val != null && val.IsInvalid)
				throw new DebuggerException("Expression \"" + expression.PrettyPrint() + "\" is invalid right after evaluation");
			
			// Add the result to cache
			context.Process.CachedExpressions[expression] = val;
			
			return val;
		}
		
		StackFrame context;
		
		public StackFrame Context {
			get { return context; }
		}
		
		ExpressionEvaluator(StackFrame context)
		{
			this.context = context;
		}
		
		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			BinaryOperatorType op;
			switch (assignmentExpression.Op) {
				case AssignmentOperatorType.Assign:        op = BinaryOperatorType.None; break;
				case AssignmentOperatorType.Add:           op = BinaryOperatorType.Add; break;
				case AssignmentOperatorType.ConcatString:  op = BinaryOperatorType.Concat; break;
				case AssignmentOperatorType.Subtract:      op = BinaryOperatorType.Subtract; break;
				case AssignmentOperatorType.Multiply:      op = BinaryOperatorType.Multiply; break;
				case AssignmentOperatorType.Divide:        op = BinaryOperatorType.Divide; break;
				case AssignmentOperatorType.DivideInteger: op = BinaryOperatorType.DivideInteger; break;
				case AssignmentOperatorType.ShiftLeft:     op = BinaryOperatorType.ShiftLeft; break;
				case AssignmentOperatorType.ShiftRight:    op = BinaryOperatorType.ShiftRight; break;
				case AssignmentOperatorType.ExclusiveOr:   op = BinaryOperatorType.ExclusiveOr; break;
				case AssignmentOperatorType.Modulus:       op = BinaryOperatorType.Modulus; break;
				case AssignmentOperatorType.BitwiseAnd:    op = BinaryOperatorType.BitwiseAnd; break;
				case AssignmentOperatorType.BitwiseOr:     op = BinaryOperatorType.BitwiseOr; break;
				case AssignmentOperatorType.Power:         op = BinaryOperatorType.Power; break;
				default: throw new GetValueException("Unknown operator " + assignmentExpression.Op);
			}
			
			Value right;
			if (op == BinaryOperatorType.None) {
				right = Evaluate(assignmentExpression.Right);
			} else {
				BinaryOperatorExpression binOpExpr = new BinaryOperatorExpression();
				binOpExpr.Left  = assignmentExpression.Left;
				binOpExpr.Op    = op;
				binOpExpr.Right = assignmentExpression.Right;
				right = Evaluate(binOpExpr);
			}
			
			// We can not have perfRef because we need to be able to set the value
			Value left = (Value)assignmentExpression.Left.AcceptVisitor(this, null);
			
			if (left == null) {
				// Can this happen?
				throw new GetValueException(string.Format("\"{0}\" can not be set", assignmentExpression.Left.PrettyPrint()));
			}
			if (!left.IsReference && left.Type.FullName != right.Type.FullName) {
				throw new GetValueException(string.Format("Type {0} expected, {1} seen", left.Type.FullName, right.Type.FullName));
			}
			left.SetValue(right);
			return right;
		}
		
		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			foreach(INode statement in blockStatement.Children) {
				Evaluate(statement);
			}
			return null;
		}
		
		public override object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			return null;
		}
		
		public override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			Evaluate(expressionStatement.Expression);
			return null;
		}
		
		public override object VisitCastExpression(CastExpression castExpression, object data)
		{
			return Evaluate(castExpression.Expression);
		}
		
		public DebugType GetDebugType(INode expr)
		{
			if (expr is ParenthesizedExpression) {
				return GetDebugType(((ParenthesizedExpression)expr).Expression);
			} else if (expr is CastExpression) {
				return DebugType.CreateFromTypeReference(context.AppDomain, ((CastExpression)expr).CastTo);
			} else {
				return null;
			}
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string identifier = identifierExpression.Identifier;
			
			if (identifier == "__exception") {
				if (context.Thread.CurrentException != null) {
					return context.Thread.CurrentException.Value;
				} else {
					throw new GetValueException("No current exception");
				}
			}
			
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
				Value indexValue = Evaluate(indexExpr);
				indexes.Add(indexValue);
			}
			
			Value target = Evaluate(indexerExpression.TargetObject);
			
			if (target.Type.IsArray) {
				List<int> intIndexes = new List<int>();
				foreach(Value index in indexes) {
					if (!index.Type.IsInteger) throw new GetValueException("Integer expected for indexer");
					intIndexes.Add((int)index.PrimitiveValue);
				}
				return target.GetArrayElement(intIndexes.ToArray());
			}
			
			if (target.Type.IsPrimitive && target.PrimitiveValue is string) {
				if (indexes.Count == 1 && indexes[0].Type.IsInteger) {
					int index = (int)indexes[0].PrimitiveValue;
					return Eval.CreateValue(context.AppDomain, ((string)target.PrimitiveValue)[index]);
				} else {
					throw new GetValueException("Expected single integer index");
				}
			}
			
			PropertyInfo pi = target.Type.GetProperty("Item");
			if (pi == null) throw new GetValueException("The object does not have an indexer property");
			return target.GetPropertyValue(pi, indexes.ToArray());
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Value target;
			DebugType targetType;
			string methodName;
			MemberReferenceExpression memberRef = invocationExpression.TargetObject as MemberReferenceExpression;
			if (memberRef != null) {
				target = Evaluate(memberRef.TargetObject);
				targetType = GetDebugType(memberRef.TargetObject) ?? target.Type;
				methodName = memberRef.MemberName;
			} else {
				IdentifierExpression ident = invocationExpression.TargetObject as IdentifierExpression;
				if (ident != null) {
					target = Evaluate(new ThisReferenceExpression());
					targetType = target.Type;
					methodName = ident.Identifier;
				} else {
					throw new GetValueException("Member reference expected for method invocation");
				}
			}
			MethodInfo method;
			IList<MemberInfo> methods = targetType.GetMembers(methodName, BindingFlags.Method);
			if (methods.Count == 0)
				methods = targetType.GetMembers(methodName, BindingFlags.Method | BindingFlags.IncludeSuperType);
			if (methods.Count == 0) {
				throw new GetValueException("Method " + methodName + " not found");
			} else if (methods.Count == 1) {
				method = (MethodInfo)methods[0];
			} else {
				List<DebugType> argTypes = new List<DebugType>();
				foreach(Expression expr in invocationExpression.Arguments) {
					DebugType argType = GetDebugType(expr);
					if (argType == null)
						throw new GetValueException("Multiple methods with name " + methodName + " found.  Use explicit casts for arguments to select method overload.");
					argTypes.Add(argType);
				}
				method = target.Type.GetMethod(methodName, argTypes.ToArray());
				if (method == null)
					throw new GetValueException("Can not find overload with given types");
			}
			List<Value> args = new List<Value>();
			foreach(Expression expr in invocationExpression.Arguments) {
				args.Add(Evaluate(expr));
			}
			return target.InvokeMethod(method, args.ToArray());
		}
		
		public override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			List<Expression> constructorParameters = objectCreateExpression.Parameters;
			DebugType[] constructorParameterTypes = new DebugType[constructorParameters.Count];
			for (int i = 0; i < constructorParameters.Count; i++) {
				constructorParameterTypes[i] = GetDebugType(constructorParameters[i]);
			}
			Value[] constructorParameterValues = new Value[constructorParameters.Count];
			for (int i = 0; i < constructorParameters.Count; i++) {
				constructorParameterValues[i] = Evaluate(constructorParameters[i]);
			}
			return Eval.NewObject(DebugType.CreateFromTypeReference(context.AppDomain, objectCreateExpression.CreateType), 
			                      constructorParameterValues, constructorParameterTypes);
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			Value target = Evaluate(memberReferenceExpression.TargetObject);
			DebugType targetType = GetDebugType(memberReferenceExpression.TargetObject) ?? target.Type;
			MemberInfo memberInfo = targetType.GetMember(memberReferenceExpression.MemberName, BindingFlags.AllInThisType);
			if (memberInfo == null)
				memberInfo = targetType.GetMember(memberReferenceExpression.MemberName, BindingFlags.All);
			if (memberInfo == null)
				throw new GetValueException("Member \"" + memberReferenceExpression.MemberName + "\" not found");
			Value member = target.GetMemberValue(memberInfo);
			return member;
		}
		
		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return Evaluate(parenthesizedExpression.Expression);
		}
		
		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			return Eval.CreateValue(context.AppDomain, primitiveExpression.Value);
		}
		
		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return context.GetThisValue();
		}
		
		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			Value value = Evaluate(unaryOperatorExpression.Expression);
			UnaryOperatorType op = unaryOperatorExpression.Op;
			
			if (op == UnaryOperatorType.Dereference) {
				if (!value.Type.IsPointer) throw new GetValueException("Target object is not a pointer");
				return value.Dereference(); // TODO: Test
			}
			
			if (!value.Type.IsPrimitive) throw new GetValueException("Primitive value expected");
			
			object val = value.PrimitiveValue;
			
			object result = null;
			
			// Bool operation
			if (val is bool) {
				bool a = Convert.ToBoolean(val);
				switch (op) {
					case UnaryOperatorType.Not: result = !a; break;
				}
			}
			
			// Float operation
			if (val is double || val is float) {
				double a = Convert.ToDouble(val);
				switch (op) {
					case UnaryOperatorType.Minus: result = -a; break;
					case UnaryOperatorType.Plus:  result = +a; break;
				}
			}
		
			// Integer operation
			if (val is byte || val is sbyte || val is int || val is uint || val is long || val is ulong) {
				long a = Convert.ToInt64(val);
				switch (op) {
					case UnaryOperatorType.Decrement:     result = a - 1; break;
					case UnaryOperatorType.Increment:     result = a + 1; break;
					case UnaryOperatorType.PostDecrement: result = a; break;
					case UnaryOperatorType.PostIncrement: result = a; break;
					case UnaryOperatorType.Minus:         result = -a; break;
					case UnaryOperatorType.Plus:          result = a; break;
					case UnaryOperatorType.BitNot:        result = ~a; break;
				}
				switch (op) {
					case UnaryOperatorType.Decrement:
					case UnaryOperatorType.PostDecrement:
						VisitAssignmentExpression(new AssignmentExpression(unaryOperatorExpression.Expression, AssignmentOperatorType.Subtract, new PrimitiveExpression(1)), null);
						break;
					case UnaryOperatorType.Increment:
					case UnaryOperatorType.PostIncrement:
						VisitAssignmentExpression(new AssignmentExpression(unaryOperatorExpression.Expression, AssignmentOperatorType.Add, new PrimitiveExpression(1)), null);
						break;
				}
			}
			
			if (result == null) throw new GetValueException("Unsuppored unary expression " + op);
			
			return Eval.CreateValue(context.AppDomain, result);
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			Value left = Evaluate(binaryOperatorExpression.Left);
			Value right = Evaluate(binaryOperatorExpression.Right);
			
			object result = VisitBinaryOperatorExpressionInternal(left, right, binaryOperatorExpression.Op);
			// Conver long to int if possible
			if (result is long && int.MinValue <= (long)result && (long)result <= int.MaxValue) result = (int)(long)result;
			return Eval.CreateValue(context.AppDomain, result);
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
							default: throw new GetValueException("Unsupported operator for strings: " + op);
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
							
							default: throw new GetValueException("Unsupported operator for bools: " + op);
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
							
							default: throw new GetValueException("Unsupported operator for floats: " + op);
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
							
							default: throw new GetValueException("Unsupported operator for integers: " + op);
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
							
							default: throw new GetValueException("Unsupported operator for chars: " + op);
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
