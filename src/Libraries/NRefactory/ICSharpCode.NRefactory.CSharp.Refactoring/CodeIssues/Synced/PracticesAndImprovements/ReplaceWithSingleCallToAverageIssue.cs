// 
// ReplaceWithSingleCallToAverageIssue.cs
//
// Author:
//       Marius Ungureanu <marius.ungureanu@xamarin.com>
// 
// Copyright (c) 2014 Xamarin <http://xamarin.com>
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
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Replace with single call to Average(...)",
		Description = "Replace with single call to Average(...)",
		Category = IssueCategories.PracticesAndImprovements,
		Severity = Severity.Suggestion,
		AnalysisDisableKeyword = "ReplaceWithSingleCallToAverage")]
	public class ReplaceWithSingleCallToAverageIssue : GatherVisitorCodeIssueProvider
	{
		static readonly AstNode pattern =
			new InvocationExpression (
				new MemberReferenceExpression (
					new NamedNode ("selectInvoke",
						new InvocationExpression (
							new MemberReferenceExpression (new AnyNode ("target"), "Select"),
							new AnyNode ())),
					Pattern.AnyString));

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor<ReplaceWithSingleCallToAverageIssue>(context, "Average");
		}

		internal class GatherVisitor<T> : GatherVisitorBase<T> where T : GatherVisitorCodeIssueProvider
		{
			readonly string member;

			public GatherVisitor (BaseRefactoringContext ctx, string member) : base (ctx)
			{
				this.member = member;
			}

			public override void VisitInvocationExpression (InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression (invocationExpression);

				var match = pattern.Match (invocationExpression);
				if (!match.Success)
					return;

				var averageResolve = ctx.Resolve (invocationExpression) as InvocationResolveResult;
				if (averageResolve == null || !HasPredicateVersion(averageResolve.Member))
					return;
				var selectInvoke = match.Get<InvocationExpression> ("selectInvoke").Single ();
				var selectResolve = ctx.Resolve (selectInvoke) as InvocationResolveResult;
				if (selectResolve == null || selectResolve.Member.Name != "Select" || !IsQueryExtensionClass(selectResolve.Member.DeclaringTypeDefinition))
					return;
				if (selectResolve.Member.Parameters.Count != 2)
					return;
				var predResolve = selectResolve.Member.Parameters [1];
				if (predResolve.Type.TypeParameterCount != 2)
					return;

				AddIssue(new CodeIssue(
					invocationExpression, string.Format(ctx.TranslateString("Redundant Select() call with predicate followed by {0}()"), averageResolve.Member.Name),
					new CodeAction (
						string.Format(ctx.TranslateString("Replace with single call to '{0}'"), averageResolve.Member.Name),
						script => {
							var arg = selectInvoke.Arguments.Single ().Clone ();
							var target = match.Get<Expression> ("target").Single ().Clone ();
							script.Replace (invocationExpression, new InvocationExpression (new MemberReferenceExpression (target, averageResolve.Member.Name), arg));
						},
						invocationExpression
					)
				));
			}

			static bool IsQueryExtensionClass(ITypeDefinition typeDef)
			{
				if (typeDef == null || typeDef.Namespace != "System.Linq")
					return false;
				switch (typeDef.Name) {
					case "Enumerable":
					case "ParallelEnumerable":
					case "Queryable":
						return true;
					default:
						return false;
				}
			}

			bool HasPredicateVersion(IParameterizedMember member)
			{
				if (!IsQueryExtensionClass(member.DeclaringTypeDefinition))
					return false;
				return member.Name == this.member;
			}
		}
	}
}


