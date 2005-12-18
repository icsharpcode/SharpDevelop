// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// Prefixes the names of the specified fields with the prefix and replaces the use.
	/// </summary>
	public class PrefixFieldsVisitor : AbstractASTVisitor
	{
		List<VariableDeclaration> fields;
		List<string> curBlock = new List<string>();
		Stack<List<string>> blocks = new Stack<List<string>>();
		string prefix;
		
		public PrefixFieldsVisitor(List<VariableDeclaration> fields, string prefix)
		{
			this.fields = fields;
			this.prefix = prefix;
		}
		
		public void Run(TypeDeclaration typeDeclaration)
		{
			base.Visit(typeDeclaration, null);
			foreach (VariableDeclaration decl in fields) {
				decl.Name = prefix + decl.Name;
			}
		}
		
		public override object Visit(TypeDeclaration typeDeclaration, object data)
		{
			// TODO: treat fields of base types like locals
			return base.Visit(typeDeclaration, data);
		}
		
		public override object Visit(BlockStatement blockStatement, object data)
		{
			Push();
			object result = base.Visit(blockStatement, data);
			Pop();
			return result;
		}
		
		public override object Visit(MethodDeclaration methodDeclaration, object data)
		{
			Push();
			object result = base.Visit(methodDeclaration, data);
			Pop();
			return result;
		}
		
		public override object Visit(PropertyGetRegion p, object data)
		{
			Push();
			object result = base.Visit(p, data);
			Pop();
			return result;
		}
		
		public override object Visit(PropertySetRegion p, object data)
		{
			Push();
			object result = base.Visit(p, data);
			Pop();
			return result;
		}
		
		public override object Visit(ConstructorDeclaration constructorDeclaration, object data)
		{
			Push();
			object result = base.Visit(constructorDeclaration, data);
			Pop();
			return result;
		}
		
		private void Push()
		{
			blocks.Push(curBlock);
			curBlock = new List<string>();
		}
		
		private void Pop()
		{
			curBlock = blocks.Pop();
		}
		
		public override object Visit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			foreach (VariableDeclaration decl in localVariableDeclaration.Variables) {
				//print("add variable ${decl.Name} to block")
				curBlock.Add(decl.Name);
			}
			return base.Visit(localVariableDeclaration, data);
		}
		
		public override object Visit(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			curBlock.Add(parameterDeclarationExpression.ParameterName);
			//print("add parameter ${parameterDeclarationExpression.ParameterName} to block")
			return base.Visit(parameterDeclarationExpression, data);
		}
		
		public override object Visit(IdentifierExpression identifierExpression, object data)
		{
			string name = identifierExpression.Identifier;
			foreach (VariableDeclaration var in fields) {
				if (var.Name == name && !IsLocal(name)) {
					identifierExpression.Identifier = prefix + name;
					break;
				}
			}
			return base.Visit(identifierExpression, data);
		}
		
		public override object Visit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			if (fieldReferenceExpression.TargetObject is ThisReferenceExpression) {
				string name = fieldReferenceExpression.FieldName;
				foreach (VariableDeclaration var in fields) {
					if (var.Name == name) {
						fieldReferenceExpression.FieldName = prefix + name;
						break;
					}
				}
			}
			return base.Visit(fieldReferenceExpression, data);
		}
		
		bool IsLocal(string name)
		{
			foreach (List<string> block in blocks) {
				if (block.Contains(name))
					return true;
			}
			return curBlock.Contains(name);
		}
		
		/*
		public override object Visit(invocationExpression as InvocationExpression, object data)
		{
			// this method is a workaround for a bug in SharpRefactory
			result = data
			if invocationExpression.TargetObject != null:
				result = invocationExpression.TargetObject.AcceptVisitor(self, data)
			if invocationExpression.Parameters != null:
				for n as INode in invocationExpression.Parameters:
					n.AcceptVisitor(self, data)
			return result
		}
		
		public override object Visit(indexerExpression as IndexerExpression, object data)
		{
			// this method is a workaround for a bug in SharpRefactory
			result = indexerExpression.TargetObject.AcceptVisitor(self, data)
			for n as INode in indexerExpression.Indices {
				n.AcceptVisitor(self, data)
			}
			return result
		}
		 */
	}
}
