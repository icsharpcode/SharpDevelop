// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	/// <summary>
	/// Prettifies the Boo code by removing type declarations that aren't required in Boo.
	/// </summary>
	public class RemoveRedundantTypeReferencesVisitor : DepthFirstTransformer
	{
		protected override void OnError(Node node, Exception error)
		{
			if (error is CompilerError)
				base.OnError(node, error);
			else
				throw new CompilerError(node, error.ToString());
		}
		
		bool IsVoid(TypeReference typeRef)
		{
			SimpleTypeReference str = typeRef as SimpleTypeReference;
			return str != null && (str.Name == "void" || str.Name == "System.Void");
		}
		
		public override void OnMethod(Method node)
		{
			base.OnMethod(node);
			if (IsVoid(node.ReturnType))
				node.ReturnType = null;
		}
		
		Stack<List<Field>> fieldStack = new Stack<List<Field>>();
		
		void EnterTypeDefinition(TypeDefinition node)
		{
			List<Field> list = new List<Field>();
			fieldStack.Push(list);
			foreach (TypeMember member in node.Members) {
				if (member is Field)
					list.Add((Field)member);
			}
		}
		
		bool SearchField(string name)
		{
			foreach (List<Field> list in fieldStack) {
				foreach (Field field in list) {
					if (field.Name == name)
						return true;
				}
			}
			return false;
		}
		
		public override void OnClassDefinition(ClassDefinition node)
		{
			EnterTypeDefinition(node);
			base.OnClassDefinition(node);
			fieldStack.Pop();
		}
		
		public override void OnStructDefinition(StructDefinition node)
		{
			EnterTypeDefinition(node);
			base.OnStructDefinition(node);
			fieldStack.Pop();
		}
		
		public override void OnModule(Module node)
		{
			EnterTypeDefinition(node);
			base.OnModule(node);
			fieldStack.Pop();
		}
		
		public override void OnField(Field node)
		{
			if (node.Type != null) {
				TypeReference initializerType = GetInferredType(node.Initializer);
				if (node.Type.Matches(initializerType)) {
					node.Type = null;
				}
			}
			base.OnField(node);
		}
		
		public override void OnDeclarationStatement(DeclarationStatement node)
		{
			if (node.Declaration.Type != null && !SearchField(node.Declaration.Name)) {
				TypeReference initializerType = GetInferredType(node.Initializer);
				if (node.Declaration.Type.Matches(initializerType)) {
					node.Declaration.Type = null;
				}
			}
			base.OnDeclarationStatement(node);
		}
		
		TypeReference GetInferredType(Expression expr)
		{
			if (expr == null)
				return null;
			switch (expr.NodeType) {
				case NodeType.TypeofExpression:
					return new SimpleTypeReference("type");
				case NodeType.BoolLiteralExpression:
					return new SimpleTypeReference("bool");
				case NodeType.IntegerLiteralExpression:
					if (((IntegerLiteralExpression)expr).IsLong)
						break;
					return new SimpleTypeReference("int");
				case NodeType.StringLiteralExpression:
					return new SimpleTypeReference("string");
				case NodeType.CharLiteralExpression:
					return new SimpleTypeReference("char");
				case NodeType.DoubleLiteralExpression:
					if (((DoubleLiteralExpression)expr).IsSingle)
						break;
					return new SimpleTypeReference("double");
				case NodeType.CastExpression:
					return ((CastExpression)expr).Type;
				case NodeType.TryCastExpression:
					return ((TryCastExpression)expr).Type;
				case NodeType.MethodInvocationExpression:
					MethodInvocationExpression mie = (MethodInvocationExpression)expr;
					if (mie.Target.NodeType == NodeType.ReferenceExpression)
						return new SimpleTypeReference(((ReferenceExpression)mie.Target).Name);
					break;
			}
			return null;
		}
	}
}
