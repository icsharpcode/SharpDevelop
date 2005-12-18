// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using Boo.Lang.Compiler.Ast;

namespace Grunwald.BooBinding.CodeCompletion
{
	/// <summary>
	/// Finds an variable declaration in the boo AST.
	/// </summary>
	public class VariableLookupVisitor : DepthFirstVisitor
	{
		BooResolver resolver;
		string lookFor;
		bool acceptImplicit;
		
		public VariableLookupVisitor(BooResolver resolver, string lookFor, bool acceptImplicit)
		{
			this.resolver = resolver;
			this.lookFor = lookFor;
			this.acceptImplicit = acceptImplicit;
		}
		
		IField result;
		
		public IField Result {
			get {
				return result;
			}
		}
		
		private void InferResult(Expression expr, string name, LexicalInfo lexicalInfo, bool useElementType)
		{
			if (expr == null)
				return;
			if (result != null)
				return;
			IReturnType returnType = new InferredReturnType(expr, resolver.CallingClass);
			if (useElementType)
				returnType = new ElementReturnType(returnType);
			result = new DefaultField.LocalVariableField(returnType, name,
			                                             new DomRegion(lexicalInfo.Line, lexicalInfo.Column),
			                                             resolver.CallingClass);
		}
		
		public override void OnDeclaration(Declaration node)
		{
			if (result != null)
				return;
			if (node.Name == lookFor) {
				if (node.Type != null) {
					result = new DefaultField.LocalVariableField(resolver.ConvertType(node.Type),
					                                             node.Name,
					                                             new DomRegion(node.LexicalInfo.Line, node.LexicalInfo.Column),
					                                             resolver.CallingClass);
				}
			}
		}
		
		public override void OnDeclarationStatement(DeclarationStatement node)
		{
			if (node.Declaration.Name == lookFor) {
				Visit(node.Declaration);
				InferResult(node.Initializer, node.Declaration.Name, node.LexicalInfo, false);
			}
		}
		
		protected override void OnError(Node node, Exception error)
		{
			MessageService.ShowError(error, "VariableLookupVisitor: error processing " + node);
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (acceptImplicit) {
				ReferenceExpression reference = node.Left as ReferenceExpression;
				if (node.Operator == BinaryOperatorType.Assign && reference != null) {
					if (!(reference is MemberReferenceExpression)) {
						if (reference.Name == lookFor) {
							InferResult(node.Right, reference.Name, reference.LexicalInfo, false);
						}
					}
				}
			}
			base.OnBinaryExpression(node);
		}
		
		public override void OnForStatement(ForStatement node)
		{
			if (node.LexicalInfo.Line > resolver.CaretLine || node.Block.EndSourceLocation.Line < resolver.CaretLine)
				return;
			
			if (node.Declarations.Count != 1) {
				// TODO: support unpacking
				base.OnForStatement(node);
				return;
			}
			if (node.Declarations[0].Name == lookFor) {
				Visit(node.Declarations[0]);
				InferResult(node.Iterator, node.Declarations[0].Name, node.LexicalInfo, true);
			}
		}
		
		public override void OnGeneratorExpression(GeneratorExpression node)
		{
			if (node.LexicalInfo.Line != resolver.CaretLine)
				return;
			LoggingService.Warn("GeneratorExpression: " + node.EndSourceLocation.Line);
			if (node.Declarations.Count != 1) {
				// TODO: support unpacking
				base.OnGeneratorExpression(node);
				return;
			}
			if (node.Declarations[0].Name == lookFor) {
				Visit(node.Declarations[0]);
				InferResult(node.Iterator, node.Declarations[0].Name, node.LexicalInfo, true);
			}
			base.OnGeneratorExpression(node);
		}
	}
	
	/// <summary>
	/// Creates a hashtable name => (Expression or TypeReference) for the local
	/// variables in the block that is visited.
	/// </summary>
	public class VariableListLookupVisitor : DepthFirstVisitor
	{
		List<string> knownVariableNames;
		BooResolver resolver;
		
		public VariableListLookupVisitor(List<string> knownVariableNames, BooResolver resolver)
		{
			this.knownVariableNames = knownVariableNames;
			this.resolver = resolver;
		}
		
		Dictionary<string, IReturnType> results = new Dictionary<string, IReturnType>();
		
		public Dictionary<string, IReturnType> Results {
			get {
				return results;
			}
		}
		
		private void Add(string name, Expression expr, bool elementReturnType)
		{
			if (name == null || expr == null)
				return;
			if (results.ContainsKey(name))
				return;
			if (elementReturnType)
				results.Add(name, new ElementReturnType(new InferredReturnType(expr, resolver.CallingClass)));
			else
				results.Add(name, new InferredReturnType(expr, resolver.CallingClass));
		}
		
		private void Add(string name, TypeReference reference)
		{
			if (name == null || reference == null)
				return;
			if (results.ContainsKey(name))
				return;
			results.Add(name, resolver.ConvertType(reference));
		}
		
		public override void OnDeclaration(Declaration node)
		{
			Add(node.Name, node.Type);
		}
		
		public override void OnDeclarationStatement(DeclarationStatement node)
		{
			Visit(node.Declaration);
			Add(node.Declaration.Name, node.Initializer, false);
		}
		
		protected override void OnError(Node node, Exception error)
		{
			MessageService.ShowError(error, "VariableListLookupVisitor: error processing " + node);
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (node.Operator == BinaryOperatorType.Assign && node.Left is ReferenceExpression) {
				ReferenceExpression reference = node.Left as ReferenceExpression;
				if (node.Operator == BinaryOperatorType.Assign && reference != null) {
					if (!(reference is MemberReferenceExpression)) {
						if (!knownVariableNames.Contains(reference.Name)) {
							Add(reference.Name, node.Right, false);
						}
					}
				}
			}
			base.OnBinaryExpression(node);
		}
		
		public override void OnForStatement(ForStatement node)
		{
			if (node.LexicalInfo.Line > resolver.CaretLine || node.Block.EndSourceLocation.Line < resolver.CaretLine)
				return;
			
			if (node.Declarations.Count != 1) {
				// TODO: support unpacking
				base.OnForStatement(node);
				return;
			}
			Add(node.Declarations[0].Name, node.Iterator, true);
		}
	}
}
