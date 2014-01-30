//
// CanBeReplacedWithTryCastAndCheckForNullIssue.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
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
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Type check and casts can be replaced with 'as' and null check",
	                  Description="Type check and casts can be replaced with 'as' and null check",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "CanBeReplacedWithTryCastAndCheckForNull")]
	public class CanBeReplacedWithTryCastAndCheckForNullIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CanBeReplacedWithTryCastAndCheckForNullIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}
			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);

				IsExpression isExpression;
				int foundCastCount;
				if (UseAsAndNullCheckAction.ScanIfElse(ctx, ifElseStatement, out isExpression, out foundCastCount) == null)
					return;
				if (foundCastCount == 0)
					return;

				AddIssue(new CodeIssue(
					isExpression.IsToken,
					ctx.TranslateString("Type check and casts can be replaced with 'as' and null check")
				) { ActionProvider = { typeof(UseAsAndNullCheckAction) } } );
			}
		}
	}
}