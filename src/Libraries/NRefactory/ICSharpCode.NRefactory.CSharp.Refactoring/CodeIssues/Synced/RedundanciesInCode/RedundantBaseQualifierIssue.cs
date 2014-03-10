//
// RedundantBaseQualifierIssue.cs
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
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Analysis;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Finds redundant base qualifier 
	/// </summary>
	[IssueDescription("Redundant 'base.' qualifier",
	                  Description = "'base.' is redundant and can safely be removed.",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "RedundantBaseQualifier")]
	public class RedundantBaseQualifierIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			var declarationSpaceVisitor = new LocalDeclarationSpaceVisitor();
			context.RootNode.AcceptVisitor(declarationSpaceVisitor);

			return new GatherVisitor(context, declarationSpaceVisitor);
		}

		class GatherVisitor : GatherVisitorBase<RedundantBaseQualifierIssue>
		{
			LocalDeclarationSpaceVisitor declarationsSpaceVisitor;

			public GatherVisitor(BaseRefactoringContext ctx, LocalDeclarationSpaceVisitor visitor) : base (ctx)
			{
				declarationsSpaceVisitor = visitor;
			}

			public override void VisitSyntaxTree(SyntaxTree syntaxTree)
			{
				base.VisitSyntaxTree(syntaxTree);
				AnalyzeAllThisReferences();
			}

			static IMember GetMember(ResolveResult result)
			{
				if (result is MemberResolveResult)
					return ((MemberResolveResult)result).Member;
				if (result is MethodGroupResolveResult)
					return ((MethodGroupResolveResult)result).Methods.FirstOrDefault();
				return null;
			}

			readonly IList<BaseReferenceExpression> thisReferences = new List<BaseReferenceExpression>();

			public override void VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression)
			{
				base.VisitBaseReferenceExpression(baseReferenceExpression);
				if (!IsSuppressed(baseReferenceExpression.Location))
					thisReferences.Add(baseReferenceExpression);
			}

			void AnalyzeAllThisReferences()
			{
				foreach (var thisReference in thisReferences) {
					AnalyzeThisReferenceExpression(thisReference);
				}
			}

			void AnalyzeThisReferenceExpression(BaseReferenceExpression baseReferenceExpression)
			{
				var memberReference = baseReferenceExpression.Parent as MemberReferenceExpression;
				if (memberReference == null) {
					return;
				}

				var state = ctx.GetResolverStateAfter(baseReferenceExpression);
				var wholeResult = ctx.Resolve(memberReference);

				IMember member = GetMember(wholeResult);
				if (member == null)
					return;

				var localDeclarationSpace = declarationsSpaceVisitor.GetDeclarationSpace(baseReferenceExpression);
				if (localDeclarationSpace == null || localDeclarationSpace.IsNameUsed(member.Name))
					return;

				var result = state.LookupSimpleNameOrTypeName(memberReference.MemberName, EmptyList<IType>.Instance, NameLookupMode.Expression);
				var parentResult = ctx.Resolve(memberReference.Parent) as CSharpInvocationResolveResult;

				bool isRedundant;
				if (result is MemberResolveResult) {
					isRedundant = ((MemberResolveResult)result).Member.Region.Equals(member.Region);
				} else if (parentResult != null && parentResult.IsExtensionMethodInvocation) {
					// 'this.' is required for extension method invocation
					isRedundant = false;
				} else if (result is MethodGroupResolveResult) {
					isRedundant = ((MethodGroupResolveResult)result).Methods.Any(m => m.Region.Equals(member.Region));
				} else {
					return;
				}

				var basicMembers = state.CurrentTypeDefinition.DirectBaseTypes.First().GetMembers();
				var extendedMembers = state.CurrentTypeDefinition.GetMembers().Except(basicMembers);
				if (extendedMembers.Any(f => f.Name.Equals(member.Name)))
					return;

				if (isRedundant) {
					AddIssue(new CodeIssue(
						baseReferenceExpression.StartLocation, 
						memberReference.MemberNameToken.StartLocation, 
						ctx.TranslateString("'base.' is redundant and can be removed safely."), 
						ctx.TranslateString("Remove 'base.'"), 
						script => {
							script.Replace(memberReference, RefactoringAstHelper.RemoveTarget(memberReference));
						}
					) { IssueMarker = IssueMarker.GrayOut });
				}
			}
		}
	}
}