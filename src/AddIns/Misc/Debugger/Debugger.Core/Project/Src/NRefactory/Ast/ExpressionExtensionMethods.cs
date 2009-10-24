// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.NRefactory.Visitors;
using System;
using System.Collections.Generic;
using System.Reflection;
using Debugger;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Ast
{
	public static class ExpressionExtensionMethods
	{
		public static Value Evaluate(this Expression expression, Process process)
		{
			return ExpressionEvaluator.Evaluate(expression, process);
		}
		
		public static UnaryOperatorExpression AppendDereference(this Expression expression)
		{
			return new UnaryOperatorExpression(new ParenthesizedExpression(expression), UnaryOperatorType.Dereference);
		}
		
		public static IndexerExpression AppendIndexer(this Expression expression, params int[] indices)
		{
			IndexerExpression indexerExpr = new IndexerExpression(Parenthesize(expression), new List<Expression>());
			foreach(int index in indices) {
				indexerExpr.Indexes.Add(
					new CastExpression(
						new TypeReference(typeof(int).FullName),
						new PrimitiveExpression(index),
						CastType.Cast
					)
				);
			}
			return indexerExpr;
		}
		
		public static Expression AppendMemberReference(this Expression expresion, IDebugMemberInfo memberInfo, params Expression[] args)
		{
			Expression target;
			if (memberInfo.IsStatic) {
				target = new TypeReferenceExpression(
					memberInfo.DeclaringType.ToTypeReference()
				);
			} else {
				target = new ParenthesizedExpression(
					new CastExpression(
						memberInfo.DeclaringType.ToTypeReference(),
						Parenthesize(expresion),
						CastType.Cast
					)
				);
			}
			
			if (memberInfo is DebugFieldInfo) {
				if (args.Length > 0)
					throw new DebuggerException("No arguments expected for a field");
				return new MemberReferenceExpression(target, memberInfo.Name);
			}
			
			if (memberInfo is MethodInfo) {
				return new InvocationExpression(
					new MemberReferenceExpression(target, memberInfo.Name),
					AddExplicitTypes((MethodInfo)memberInfo, args)
				);
			}
			
			if (memberInfo is PropertyInfo) {
				PropertyInfo propInfo = (PropertyInfo)memberInfo;
				if (args.Length > 0) {
					if (memberInfo.Name != "Item")
						throw new DebuggerException("Arguments expected only for the Item property");
					return new IndexerExpression(
						target,
						AddExplicitTypes(propInfo.GetGetMethod() ?? propInfo.GetSetMethod(), args)
					);
				} else {
					return new MemberReferenceExpression(target, memberInfo.Name);
				}
			}
			throw new DebuggerException("Unknown member type " + memberInfo.GetType().FullName);
		}
		
		static Expression Parenthesize(Expression expr)
		{
			if (expr is IdentifierExpression ||
			    expr is MemberReferenceExpression ||
			    expr is IndexerExpression ||
			    expr is ParenthesizedExpression ||
			    expr is PrimitiveExpression)
				return expr;
			return new ParenthesizedExpression(expr);
		}
		
		static List<Expression> AddExplicitTypes(MethodInfo method, Expression[] args)
		{
			if (args.Length != method.GetParameters().Length)
				throw new DebuggerException("Incorrect number of arguments");
			List<Expression> typedArgs = new List<Expression>(args.Length);
			for(int i = 0; i < args.Length; i++) {
				typedArgs.Add(
					new CastExpression(
						method.GetParameters()[i].ParameterType.ToTypeReference(),
						Parenthesize(args[i]),
						CastType.Cast
					)
				);
			}
			return typedArgs;
		}
		
		public static string GetDottedName(this Type type)
		{
			string fullName = null;
			for(Type currentType = type; currentType != null; currentType = currentType.DeclaringType) {
				string name = currentType.Name;
				if (name.IndexOf('`') != -1)
					name = name.Substring(0, name.IndexOf('`'));
				if (currentType.Namespace != string.Empty)
					name = currentType.Namespace + "." + name;
				if (fullName == null) {
					fullName = name;
				} else {
					fullName = name + "." + fullName;
				}
			}
			return fullName;
		}
		
		public static TypeReference ToTypeReference(this Type type)
		{
			List<int> arrayRanks = new List<int>();
			int pointerNest = 0;
			while(true) {
				// TODO: Combined arrays and pointers?
				// TODO: Check
				if (type.IsArray) {
					arrayRanks.Insert(0, type.GetArrayRank() - 1);
					type = type.GetElementType();
				} else if (type.IsPointer) {
					pointerNest++;
					type = type.GetElementType();
				} else {
					break;
				}
			}
			string name = type.GetDottedName();
			List<TypeReference> genArgs = new List<TypeReference>();
			foreach(DebugType genArg in type.GetGenericArguments()) {
				genArgs.Add(genArg.ToTypeReference());
			}
			TypeReference typeRef = new TypeReference(name, pointerNest, arrayRanks.ToArray(), genArgs);
			return typeRef;
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
