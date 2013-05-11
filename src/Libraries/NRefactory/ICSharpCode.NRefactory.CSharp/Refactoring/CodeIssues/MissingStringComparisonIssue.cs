//
// MissingStringComparisonIssue.cs
//
// Author:
//       Daniel Grunwald <daniel@danielgrunwald.de>
//
// Copyright (c) 2012 Daniel Grunwald
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Missing StringComparison argument",
	                  Description = "Warns when a culture-aware comparison is used by default.",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning)]
	public class MissingStringComparisonIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}
		
		class GatherVisitor : GatherVisitorBase<MissingStringComparisonIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base(ctx)
			{
			}
			
			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				
				MemberReferenceExpression mre = invocationExpression.Target as MemberReferenceExpression;
				if (mre == null)
					return;
				switch (mre.MemberName) {
					case "StartsWith":
					case "EndsWith":
						if (invocationExpression.Arguments.Count != 1)
							return;
						break;
					case "IndexOf":
					case "LastIndexOf":
						break;
					default:
						return;
				}
				
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
				AddIssue(invocationExpression.LParToken.StartLocation, invocationExpression.RParToken.EndLocation,
				         mre.MemberName + "() call is missing StringComparison argument",
				         new [] {
							new CodeAction("Use ordinal comparison", script => AddArgument(script, invocationExpression, "Ordinal"), invocationExpression),
							new CodeAction("Use culture-aware comparison", script => AddArgument(script, invocationExpression, "CurrentCulture"), invocationExpression)
				         });
			}
			
			void AddArgument(Script script, InvocationExpression invocationExpression, string stringComparison)
			{
				var astBuilder = ctx.CreateTypeSytemAstBuilder(invocationExpression);
				var newArgument = astBuilder.ConvertType(new TopLevelTypeName("System", "StringComparison")).Member(stringComparison);
				var copy = (InvocationExpression)invocationExpression.Clone();
				copy.Arguments.Add(newArgument);
				script.Replace(invocationExpression, copy);
			}
		}
	}
}
