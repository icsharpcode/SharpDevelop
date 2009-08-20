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
			
			Value result;
			try {
				result = (Value)code.AcceptVisitor(new ExpressionEvaluator(context), null);
			} catch (GetValueException) {
				throw;
			} catch (NotImplementedException e) {
				throw new GetValueException(code, "Language feature not implemented: " + e.Message);
			}
			return result;
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
		
		void AddToCache(INode expression, Value value)
		{
			// Expressions are cleared then the process is resumed
			context.Process.CachedExpressions[expression] = value;
		}
		
		bool TryGetCached(INode expression, out Value cached)
		{
			Value val;
			if (context.Process.CachedExpressions.TryGetValue(expression, out val)) {
				if (val == null || !val.IsInvalid) {
					// context.Process.TraceMessage("Is cached: {0}", expression.PrettyPrint());
					cached = val;
					return true;
				}
			}
			cached = null;
			return false;
		}
		
		Value EvalAndPermRef(INode expression)
		{
			Value val = (Value)expression.AcceptVisitor(this, null);
			if (val != null)
				val = val.GetPermanentReference();
			AddToCache(expression, val);
			return val;
		}
		
		StackFrame context;
		
		public StackFrame Context {
			get { return context; }
		}
		
		public ExpressionEvaluator(StackFrame context)
		{
			this.context = context;
		}
		
		IDisposable LogEval(INode expression)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			return new CallbackOnDispose(delegate {
				if (!context.Process.CachedExpressions.ContainsKey(expression))
					throw new DebuggerException("Result not added to cache");
				watch.Stop();
				context.Process.TraceMessage("Evaluated: {0} in {1} ms total", expression.PrettyPrint(), watch.ElapsedMilliseconds);
			});
		}
		
		public override object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
		{
			using(LogEval(emptyStatement)) {
				return null;
			}
		}
		
		public override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			using(LogEval(expressionStatement)) {
				EvalAndPermRef(expressionStatement.Expression);
				return null;
			}
		}
		
		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			Value cached;
			if (TryGetCached(parenthesizedExpression, out cached)) return cached;
			using(LogEval(parenthesizedExpression)) {
				Value res = EvalAndPermRef(parenthesizedExpression.Expression);
				
				AddToCache(parenthesizedExpression, res);
				return res;
			}
		}
		
		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			using(LogEval(blockStatement)) {
				foreach(INode statement in blockStatement.Children) {
					EvalAndPermRef(statement);
				}
				return null;
			}
		}
		
		/// <remarks> We have to put that in cache as well otherwise expaning (a = b).Prop will reevalute </remarks>
		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			Value cached;
			if (TryGetCached(assignmentExpression, out cached)) return cached;
			using(LogEval(assignmentExpression)) {
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
					right = EvalAndPermRef(assignmentExpression.Right);
				} else {
					BinaryOperatorExpression binOpExpr = new BinaryOperatorExpression();
					binOpExpr.Left  = assignmentExpression.Left;
					binOpExpr.Op    = op;
					binOpExpr.Right = assignmentExpression.Right;
					right = (Value)VisitBinaryOperatorExpression(binOpExpr, null);
				}
				right = right.GetPermanentReference();
				
				Value left = EvalAndPermRef(assignmentExpression.Left);
				
				if (!left.IsReference && left.Type.FullName != right.Type.FullName) {
					throw new GetValueException(string.Format("Type {0} expected, {1} seen", left.Type.FullName, right.Type.FullName));
				}
				left.SetValue(right);
				
				AddToCache(assignmentExpression, right);
				return right;
			}
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			Value cached;
			if (TryGetCached(identifierExpression, out cached)) return cached;
			using(LogEval(identifierExpression)) {
				string identifier = identifierExpression.Identifier;
				
				Value result = null;
				
				if (identifier == "__exception") {
					if (context.Thread.CurrentException != null) {
						result = context.Thread.CurrentException.Value;
					} else {
						throw new GetValueException("No current exception");
					}
				}
				
				result = result ?? context.GetArgumentValue(identifier);
				
				result = result ?? context.GetLocalVariableValue(identifier);
				
				if (result == null) {
					if (!context.MethodInfo.IsStatic) {
						// Can be null
						result = context.GetThisValue().GetMemberValue(identifier);
					} else {
						MemberInfo memberInfo = context.MethodInfo.DeclaringType.GetMember(identifier);
						if (memberInfo != null && memberInfo.IsStatic) {
							result = Value.GetMemberValue(null, memberInfo, null);
						}
					}
				}
				
				if (result == null)
					throw new GetValueException("Identifier \"" + identifier + "\" not found in this context");
				
				AddToCache(identifierExpression, result);
				return result;
			}
		}
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			Value cached;
			if (TryGetCached(indexerExpression, out cached)) return cached;
			using(LogEval(indexerExpression)) {
				List<Value> indexes = new List<Value>();
				foreach(Expression indexExpr in indexerExpression.Indexes) {
					Value indexValue = EvalAndPermRef(indexExpr);
					indexes.Add(indexValue);
				}
				
				Value target = EvalAndPermRef(indexerExpression.TargetObject);
				
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
				Value result = target.GetPropertyValue(pi, indexes.ToArray());
				
				AddToCache(indexerExpression, result);
				return result;
			}
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			Value cached;
			if (TryGetCached(invocationExpression, out cached)) return cached;
			using(LogEval(invocationExpression)) {
				Value target;
				string methodName;
				MemberReferenceExpression memberRef = invocationExpression.TargetObject as MemberReferenceExpression;
				if (memberRef != null) {
					target = EvalAndPermRef(memberRef.TargetObject);
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
					args.Add(EvalAndPermRef(expr));
				}
				MethodInfo method = target.Type.GetMember(methodName, BindingFlags.Method | BindingFlags.IncludeSuperType) as MethodInfo;
				if (method == null) {
					throw new GetValueException("Method " + methodName + " not found");
				}
				Value result = target.InvokeMethod(method, args.ToArray());
				
				AddToCache(invocationExpression, result);
				return result;
			}
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			Value cached;
			if (TryGetCached(memberReferenceExpression, out cached)) return cached;
			using(LogEval(memberReferenceExpression)) {
				Value target = EvalAndPermRef(memberReferenceExpression.TargetObject);
				Value member = target.GetMemberValue(memberReferenceExpression.MemberName);
				if (member == null)
					throw new GetValueException("Member \"" + memberReferenceExpression.MemberName + "\" not found");
				
				AddToCache(memberReferenceExpression, member);
				return member;
			}
		}
		
		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			Value cached;
			if (TryGetCached(primitiveExpression, out cached)) return cached;
			using(LogEval(primitiveExpression)){
				Value result = Eval.CreateValue(context.AppDomain, primitiveExpression.Value);
				
				AddToCache(primitiveExpression, result);
				return result;
			}
		}
		
		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			Value cached;
			if (TryGetCached(thisReferenceExpression, out cached)) return cached;
			using(LogEval(thisReferenceExpression)) {
				Value result = context.GetThisValue();
				
				AddToCache(thisReferenceExpression, result);
				return result;
			}
		}
		
		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			Value cached;
			if (TryGetCached(unaryOperatorExpression, out cached)) return cached;
			using(LogEval(unaryOperatorExpression)) {
				Value value = EvalAndPermRef(unaryOperatorExpression.Expression);
				UnaryOperatorType op = unaryOperatorExpression.Op;
				
				if (op == UnaryOperatorType.Dereference) {
					if (!value.Type.IsPointer) throw new GetValueException("Target object is not a pointer");
					return value.Dereference();
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
				
				Value res = Eval.CreateValue(context.AppDomain, result);
				
				AddToCache(unaryOperatorExpression, res);
				return res;
			}
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			Value cached;
			if (TryGetCached(binaryOperatorExpression, out cached)) return cached;
			using(LogEval(binaryOperatorExpression)) {
				Value left = EvalAndPermRef(binaryOperatorExpression.Left);
				Value right = EvalAndPermRef(binaryOperatorExpression.Right);
				
				object result = VisitBinaryOperatorExpressionInternal(left, right, binaryOperatorExpression.Op);
				// Conver long to int if possible
				if (result is long && int.MinValue <= (long)result && (long)result <= int.MaxValue) result = (int)(long)result;
				Value res = Eval.CreateValue(context.AppDomain, result);
				
				AddToCache(binaryOperatorExpression, res);
				return res;
			}
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
