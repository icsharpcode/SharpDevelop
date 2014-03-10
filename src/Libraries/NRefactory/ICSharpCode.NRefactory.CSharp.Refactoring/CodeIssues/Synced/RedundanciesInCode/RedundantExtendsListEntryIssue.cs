// 
// RedundantBaseTypeIssue.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013  Ji Kun <jikun.nus@gmail.com>
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
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Type is either mentioned in the base type list of other part, or it is interface and appears as other's type base and contains no explicit implementation.
	/// </summary>
	[IssueDescription("Redundant class or interface specification in base types list",
	                  Description= "Type is either mentioned in the base type list of another part or in another base type",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "RedundantExtendsListEntry")]
	public class RedundantExtendsListEntryIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this);
		}
		
		class GatherVisitor : GatherVisitorBase<RedundantExtendsListEntryIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx, RedundantExtendsListEntryIssue issueProvider) : base (ctx, issueProvider)
			{
			}
			
			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (typeDeclaration == null)
					return;
				
				base.VisitTypeDeclaration(typeDeclaration);
				
				if (typeDeclaration.BaseTypes.Count == 0)
					return;
				
				List<AstNode> redundantBase = new List<AstNode>();
				var type = ctx.Resolve(typeDeclaration).Type;

				if (typeDeclaration.HasModifier(Modifiers.Partial) && type.GetDefinition() != null) {
					var parts = type.GetDefinition().Parts;
					foreach (var node in typeDeclaration.BaseTypes) {
						int count = 0;
						foreach (var unresolvedTypeDefinition in parts) {
							var baseTypes = unresolvedTypeDefinition.BaseTypes;
							
							if (baseTypes.Any(f => f.ToString().Equals(node.ToString()))) {
								count++;
								if (count > 1) {
									if (!redundantBase.Contains(node))
										redundantBase.Add(node);
									break;
								}
							}
						}
					}
				}
				
				var directBaseType = type.DirectBaseTypes.Where(f => f.Kind == TypeKind.Class);
				if (directBaseType.Count() != 1)
					return;
				var members = type.GetMembers();
				var memberDeclaration = typeDeclaration.Members;
				var interfaceBase = typeDeclaration.BaseTypes.Where(delegate(AstType f) {
					var resolveResult = ctx.Resolve(f);
					if (resolveResult.IsError || resolveResult.Type.GetDefinition() == null)
						return false;
					return resolveResult.Type.GetDefinition().Kind == TypeKind.Interface;
				});
				foreach (var node in interfaceBase) {
					if (directBaseType.Single().GetDefinition().GetAllBaseTypeDefinitions().Any(f => f.Name.Equals(node.ToString()))) {
						bool flag = false;
						foreach (var member in members) {
							if (!memberDeclaration.Any(f => f.Name.Equals(member.Name))) {
								continue;
							}
							if (
								member.ImplementedInterfaceMembers.Any(
								g => g.DeclaringType.Name.Equals(node.ToString()))) {
								flag = true;
								break;
							}
						}
						if (!flag) {
							if (!redundantBase.Contains(node))
								redundantBase.Add(node);
						}
					}			
				}
				foreach (var node in redundantBase) {
					var nodeType = ctx.Resolve(node).Type;
					var issueText = nodeType.Kind == TypeKind.Interface ?
						ctx.TranslateString("Base interface '{0}' is redundant") :
						ctx.TranslateString("Base type '{0}' is already specified in other parts");

					AddIssue(new CodeIssue(
						node,
						string.Format(issueText, nodeType.Name), 
						new CodeAction (
							ctx.TranslateString("Remove redundant base type reference"),
							script => {
								if (typeDeclaration.GetCSharpNodeBefore(node).ToString().Equals(":")) {
									if (node.GetNextNode().Role != Roles.BaseType) {
										script.Remove(typeDeclaration.GetCSharpNodeBefore(node));
									}
								}
								if (typeDeclaration.GetCSharpNodeBefore(node).ToString().Equals(",")) {
									script.Remove(typeDeclaration.GetCSharpNodeBefore(node));
								}
								script.Remove(node);
							},
						node)
					) { IssueMarker = IssueMarker.GrayOut });
				}
			}
			
		}
	}
}