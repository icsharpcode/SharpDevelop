//
// StringIndexOfIsCultureSpecificIssue.cs
//
// Author:
//       Daniel Grunwald <daniel@danielgrunwald.de>
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2012 Daniel Grunwald
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'string.IndexOf' is culture-aware",
	                  Description = "Warns when a culture-aware 'IndexOf' call is used by default.",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "StringIndexOfIsCultureSpecific")]
	public class StringIndexOfIsCultureSpecificIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor<StringIndexOfIsCultureSpecificIssue>(context, "IndexOf");
		}

		internal class GatherVisitor<T> : GatherVisitorBase<T> where T : GatherVisitorCodeIssueProvider
		{
			readonly string memberName;

			public GatherVisitor(BaseRefactoringContext ctx, string memberName) : base(ctx)
			{
				this.memberName = memberName;
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);

				MemberReferenceExpression mre = invocationExpression.Target as MemberReferenceExpression;
				if (mre == null)
					return;
				if (mre.MemberName != memberName)
					return;

				var rr = ctx.Resolve(invocationExpression) as InvocationResolveResult;
				if (rr == null || rr.IsError) {
					// Not an invocation resolve result - e.g. could be a UnknownMemberResolveResult instead
					return;
				}
				if (!(rr.Member.DeclaringTypeDefinition != null && rr.Member.DeclaringTypeDefinition.KnownTypeCode == KnownTypeCode.String)) {
					// Not a string operation
					return;
				}
				IParameter firstParameter = rr.Member.Parameters.FirstOrDefault();
				if (firstParameter == null || !firstParameter.Type.IsKnownType(KnownTypeCode.String))
					return; // First parameter not a string
				IParameter lastParameter = rr.Member.Parameters.Last();
				if (lastParameter.Type.Name == "StringComparison")
					return; // already specifying a string comparison
				AddIssue(new CodeIssue(
					invocationExpression.LParToken.StartLocation, 
					invocationExpression.RParToken.EndLocation,
					string.Format(ctx.TranslateString("'{0}' is culture-aware and missing a StringComparison argument"), rr.Member.FullName),
					new CodeAction(ctx.TranslateString("Add 'StringComparison.Ordinal'"), script => AddArgument(script, invocationExpression, "Ordinal"), invocationExpression),
					new CodeAction(ctx.TranslateString("Add 'StringComparison.CurrentCulture'"), script => AddArgument(script, invocationExpression, "CurrentCulture"), invocationExpression)
				));
			}

			void AddArgument(Script script, InvocationExpression invocationExpression, string stringComparison)
			{
				var astBuilder = ctx.CreateTypeSystemAstBuilder(invocationExpression);
				var newArgument = astBuilder.ConvertType(new TopLevelTypeName("System", "StringComparison")).Member(stringComparison);
				var copy = (InvocationExpression)invocationExpression.Clone();
				copy.Arguments.Add(newArgument);
				script.Replace(invocationExpression, copy);
			}
		}
	}
}

