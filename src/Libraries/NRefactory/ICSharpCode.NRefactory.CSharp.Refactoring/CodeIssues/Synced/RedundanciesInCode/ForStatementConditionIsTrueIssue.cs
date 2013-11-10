// 
// ForStatementConditionIsTrueIssue.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("'true' is redundant as for statement condition",
	                   Description = "true is redundant as for statement condition, thus can be safely ommited",
	                   Category = IssueCategories.RedundanciesInCode,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "ForStatementConditionIsTrue")]
	public class ForStatementConditionIsTrueIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<ForStatementConditionIsTrueIssue>
		{
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			static readonly AstNode pattern =  new PrimitiveExpression(true);
		
			public override void VisitForStatement (ForStatement forstatement)
			{
				base.VisitForStatement(forstatement);

				var m = pattern.Match(forstatement.Condition);
				if (m.Success) {
					AddIssue(new CodeIssue(
						forstatement.Condition, 
						ctx.TranslateString("'true' is redundant as for statement condition"), 
						ctx.TranslateString("Remove 'true'"),
						script => script.Remove(forstatement.Condition)
					) { IssueMarker = IssueMarker.GrayOut });
				}
			}
		}
	}
}
