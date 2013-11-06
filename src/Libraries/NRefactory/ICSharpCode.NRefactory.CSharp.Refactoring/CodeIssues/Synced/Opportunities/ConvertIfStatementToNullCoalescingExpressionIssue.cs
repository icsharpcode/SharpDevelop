//
// ConvertIfStatementToNullCoalescingExpressionIssue.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'if' statement can be re-written as '??' expression",
	                  Description="Convert 'if' to '??'",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Hint,
	                  AnalysisDisableKeyword = "ConvertIfStatementToNullCoalescingExpression")]
	public class ConvertIfStatementToNullCoalescingExpressionIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConvertIfStatementToNullCoalescingExpressionIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);
				Expression rightSide;
				var leftSide = ConvertIfStatementToNullCoalescingExpressionAction.CheckNode(ifElseStatement, out rightSide);
				if (leftSide == null)
					return;
				if (ConvertIfStatementToConditionalTernaryExpressionIssue.IsComplexExpression(leftSide) || 
				    ConvertIfStatementToConditionalTernaryExpressionIssue.IsComplexExpression(rightSide))
					return;
				var previousNode = ifElseStatement.GetPrevSibling(sibling => sibling is Statement) as VariableDeclarationStatement;
				if (previousNode == null || ConvertIfStatementToConditionalTernaryExpressionIssue.IsComplexExpression(previousNode))
					return;

				AddIssue(new CodeIssue(
					ifElseStatement.IfToken,
					ctx.TranslateString("Convert to '??' expresssion")
				){ IssueMarker = IssueMarker.DottedLine, ActionProvider = { typeof(ConvertIfStatementToNullCoalescingExpressionAction) } });
			}
		}
	}
}

