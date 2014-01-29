// Copyright (c) 2013 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// 1) When a type cannot be resolved, offers to add a using declaration
	/// or to replace it with the fully qualified type name.
	/// 2) When an extension method cannot be resolved, offers to add a using declaration.
	/// 3) When the caret is on a namespace name, offers to add a using declaration
	/// and simplify the type references to use the new shorter option.
	/// </summary>
	[ContextAction ("Add using", Description = "Add missing using declaration.")]
	public class AddUsingAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			AstNode node = context.GetNode();
			if (node is Identifier)
				node = node.Parent;
			if (node is SimpleType || node is IdentifierExpression) {
				return GetActionsForType(context, node)
					.Concat(GetActionsForAddNamespaceUsing(context, node));
			} else if (node is MemberReferenceExpression && node.Parent is InvocationExpression) {
				return GetActionsForExtensionMethodInvocation(context, (InvocationExpression)node.Parent);
			} else if (node is MemberReferenceExpression) {
				return GetActionsForAddNamespaceUsing(context, node);
			} else {
				return EmptyList<CodeAction>.Instance;
			}
		}
		
		IEnumerable<CodeAction> GetActionsForType(RefactoringContext context, AstNode node)
		{
			var rr = context.Resolve(node) as UnknownIdentifierResolveResult;
			if (rr == null)
				return EmptyList<CodeAction>.Instance;
			
			string identifier = rr.Identifier;
			int tc = rr.TypeArgumentCount;
			string attributeIdentifier = null;
			if (node.Parent is Attribute)
				attributeIdentifier = identifier + "Attribute";
			
			var lookup = new MemberLookup(null, context.Compilation.MainAssembly);
			List<CodeAction> actions = new List<CodeAction>();
			foreach (var typeDefinition in context.Compilation.GetAllTypeDefinitions()) {
				if ((typeDefinition.Name == identifier || typeDefinition.Name == attributeIdentifier)
				    && typeDefinition.TypeParameterCount == tc
				    && lookup.IsAccessible(typeDefinition, false))
				{
					if (typeDefinition.DeclaringTypeDefinition == null) {
						actions.Add(NewUsingAction(context, node, typeDefinition.Namespace));
					}
					actions.Add(ReplaceWithFullTypeNameAction(context, node, typeDefinition));
				}
			}
			return actions;
		}
		
		CodeAction NewUsingAction(RefactoringContext context, AstNode node, string ns)
		{
			return new CodeAction("using " + ns + ";", s => UsingHelper.InsertUsingAndRemoveRedundantNamespaceUsage(context, s, ns), node);
		}
		
		CodeAction ReplaceWithFullTypeNameAction(RefactoringContext context, AstNode node, ITypeDefinition typeDefinition)
		{
			AstType astType = context.CreateShortType(typeDefinition);
			string textWithoutGenerics = astType.ToString();
			foreach (var typeArg in node.GetChildrenByRole(Roles.TypeArgument)) {
				astType.AddChild(typeArg.Clone(), Roles.TypeArgument);
			}
			return new CodeAction(textWithoutGenerics, s => s.Replace(node, astType), node);
		}
		
		IEnumerable<CodeAction> GetActionsForExtensionMethodInvocation(RefactoringContext context, InvocationExpression invocation)
		{
			var rr = context.Resolve(invocation) as UnknownMethodResolveResult;
			if (rr == null)
				return EmptyList<CodeAction>.Instance;
			
			var lookup = new MemberLookup(null, context.Compilation.MainAssembly);
			HashSet<string> namespaces = new HashSet<string>();
			List<CodeAction> result = new List<CodeAction>();
			foreach (var typeDefinition in context.Compilation.GetAllTypeDefinitions()) {
				if (!(typeDefinition.HasExtensionMethods && lookup.IsAccessible(typeDefinition, false))) {
					continue;
				}
				foreach (var method in typeDefinition.Methods.Where(m => m.IsExtensionMethod && m.Name == rr.MemberName)) {
					IType[] inferredTypes;
					if (CSharpResolver.IsEligibleExtensionMethod(rr.TargetType, method, true, out inferredTypes)) {
						// avoid offering the same namespace twice
						if (namespaces.Add(typeDefinition.Namespace)) {
							result.Add(NewUsingAction(context, invocation, typeDefinition.Namespace));
						}
						break; // continue with the next type
					}
				}
			}
			return result;
		}
		
		IEnumerable<CodeAction> GetActionsForAddNamespaceUsing(RefactoringContext context, AstNode node)
		{
			var nrr = context.Resolve(node) as NamespaceResolveResult;
			if (nrr == null)
				return EmptyList<CodeAction>.Instance;
			
			var trr = context.Resolve(node.Parent) as TypeResolveResult;
			if (trr == null)
				return EmptyList<CodeAction>.Instance;
			ITypeDefinition typeDef = trr.Type.GetDefinition();
			if (typeDef == null)
				return EmptyList<CodeAction>.Instance;
			
			IList<IType> typeArguments;
			ParameterizedType parameterizedType = trr.Type as ParameterizedType;
			if (parameterizedType != null)
				typeArguments = parameterizedType.TypeArguments;
			else
				typeArguments = EmptyList<IType>.Instance;
			
			var resolver = context.GetResolverStateBefore(node.Parent);
			if (resolver.ResolveSimpleName(typeDef.Name, typeArguments) is UnknownIdentifierResolveResult) {
				// It's possible to remove the explicit namespace usage and introduce a using instead
				return new[] { NewUsingAction(context, node, typeDef.Namespace) };
			}
			return EmptyList<CodeAction>.Instance;
		}
	}
}
