// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Ast;
using Ast = ICSharpCode.NRefactory.Ast;
using Debugger.Wrappers.CorSym;

namespace Debugger
{
	class EvaluateAstVisitor: NotImplementedAstVisitor
	{
		StackFrame stackFrame;
		
		public StackFrame StackFrame {
			get { return stackFrame; }
		}
		
		public EvaluateAstVisitor(StackFrame stackFrame)
		{
			this.stackFrame = stackFrame;
		}
		
		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return this.StackFrame.ThisValue;
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (identifierExpression is LocalVariableIdentifierExpression) {
				LocalVariableIdentifierExpression localVariableIdentifierExpression = (LocalVariableIdentifierExpression)identifierExpression;
				return this.StackFrame.GetLocalVariableValue(localVariableIdentifierExpression.SymVar);
			} else if (identifierExpression is ParameterIdentifierExpression) {
				ParameterIdentifierExpression parameterIdentifierExpression = (ParameterIdentifierExpression)identifierExpression;
				return this.StackFrame.GetArgument(parameterIdentifierExpression.ParameterIndex);
			} else {
				return this.StackFrame.GetValue(identifierExpression.Identifier);
			}
		}
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			Value target = (Value)indexerExpression.TargetObject.AcceptVisitor(this, data);
			List<int> indexes = new List<int>();
			foreach(Ast.Expression indexExpr in indexerExpression.Indexes) {
				if (indexExpr is PrimitiveExpression) {
					PrimitiveExpression primitiveExpression = (PrimitiveExpression)indexExpr;
					indexes.Add(int.Parse(primitiveExpression.StringValue));
				} else {
					Value indexValue = (Value)indexExpr.AcceptVisitor(this, data);
					if (!indexValue.IsInteger) {
						throw new DebuggerException("Integer expected");
					}
					indexes.Add((int)indexValue.PrimitiveValue);
				}
			}
			return target.GetArrayElement(indexes.ToArray());
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			Value target = (Value)memberReferenceExpression.TargetObject.AcceptVisitor(this, data);
			if (memberReferenceExpression is FieldReferenceExpression) {
				FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)memberReferenceExpression;
				return target.GetFieldValue(fieldReferenceExpression.FieldInfo);
			} else if (memberReferenceExpression is PropertyReferenceExpression) {
				PropertyReferenceExpression propertyReferenceExpression = (PropertyReferenceExpression)memberReferenceExpression;
				return target.GetPropertyValue(propertyReferenceExpression.PropertyInfo);
			} else {
				return target.GetMember(memberReferenceExpression.FieldName);
			}
		}
	}
}
