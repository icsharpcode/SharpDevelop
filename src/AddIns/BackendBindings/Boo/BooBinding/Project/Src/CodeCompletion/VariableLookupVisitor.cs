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
	public abstract class VariableLookupVisitorBase : DepthFirstVisitor
	{
		protected BooResolver resolver;
		protected bool acceptImplicit = true;
		
		protected abstract void DeclarationFound(string declarationName, TypeReference declarationType, Expression initializer, LexicalInfo lexicalInfo);
		protected abstract void IterationDeclarationFound(string declarationName, TypeReference declarationType, Expression initializer, LexicalInfo lexicalInfo);
		
		public override void OnDeclarationStatement(DeclarationStatement node)
		{
			DeclarationFound(node.Declaration.Name, node.Declaration.Type, node.Initializer, node.LexicalInfo);
		}
		
		public override void OnParameterDeclaration(ParameterDeclaration node)
		{
			DeclarationFound(node.Name, node.Type, null, node.LexicalInfo);
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
						DeclarationFound(reference.Name, null, node.Right, node.LexicalInfo);
					}
				}
			}
			base.OnBinaryExpression(node);
		}
		
		public override void OnForStatement(ForStatement node)
		{
			if (node.LexicalInfo.Line <= resolver.CaretLine && node.Block.EndSourceLocation.Line >= resolver.CaretLine - 1) {
				foreach (Declaration decl in node.Declarations) {
					IterationDeclarationFound(decl.Name, decl.Type, node.Iterator, node.LexicalInfo);
				}
			}
			base.OnForStatement(node);
		}
		
		public override void OnGeneratorExpression(GeneratorExpression node)
		{
			if (node.LexicalInfo.Line != resolver.CaretLine)
				return;
			LoggingService.Warn("GeneratorExpression: " + node.EndSourceLocation.Line);
			foreach (Declaration decl in node.Declarations) {
				IterationDeclarationFound(decl.Name, decl.Type, node.Iterator, node.LexicalInfo);
			}
			base.OnGeneratorExpression(node);
		}
		
		public override void OnUnpackStatement(UnpackStatement node)
		{
			ArrayLiteralExpression ale = node.Expression as ArrayLiteralExpression;
			for (int i = 0; i < node.Declarations.Count; i++) {
				Declaration decl = node.Declarations[i];
				if (acceptImplicit && ale != null && ale.Items.Count > i) {
					DeclarationFound(decl.Name, decl.Type, ale.Items[i], decl.LexicalInfo);
				} else if (decl.Type != null) {
					DeclarationFound(decl.Name, decl.Type, null, decl.LexicalInfo);
				}
			}
		}
	}
	
	/// <summary>
	/// Finds an variable declaration in the boo AST.
	/// </summary>
	public class VariableLookupVisitor : VariableLookupVisitorBase
	{
		string lookFor;
		
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
		
		protected override void IterationDeclarationFound(string declarationName, TypeReference declarationType, Expression iterator, LexicalInfo lexicalInfo)
		{
			if (result != null)
				return;
			if (declarationName == lookFor) {
				if (declarationType != null) {
					result = new DefaultField.LocalVariableField(resolver.ConvertType(declarationType),
					                                             declarationName,
					                                             new DomRegion(lexicalInfo.Line, lexicalInfo.Column),
					                                             resolver.CallingClass);
				} else if (iterator != null) {
					InferResult(iterator, declarationName, lexicalInfo, true);
				}
			}
		}
		
		protected override void DeclarationFound(string declarationName, TypeReference declarationType, Expression initializer, LexicalInfo lexicalInfo)
		{
			if (result != null)
				return;
			if (declarationName == lookFor) {
				if (declarationType != null) {
					result = new DefaultField.LocalVariableField(resolver.ConvertType(declarationType),
					                                             declarationName,
					                                             new DomRegion(lexicalInfo.Line, lexicalInfo.Column),
					                                             resolver.CallingClass);
				} else if (initializer != null) {
					InferResult(initializer, declarationName, lexicalInfo, false);
				}
			}
		}
		
		private void InferResult(Expression expr, string name, LexicalInfo lexicalInfo, bool useElementType)
		{
			if (expr == null)
				return;
			IReturnType returnType = new InferredReturnType(expr, resolver.CallingClass);
			if (useElementType)
				returnType = new ElementReturnType(returnType);
			result = new DefaultField.LocalVariableField(returnType, name,
			                                             new DomRegion(lexicalInfo.Line, lexicalInfo.Column),
			                                             resolver.CallingClass);
		}
	}
	
	/// <summary>
	/// Creates a hashtable name => (Expression or TypeReference) for the local
	/// variables in the block that is visited.
	/// </summary>
	public class VariableListLookupVisitor : VariableLookupVisitorBase
	{
		List<string> knownVariableNames;
		
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
		
		protected override void DeclarationFound(string declarationName, TypeReference declarationType, Expression initializer, LexicalInfo lexicalInfo)
		{
			if (declarationType != null) {
				Add(declarationName, declarationType);
			} else if (initializer != null) {
				Add(declarationName, initializer, false);
			}
		}
		
		protected override void IterationDeclarationFound(string declarationName, TypeReference declarationType, Expression initializer, LexicalInfo lexicalInfo)
		{
			if (declarationType != null) {
				Add(declarationName, declarationType);
			} else if (initializer != null) {
				Add(declarationName, initializer, true);
			}
		}
	}
}
