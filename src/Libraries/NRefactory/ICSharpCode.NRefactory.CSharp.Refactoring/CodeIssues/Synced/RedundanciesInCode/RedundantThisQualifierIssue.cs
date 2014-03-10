// 
// RedundantThisInspector.cs
// 
// RedundantThisInspector.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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

using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using System.Diagnostics;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.CSharp.Analysis;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Finds redundant this usages.
	/// </summary>
	[IssueDescription("Redundant 'this.' qualifier",
	                  Description= "'this.' is redundant and can be removed safely.",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "RedundantThisQualifier")]
	[SubIssueAttribute(RedundantThisQualifierIssue.InsideConstructors)]
	[SubIssueAttribute(RedundantThisQualifierIssue.EverywhereElse)]
	public class RedundantThisQualifierIssue : GatherVisitorCodeIssueProvider
	{
		public const string InsideConstructors = "Inside constructors";
		public const string EverywhereElse = "Everywhere else";

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			var declarationSpaceVisitor = new LocalDeclarationSpaceVisitor();
			context.RootNode.AcceptVisitor(declarationSpaceVisitor);

			return new GatherVisitor(declarationSpaceVisitor, context, this);
		}

		class GatherVisitor : GatherVisitorBase<RedundantThisQualifierIssue>
		{
			bool InsideConstructors {
				get {
					return SubIssue == RedundantThisQualifierIssue.InsideConstructors;
				}
			}

			LocalDeclarationSpaceVisitor declarationsSpaceVisitor;

			public GatherVisitor (LocalDeclarationSpaceVisitor visitor, BaseRefactoringContext ctx, RedundantThisQualifierIssue qualifierDirectiveEvidentIssueProvider) : base (ctx, qualifierDirectiveEvidentIssueProvider)
			{
				declarationsSpaceVisitor = visitor;
			}

			static IMember GetMember (ResolveResult result)
			{
				if (result is MemberResolveResult) {
					return ((MemberResolveResult)result).Member;
				}
				if (result is MethodGroupResolveResult) {
					return ((MethodGroupResolveResult)result).Methods.FirstOrDefault();
				}
				return null;
			}

			public override void VisitSyntaxTree(SyntaxTree syntaxTree)
			{
				base.VisitSyntaxTree(syntaxTree);
				AnalyzeAllThisReferences();
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				if (InsideConstructors)
					base.VisitConstructorDeclaration(constructorDeclaration);
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				if (!InsideConstructors)
					base.VisitMethodDeclaration(methodDeclaration);
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
				if (!InsideConstructors)
					base.VisitIndexerDeclaration(indexerDeclaration);
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
				if (!InsideConstructors)
					base.VisitCustomEventDeclaration(eventDeclaration);
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				if (!InsideConstructors)
					base.VisitDestructorDeclaration(destructorDeclaration);
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				if (!InsideConstructors)
					base.VisitFieldDeclaration(fieldDeclaration);
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
				if (!InsideConstructors)
					base.VisitOperatorDeclaration(operatorDeclaration);
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				if (!InsideConstructors)
					base.VisitPropertyDeclaration(propertyDeclaration);
			}

			IList<ThisReferenceExpression> thisReferences = new List<ThisReferenceExpression> ();

			public override void VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression)
			{
				base.VisitThisReferenceExpression(thisReferenceExpression);
				if (!IsSuppressed(thisReferenceExpression.Location))
					thisReferences.Add(thisReferenceExpression);
			}

			void AnalyzeAllThisReferences ()
			{
				foreach (var thisReference in thisReferences) {
					AnalyzeThisReferenceExpression(thisReference);
				}
			}

			void AnalyzeThisReferenceExpression (ThisReferenceExpression thisReferenceExpression)
			{
				var memberReference = thisReferenceExpression.Parent as MemberReferenceExpression;
				if (memberReference == null) {
					return;
				}

				var state = ctx.GetResolverStateAfter(thisReferenceExpression);
				var wholeResult = ctx.Resolve(memberReference);
			
				IMember member = GetMember(wholeResult);
				if (member == null) { 
					return;
				}

				var localDeclarationSpace = declarationsSpaceVisitor.GetDeclarationSpace(thisReferenceExpression);
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

				if (isRedundant) {
					var issueDescription = ctx.TranslateString("'this.' is redundant and can be removed safely.");
					var actionDescription = ctx.TranslateString("Remove 'this.'");
					AddIssue(new CodeIssue(thisReferenceExpression.StartLocation, memberReference.MemberNameToken.StartLocation, issueDescription, actionDescription, script => {
						script.Replace(memberReference, RefactoringAstHelper.RemoveTarget(memberReference));
					}) { IssueMarker = IssueMarker.GrayOut });
				}
			}
		}
	}
}