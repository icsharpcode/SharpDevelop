// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

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
		
		SourceLocation GetEndSourceLocation(Node node)
		{
			if (node.EndSourceLocation.IsValid) return node.EndSourceLocation;
			if (node is BlockExpression) {
				return GetEndSourceLocation((node as BlockExpression).Body);
			} else if (node is ForStatement) {
				return GetEndSourceLocation((node as ForStatement).Block);
			} else if (node is ExceptionHandler) {
				return GetEndSourceLocation((node as ExceptionHandler).Block);
			} else if (node is Block) {
				StatementCollection st = (node as Block).Statements;
				if (st.Count > 0) {
					return GetEndSourceLocation(st[st.Count - 1]);
				}
			}
			return node.EndSourceLocation;
		}
		
		public override void OnBlockExpression(BlockExpression node)
		{
			if (node.LexicalInfo.Line <= resolver.CaretLine && GetEndSourceLocation(node).Line >= resolver.CaretLine - 1) {
				foreach (ParameterDeclaration param in node.Parameters) {
					DeclarationFound(param.Name, param.Type ?? (resolver.IsDucky ? new SimpleTypeReference("duck") : new SimpleTypeReference("object")), null, param.LexicalInfo);
				}
				base.OnBlockExpression(node);
			}
		}
		
		protected override void OnError(Node node, Exception error)
		{
			MessageService.ShowException(error, "VariableLookupVisitor: error processing " + node);
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (acceptImplicit) {
				ReferenceExpression reference = node.Left as ReferenceExpression;
				if (node.Operator == BinaryOperatorType.Assign && reference != null) {
					if (!(reference is MemberReferenceExpression)) {
						DeclarationFound(reference.Name, null, node.Right, reference.LexicalInfo);
					}
				}
			}
			base.OnBinaryExpression(node);
		}
		
		public override void OnForStatement(ForStatement node)
		{
			if (node.LexicalInfo.Line <= resolver.CaretLine && GetEndSourceLocation(node).Line >= resolver.CaretLine - 1) {
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
		
		public override void OnExceptionHandler(ExceptionHandler node)
		{
			if (node.Declaration != null) {
				if (node.LexicalInfo.Line <= resolver.CaretLine && GetEndSourceLocation(node).Line >= resolver.CaretLine) {
					DeclarationFound(node.Declaration.Name, node.Declaration.Type ?? new SimpleTypeReference("System.Exception"), null, node.Declaration.LexicalInfo);
				}
			}
			base.OnExceptionHandler(node);
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
			// Prevent creating an infinite number of InferredReturnTypes in inferring cycles
			IReturnType returnType;
			if (expr.ContainsAnnotation("DomReturnType")) {
				returnType = (IReturnType)expr["DomReturnType"];
			} else {
				returnType = new BooInferredReturnType(expr, resolver.CallingClass);
				expr.Annotate("DomReturnType", returnType);
			}
			if (useElementType)
				returnType = new ElementReturnType(resolver.ProjectContent, returnType);
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
				results.Add(name, new ElementReturnType(resolver.ProjectContent,
				                                        new BooInferredReturnType(expr, resolver.CallingClass)));
			else
				results.Add(name, new BooInferredReturnType(expr, resolver.CallingClass));
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
				if (!knownVariableNames.Contains(declarationName)) {
					Add(declarationName, initializer, false);
				}
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
