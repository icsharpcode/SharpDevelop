// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.NRefactory.PrettyPrinter;
using System;
using System.Collections.Generic;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;

namespace Debugger
{
	public static class ExpressionExtensionMethods
	{
		public static Value Evaluate(this Expression expression, Process process)
		{
			return ExpressionEvaluator.Evaluate(expression, process);
		}
		
		public static UnaryOperatorExpression AppendDereference(this Expression expression)
		{
			return new UnaryOperatorExpression(expression, UnaryOperatorType.Dereference);
		}
		
		public static IndexerExpression AppendIndexer(this Expression expression, params int[] indices)
		{
			IndexerExpression indexerExpr = new IndexerExpression(expression, new List<Expression>());
			foreach(int index in indices) {
				indexerExpr.Indexes.Add(new PrimitiveExpression(index));
			}
			return indexerExpr;
		}
		
		public static Expression AppendMemberReference(this Expression expresion, MemberInfo memberInfo, params Expression[] args)
		{
			// TODO: Member hidding safety
			// TODO: Method overload safety
			Expression target = memberInfo.IsStatic ? new IdentifierExpression(memberInfo.DeclaringType.FullName) : expresion;
			if (memberInfo is FieldInfo) {
				if (args.Length > 0) throw new DebuggerException("No argumetns expected for a field");
				return new MemberReferenceExpression(target, memberInfo.Name);
			}
			if (memberInfo is MethodInfo) {
				return new InvocationExpression(new MemberReferenceExpression(target, memberInfo.Name), new List<Expression>(args));
			}
			if (memberInfo is PropertyInfo) {
				if (args.Length > 0) {
					if (memberInfo.Name != "Item") throw new DebuggerException("Arguments expected only for the Item property");
					return new IndexerExpression(target, new List<Expression>(args));
				} else {
					return new MemberReferenceExpression(target, memberInfo.Name);
				}
			}
			throw new DebuggerException("Unknown member type " + memberInfo.GetType().FullName);
		}
		
		public static string PrettyPrint(this INode code)
		{
			if (code == null) return string.Empty;
			CSharpOutputVisitor csOutVisitor = new CSharpOutputVisitor();
			code.AcceptVisitor(csOutVisitor, null);
			return csOutVisitor.Text;
		}
	}
}
