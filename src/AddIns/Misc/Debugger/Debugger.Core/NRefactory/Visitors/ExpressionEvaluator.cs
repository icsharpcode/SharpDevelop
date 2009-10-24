// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Debugger;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Visitors
{
	public class EvaluateException: GetValueException
	{
		public EvaluateException(INode code, string msg):base(code, msg) {}
		public EvaluateException(INode code, string msgFmt, params object[] msgArgs):base(code, string.Format(msgFmt, msgArgs)) {}
	}
	
	class TypedValue
	{
		Value value;
		DebugType type;
		
		public Value Value {
			get { return value; }
		}
		
		public DebugType Type {
			get { return type; }
		}
		
		public object PrimitiveValue {
			get { return value.PrimitiveValue; }
		}
		
		public TypedValue(Value value, DebugType type)
		{
			this.value = value;
			this.type = type;
		}
	}
	
	public class ExpressionEvaluator: NotImplementedAstVisitor
	{
		StackFrame context;
		
		public StackFrame Context {
			get { return context; }
		}
		
		ExpressionEvaluator(StackFrame context)
		{
			this.context = context;
		}
		
		public static INode Parse(string code, SupportedLanguage language)
		{
			SnippetParser parser = new SnippetParser(language);
			INode astRoot = parser.Parse(code);
			if (parser.Errors.Count > 0) {
				throw new GetValueException(parser.Errors.ErrorOutput);
			}
			if (parser.SnippetType != SnippetType.Expression && parser.SnippetType != SnippetType.Statements) {
				throw new GetValueException("Code must be expression or statement");
			}
			return astRoot;
		}
		
		/// <summary> Evaluate given expression.  If you have expression tree already, use overloads of this method.</summary>
		/// <returns> Returned value or null for statements </returns>
		public static Value Evaluate(string code, SupportedLanguage language, StackFrame context)
		{
			return Evaluate(Parse(code, language), context);
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
			
			TypedValue val = new ExpressionEvaluator(context).Evaluate(code, false);
			if (val == null)
				return null;
			return val.Value;
		}
		
		/// <summary>
		/// Parses string representation of an expression (eg. "a.b[10] + 2") into NRefactory Expression tree.
		/// </summary>
		public static Expression ParseExpression(string code, SupportedLanguage language)
		{
			SnippetParser parser = new SnippetParser(language);
			INode astRoot = parser.Parse(code);
			if (parser.Errors.Count > 0) {
				throw new GetValueException(parser.Errors.ErrorOutput);
			}
			Expression astExpression = astRoot as Expression;
			if (astExpression == null) {
				throw new GetValueException("Code must be expression");
			}
			return astExpression;
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
					DebugPropertyInfo itemProperty = (DebugPropertyInfo)val.Type.GetProperty("Item");
					Value item = val.GetPropertyValue(itemProperty, Eval.CreateValue(val.AppDomain, i));
					sb.Append(FormatValue(item));
				}
				sb.Append("}");
				return sb.ToString();
			} else if (val.Type.FullName == typeof(char).FullName) {
				return "'" + val.PrimitiveValue.ToString() + "'";
			} else if (val.Type.FullName == typeof(string).FullName) {
				return "\"" + val.PrimitiveValue.ToString() + "\"";
			} else if (val.Type.IsPrimitive) {
				return val.PrimitiveValue.ToString();
			} else {
				return val.InvokeToString();
			}
		}
		
		TypedValue Evaluate(INode expression)
		{
			return Evaluate(expression, true);
		}
		
		TypedValue Evaluate(INode expression, bool permRef)
		{
			// Try to get the value from cache
			// (the cache is cleared when the process is resumed)
			TypedValue val;
			if (context.Process.CachedExpressions.TryGetValue(expression, out val)) {
				if (val == null || !val.Value.IsInvalid)
					return val;
			}
			
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			try {
				val = (TypedValue)expression.AcceptVisitor(this, null);
				if (val != null && permRef)
					val = new TypedValue(val.Value.GetPermanentReference(), val.Type);
			} catch (GetValueException e) {
				e.Expression = expression;
				throw;
			} catch (NotImplementedException e) {
				throw new GetValueException(expression, "Language feature not implemented: " + e.Message);
			} finally {
				watch.Stop();
				context.Process.TraceMessage("Evaluated: {0} in {1} ms total", expression.PrettyPrint(), watch.ElapsedMilliseconds);
			}
			
			if (val != null && val.Value.IsInvalid)
				throw new DebuggerException("Expression \"" + expression.PrettyPrint() + "\" is invalid right after evaluation");
			
			// Add the result to cache
			context.Process.CachedExpressions[expression] = val;
			
			return val;
		}
		
		List<TypedValue> EvaluateAll(List<Expression> exprs)
		{
			List<TypedValue> vals = new List<TypedValue>(exprs.Count);
			foreach(Expression expr in exprs) {
				vals.Add(Evaluate(expr));
			}
			return vals;
		}
		
		int EvaluateAsInt(INode expression)
		{
			if (expression is PrimitiveExpression) {
				int? i = ((PrimitiveExpression)expression).Value as int?;
				if (i == null)
					throw new EvaluateException(expression, "Integer expected");
				return i.Value;
			} else {
				TypedValue typedVal = Evaluate(expression);
				if (typedVal.Type.CanImplicitelyConvertTo(typeof(int))) {
					int i = (int)Convert.ChangeType(typedVal.PrimitiveValue, typeof(int));
					return i;
				} else {
					throw new EvaluateException(expression, "Integer expected");
				}
			}
		}
		
		TypedValue EvaluateAs(INode expression, DebugType type)
		{
			TypedValue val = Evaluate(expression);
			if (val.Type == type)
				return val;
			if (!val.Type.CanImplicitelyConvertTo(type))
				throw new EvaluateException(expression, "Can not implicitely cast {0} to {1}", val.Type.FullName, type.FullName);
			if (type.IsPrimitive) {
				object oldVal = val.PrimitiveValue;
				object newVal;
				try {
					newVal = Convert.ChangeType(oldVal, type.PrimitiveType);
				} catch (InvalidCastException) {
					throw new EvaluateException(expression, "Can not cast {0} to {1}", val.GetType().FullName, type.FullName);
				} catch (OverflowException) {
					throw new EvaluateException(expression, "Overflow");
				}
				return CreateValue(newVal);
			} else {
				return new TypedValue(val.Value, type);
			}
		}
		
		Value[] GetValues(List<TypedValue> typedVals)
		{
			List<Value> vals = new List<Value>(typedVals.Count);
			foreach(TypedValue typedVal in typedVals) {
				vals.Add(typedVal.Value);
			}
			return vals.ToArray();
		}
		
		DebugType[] GetTypes(List<TypedValue> typedVals)
		{
			List<DebugType> types = new List<DebugType>(typedVals.Count);
			foreach(TypedValue typedVal in typedVals) {
				types.Add(typedVal.Type);
			}
			return types.ToArray();
		}
		
		DebugType GetDebugType(INode expr)
		{
			if (expr is ParenthesizedExpression) {
				return GetDebugType(((ParenthesizedExpression)expr).Expression);
			} else if (expr is CastExpression) {
				return ((CastExpression)expr).CastTo.ResolveType(context.AppDomain);
			} else {
				return null;
			}
		}
		
		TypedValue CreateValue(object primitiveValue)
		{
			Value val = Eval.CreateValue(context.AppDomain, primitiveValue);
			return new TypedValue(val, val.Type);
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
			
			TypedValue right;
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
			TypedValue left = (TypedValue)assignmentExpression.Left.AcceptVisitor(this, null);
			
			if (left == null) {
				// Can this happen?
				throw new GetValueException(string.Format("\"{0}\" can not be set", assignmentExpression.Left.PrettyPrint()));
			}
			if (!left.Value.IsReference && left.Type.FullName != right.Type.FullName) {
				throw new GetValueException(string.Format("Type {0} expected, {1} seen", left.Type.FullName, right.Type.FullName));
			}
			left.Value.SetValue(right.Value);
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
			TypedValue val = Evaluate(castExpression.Expression);
			DebugType castTo = castExpression.CastTo.ResolveType(context.AppDomain);
			if (castTo.IsPrimitive && val.Type.IsPrimitive && castTo != val.Type) {
				object oldVal = val.PrimitiveValue;
				object newVal;
				try {
					newVal = Convert.ChangeType(oldVal, castTo.PrimitiveType);
				} catch (InvalidCastException) {
					throw new EvaluateException(castExpression, "Can not cast {0} to {1}", val.Type.FullName, castTo.FullName);
				} catch (OverflowException) {
					throw new EvaluateException(castExpression, "Overflow");
				}
				val = CreateValue(newVal);
			}
			if (!castTo.IsAssignableFrom(val.Value.Type))
				throw new GetValueException("Can not cast {0} to {1}", val.Value.Type.FullName, castTo.FullName);
			return new TypedValue(val.Value, castTo);
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string identifier = identifierExpression.Identifier;
			
			if (identifier == "__exception") {
				if (context.Thread.CurrentException != null) {
					return new TypedValue(
						context.Thread.CurrentException.Value,
						DebugType.CreateFromType(context.AppDomain.Mscorlib, typeof(System.Exception))
					);
				} else {
					throw new GetValueException("No current exception");
				}
			}
			
			DebugParameterInfo par = context.MethodInfo.GetParameter(identifier);
			if (par != null)
				return new TypedValue(par.GetValue(context), (DebugType)par.ParameterType);
			
			DebugLocalVariableInfo loc = context.MethodInfo.GetLocalVariable(identifier);
			if (loc != null)
				return new TypedValue(loc.GetValue(context), (DebugType)loc.LocalType);
			
			// Instance class members
			// Note that the method might be generated instance method that represents anonymous method
			TypedValue thisValue = GetThisValue();
			if (thisValue != null) {
				IDebugMemberInfo instMember = (IDebugMemberInfo)thisValue.Type.GetMember<MemberInfo>(identifier, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, DebugType.IsFieldOrNonIndexedProperty);
				if (instMember != null)
					return new TypedValue(Value.GetMemberValue(thisValue.Value, (MemberInfo)instMember), instMember.MemberType);
			}
			
			// Static class members
			foreach(DebugType declaringType in ((DebugType)context.MethodInfo.DeclaringType).GetSelfAndDeclaringTypes()) {
				IDebugMemberInfo statMember = (IDebugMemberInfo)declaringType.GetMember<MemberInfo>(identifier, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, DebugType.IsFieldOrNonIndexedProperty);
				if (statMember != null)
					return new TypedValue(Value.GetMemberValue(null, (MemberInfo)statMember), statMember.MemberType);
			}
			
			throw new GetValueException("Identifier \"" + identifier + "\" not found in this context");
		}
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			TypedValue target = Evaluate(indexerExpression.TargetObject);
			
			if (target.Type.IsArray) {
				List<int> intIndexes = new List<int>();
				foreach(Expression indexExpr in indexerExpression.Indexes) {
					intIndexes.Add(EvaluateAsInt(indexExpr));
				}
				return new TypedValue(
					target.Value.GetArrayElement(intIndexes.ToArray()),
					(DebugType)target.Type.GetElementType()
				);
			} else if (target.Type.FullName == typeof(string).FullName) {
				if (indexerExpression.Indexes.Count != 1)
					throw new GetValueException("Single index expected");
				
				int index = EvaluateAsInt(indexerExpression.Indexes[0]);
				return CreateValue(((string)target.PrimitiveValue)[index]);
			} else {
				List<TypedValue> indexes = EvaluateAll(indexerExpression.Indexes);
				DebugPropertyInfo pi = (DebugPropertyInfo)target.Type.GetProperty("Item", GetTypes(indexes));
				if (pi == null)
					throw new GetValueException("The object does not have an indexer property");
				return new TypedValue(
					target.Value.GetPropertyValue(pi, GetValues(indexes)),
					(DebugType)pi.PropertyType
				);
			}
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			TypedValue target;
			DebugType targetType;
			string methodName;
			MemberReferenceExpression memberRef = invocationExpression.TargetObject as MemberReferenceExpression;
			if (memberRef != null) {
				// TODO: Optimize
				try {
					// Instance
					target = Evaluate(memberRef.TargetObject);
					targetType = target.Type;
				} catch (GetValueException) {
					// Static
					target = null;
					targetType = memberRef.TargetObject.ResolveType(context.AppDomain);
					if (targetType == null)
						throw;
				}
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
			List<TypedValue> args = EvaluateAll(invocationExpression.Arguments);
			MethodInfo method = targetType.GetMethod(methodName, DebugType.BindingFlagsAllInScope, null, GetTypes(args), null);
			if (method == null)
				throw new GetValueException("Method " + methodName + " not found");
			Value retVal = Value.InvokeMethod(target != null ? target.Value : null, method, GetValues(args));
			if (retVal == null)
				return null;
			return new TypedValue(retVal, (DebugType)method.ReturnType);
		}
		
		public override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			if (!objectCreateExpression.ObjectInitializer.IsNull)
				throw new EvaluateException(objectCreateExpression.ObjectInitializer, "Object initializers not supported");
			
			DebugType type = objectCreateExpression.CreateType.ResolveType(context.AppDomain);
			List<TypedValue> ctorArgs = EvaluateAll(objectCreateExpression.Parameters);
			ConstructorInfo ctor = type.GetConstructor(BindingFlags.Default, null, CallingConventions.Any, GetTypes(ctorArgs), null);
			if (ctor == null)
				throw new EvaluateException(objectCreateExpression, "Constructor not found");
			Value val = (Value)ctor.Invoke(GetValues(ctorArgs));
			return new TypedValue(val, type);
		}
		
		public override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			if (arrayCreateExpression.CreateType.RankSpecifier[0] != 0)
				throw new EvaluateException(arrayCreateExpression, "Multi-dimensional arrays are not suppored");
			
			DebugType type = arrayCreateExpression.CreateType.ResolveType(context.AppDomain);
			int length = 0;
			if (arrayCreateExpression.Arguments.Count == 1) {
				length = EvaluateAsInt(arrayCreateExpression.Arguments[0]);
			} else if (!arrayCreateExpression.ArrayInitializer.IsNull) {
				length = arrayCreateExpression.ArrayInitializer.CreateExpressions.Count;
			}
			Value array = Eval.NewArray((DebugType)type.GetElementType(), (uint)length, null);
			if (!arrayCreateExpression.ArrayInitializer.IsNull) {
				List<Expression> inits = arrayCreateExpression.ArrayInitializer.CreateExpressions;
				if (inits.Count != length)
					throw new EvaluateException(arrayCreateExpression, "Incorrect initializer length");
				for(int i = 0; i < length; i++) {
					TypedValue init = EvaluateAs(inits[i], (DebugType)type.GetElementType());
					array.SetArrayElement(new int[] { i }, init.Value);
				}
			}
			return new TypedValue(array, type);
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			TypedValue target;
			DebugType targetType;
			try {
				// Instance
				target = Evaluate(memberReferenceExpression.TargetObject);
				targetType = target.Type;
			} catch (GetValueException) {
				// Static
				target = null;
				targetType = memberReferenceExpression.TargetObject.ResolveType(context.AppDomain);
				if (targetType == null)
					throw;
			}
			MemberInfo[] memberInfos = targetType.GetMember(memberReferenceExpression.MemberName, DebugType.BindingFlagsAllInScope);
			if (memberInfos.Length == 0)
				throw new GetValueException("Member \"" + memberReferenceExpression.MemberName + "\" not found");
			return new TypedValue(
				Value.GetMemberValue(target != null ? target.Value : null, memberInfos[0]),
				((IDebugMemberInfo)memberInfos[0]).MemberType
			);
		}
		
		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return Evaluate(parenthesizedExpression.Expression);
		}
		
		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			return CreateValue(primitiveExpression.Value);
		}
		
		TypedValue GetThisValue()
		{
			// This is needed so that captured 'this' is supported
			foreach(DebugLocalVariableInfo locVar in context.MethodInfo.GetLocalVariables()) {
				if (locVar.IsThis)
					return new TypedValue(locVar.GetValue(context), (DebugType)locVar.LocalType);
			}
			return null;
		}
		
		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			TypedValue thisValue = GetThisValue();
			if (thisValue == null)
				throw new GetValueException(context.MethodInfo.FullName + " is static method and does not have \"this\"");
			return thisValue;
		}
		
		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			TypedValue value = Evaluate(unaryOperatorExpression.Expression);
			UnaryOperatorType op = unaryOperatorExpression.Op;
			
			if (op == UnaryOperatorType.Dereference) {
				if (!value.Type.IsPointer)
					throw new GetValueException("Target object is not a pointer");
				return new TypedValue(value.Value.Dereference(), (DebugType)value.Type.GetElementType());
			}
			
			if (!value.Type.IsPrimitive)
				throw new GetValueException("Primitive value expected");
			
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
			
			if (result == null)
				throw new GetValueException("Unsuppored unary expression " + op);
			
			return CreateValue(result);
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			TypedValue left = Evaluate(binaryOperatorExpression.Left);
			TypedValue right = Evaluate(binaryOperatorExpression.Right);
			
			object result = VisitBinaryOperatorExpressionInternal(left, right, binaryOperatorExpression.Op);
			// Conver long to int if possible
			if (result is long && int.MinValue <= (long)result && (long)result <= int.MaxValue)
				result = (int)(long)result;
			return CreateValue(result);
		}
		
		object VisitBinaryOperatorExpressionInternal(TypedValue leftValue, TypedValue rightValue, BinaryOperatorType op)
		{
			object left = leftValue.Type.IsPrimitive ? leftValue.PrimitiveValue : null;
			object right = rightValue.Type.IsPrimitive ? rightValue.PrimitiveValue : null;
			
			// Both are classes - do reference comparison
			if (left == null && right == null) {
				if (leftValue.Value.IsNull || rightValue.Value.IsNull) {
					return leftValue.Value.IsNull && rightValue.Value.IsNull;
				} else {
					// TODO: Make sure this works for byrefs and arrays
					return leftValue.Value.Address == rightValue.Value.Address;
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
