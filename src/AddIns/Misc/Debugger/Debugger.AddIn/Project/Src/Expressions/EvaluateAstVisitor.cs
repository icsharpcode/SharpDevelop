// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using ICSharpCode.NRefactory.Ast;
using System.Collections.Generic;

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
		
		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			return Eval.CreateValue(context.Process, primitiveExpression.Value);
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string identifier = identifierExpression.Identifier;
			
			if (identifier == "this") {
				return context.GetThisValue();
			}
			
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
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			List<int> indexes = new List<int>();
			foreach(Expression indexExpr in indexerExpression.Indexes) {
				Value indexValue = (Value)indexExpr.AcceptVisitor(this, null);
				if (!indexValue.IsInteger) throw new GetValueException("Integer expected");
				indexes.Add((int)indexValue.PrimitiveValue);
			}
			Value target = (Value)indexerExpression.TargetObject.AcceptVisitor(this, null);
			if (!target.IsArray) throw new GetValueException("Target is not array");
			return target.GetArrayElement(indexes.ToArray());
		}
	}
}
