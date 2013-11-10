// 
// UnusedAnonymousMethodSignatureIssue.cs
// 
// Author:
//      Luís Reis <luiscubal@gmail.com>
//      Mike Krüger <mkrueger@xamarn.com>
//
// Copyright (c) 2013 Luís Reis
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

using System.Collections.Generic;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Anonymous method signature is not required",
		Description = "Detects when no delegate parameter is used in the anonymous method body.",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "UnusedAnonymousMethodSignature")]
	public class UnusedAnonymousMethodSignatureIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<UnusedAnonymousMethodSignatureIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base(ctx)
			{
			}

			bool IsParameterListRedundant(Expression expression)
			{
				var validTypes = TypeGuessing.GetValidTypes(ctx.Resolver, expression);
				return validTypes.Count(t => t.Kind == TypeKind.Delegate) == 1;
			}

			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
				base.VisitAnonymousMethodExpression(anonymousMethodExpression);
				if (!anonymousMethodExpression.HasParameterList || !IsParameterListRedundant(anonymousMethodExpression))
					return;

				var parameters = anonymousMethodExpression.Parameters.ToList();
				if (parameters.Count > 0) {
					var usageAnalysis = new ConvertToConstantIssue.VariableUsageAnalyzation(ctx);
					anonymousMethodExpression.Body.AcceptVisitor(usageAnalysis); 
					foreach (var parameter in parameters) {
						var rr = ctx.Resolve(parameter) as LocalResolveResult;
						if (rr == null)
							continue;
						if (usageAnalysis.GetStatus(rr.Variable) != ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod.VariableState.None)
							return;
					}
				}

				AddIssue(new CodeIssue(anonymousMethodExpression.LParToken.StartLocation,
					anonymousMethodExpression.RParToken.EndLocation,
					ctx.TranslateString("Specifying signature is redundant because no parameter is used"),
					ctx.TranslateString("Remove redundant signature"),
					script => {
						int start = script.GetCurrentOffset(anonymousMethodExpression.DelegateToken.EndLocation);
						int end = script.GetCurrentOffset(anonymousMethodExpression.Body.StartLocation);

						script.Replace(start, end - start, " ");
					}) { IssueMarker = IssueMarker.GrayOut });
			}
		}
	}
}