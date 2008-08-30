// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.MetaData;
using System;
using System.Collections.Generic;
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
				MetaData.MemberInfo memberInfo = context.MethodInfo.DeclaringType.GetMember(identifier);
				if (memberInfo != null && memberInfo.IsStatic) {
					return Value.GetMemberValue(null, memberInfo, null);
				}
			}
			
			throw new GetValueException("Identifier \"" + identifier + "\" not found in this context");
		}
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			List<int> indexes = new List<int>();
			foreach(Expression indexExpr in indexerExpression.Indexes) {
				Value indexValue = (Value)indexExpr.AcceptVisitor(this, null);
				if (!indexValue.Type.IsInteger) throw new GetValueException("Integer expected");
				indexes.Add((int)indexValue.PrimitiveValue);
			}
			Value target = (Value)indexerExpression.TargetObject.AcceptVisitor(this, null);
			if (!target.Type.IsArray) throw new GetValueException("Target is not array");
			return target.GetArrayElement(indexes.ToArray());
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			MemberReferenceExpression memberRef = invocationExpression.TargetObject as MemberReferenceExpression;
			if (memberRef == null) {
				throw new GetValueException("Member reference expected duting method invocation");
			}
			Value target = ((Value)memberRef.TargetObject.AcceptVisitor(this, null)).GetPermanentReference();
			List<Value> args = new List<Value>();
			foreach(Expression expr in invocationExpression.Arguments) {
				args.Add(((Value)expr.AcceptVisitor(this, null)).GetPermanentReference());
			}
			MethodInfo method = target.Type.GetMember(memberRef.MemberName, BindingFlags.Method | BindingFlags.IncludeSuperType) as MethodInfo;
			if (method == null) {
				throw new GetValueException("Method " + memberRef.MemberName + " not found");
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
			if (primitiveExpression.Value == null) {
				return Eval.CreateValue(context.Process, null);
			} else if (primitiveExpression.Value is string) {
				return Eval.NewString(context.Process, primitiveExpression.Value as string);
			} else {
				Value val = Eval.NewObjectNoConstructor(DebugType.Create(context.Process, null, primitiveExpression.Value.GetType().FullName));
				val.PrimitiveValue = primitiveExpression.Value;
				return val;
			}
		}
		
		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return context.GetThisValue();
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			Value left = ((Value)binaryOperatorExpression.Left.AcceptVisitor(this, null)).GetPermanentReference();
			Value right = ((Value)binaryOperatorExpression.Right.AcceptVisitor(this, null)).GetPermanentReference();
			
			if (!left.IsReference && left.Type.FullName != right.Type.FullName) {
				throw new GetValueException(string.Format("Type {0} expected, {1} seen", left.Type.FullName, right.Type.FullName));
			}
			
			Value val = Eval.NewObjectNoConstructor(DebugType.Create(context.Process, null, typeof(bool).FullName));
			
			switch (binaryOperatorExpression.Op)
			{
				case BinaryOperatorType.Equality :
					val.PrimitiveValue = (right.PrimitiveValue.ToString() == left.PrimitiveValue.ToString());
					break;
				case BinaryOperatorType.InEquality :
					val.PrimitiveValue = (right.PrimitiveValue.ToString() != left.PrimitiveValue.ToString());
					break;
				default :
					throw new NotImplementedException("BinaryOperator not implemented!");
			}
			
			return val;
		}
	}
}
