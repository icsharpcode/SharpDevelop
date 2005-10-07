// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
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
		
		private void InferResult(Expression expr, string name, LexicalInfo lexicalInfo)
		{
			if (expr == null)
				return;
			if (result != null)
				return;
			result = new DefaultField.LocalVariableField(new InferredReturnType(expr), name,
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
				InferResult(node.Initializer, node.Declaration.Name, node.LexicalInfo);
			}
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (acceptImplicit) {
				ReferenceExpression reference = node.Left as ReferenceExpression;
				if (node.Operator == BinaryOperatorType.Assign && reference != null) {
					if (!(reference is MemberReferenceExpression)) {
						if (reference.Name == lookFor) {
							InferResult(node.Right, reference.Name, reference.LexicalInfo);
						}
					}
				}
			}
			base.OnBinaryExpression(node);
		}
	}
	
	/// <summary>
	/// Creates a hashtable name => (Expression or TypeReference) for the local
	/// variables in the block that is visited.
	/// </summary>
	public class VariableListLookupVisitor : DepthFirstVisitor
	{
		ArrayList knownVariableNames;
		
		public VariableListLookupVisitor(ArrayList knownVariableNames)
		{
			this.knownVariableNames = knownVariableNames;
		}
		
		Hashtable results = new Hashtable();
		
		public Hashtable Results {
			get {
				return results;
			}
		}
		
		private void Add(string name, Expression expr)
		{
			if (name == null || expr == null)
				return;
			if (results.ContainsKey(name))
				return;
			results.Add(name, expr);
		}
		
		private void Add(string name, TypeReference reference)
		{
			if (name == null || reference == null)
				return;
			if (results.ContainsKey(name))
				return;
			results.Add(name, reference);
		}
		
		public override void OnDeclaration(Declaration node)
		{
			Add(node.Name, node.Type);
		}
		
		public override void OnDeclarationStatement(DeclarationStatement node)
		{
			Visit(node.Declaration);
			Add(node.Declaration.Name, node.Initializer);
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (node.Operator == BinaryOperatorType.Assign && node.Left is ReferenceExpression) {
				ReferenceExpression reference = node.Left as ReferenceExpression;
				if (node.Operator == BinaryOperatorType.Assign && reference != null) {
					if (!(reference is MemberReferenceExpression)) {
						if (!knownVariableNames.Contains(reference.Name)) {
							Add(reference.Name, node.Right);
						}
					}
				}
			}
			base.OnBinaryExpression(node);
		}
	}
}
