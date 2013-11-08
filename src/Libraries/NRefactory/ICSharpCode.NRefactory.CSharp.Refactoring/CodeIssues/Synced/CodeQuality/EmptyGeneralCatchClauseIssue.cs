//
// EmptyGeneralCatchClauseIssue.cs
//
// Author:
//       Ji Kun <jikun.nus@gmail.com>
//
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// A catch clause that catches System.Exception and has an empty body
	/// </summary>
	[IssueDescription("Empty general catch clause",
	                  Description= "A catch clause that catches System.Exception and has an empty body",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "EmptyGeneralCatchClause")]
	public class EmptyGeneralCatchClauseIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this);
		}
		
		class GatherVisitor : GatherVisitorBase<EmptyGeneralCatchClauseIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx, EmptyGeneralCatchClauseIssue issueProvider) : base (ctx, issueProvider)
			{
			}
			
			public override void VisitCatchClause(CatchClause catchClause)
			{
				base.VisitCatchClause(catchClause);

				AstType type = catchClause.Type;
				if (!type.IsNull) {
					var resolvedType = ctx.Resolve(type);

					if (resolvedType.IsError ||
						!resolvedType.Type.Namespace.Equals("System") || !resolvedType.Type.Name.Equals("Exception"))
						return;
				}

				var body = catchClause.Body;
				if (body.Statements.Any())
					return;

				AddIssue(new CodeIssue(catchClause.CatchToken, ctx.TranslateString("Empty general catch clause suppresses any error")));
			}
		}
	}
}

