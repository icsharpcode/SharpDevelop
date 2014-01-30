// 
// RedundantCaseLabelIssue.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Redundant 'case' label",
						Description = "'case' label is redundant.",
						Category = IssueCategories.RedundanciesInCode,
						Severity = Severity.Warning,
                        AnalysisDisableKeyword = "RedundantCaseLabel")]
	public class RedundantCaseLabelIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantCaseLabelIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitSwitchSection (SwitchSection switchSection)
			{
				base.VisitSwitchSection (switchSection);

				if (switchSection.CaseLabels.Count < 2)
					return;

				if (!switchSection.CaseLabels.Any(label => label.Expression.IsNull))
					return;

				foreach (var caseLabel in switchSection.CaseLabels) {
					if (caseLabel.Expression.IsNull)
						continue;
					AddIssue(new CodeIssue(
						caseLabel,
						ctx.TranslateString("Redundant case label"),
						string.Format(ctx.TranslateString("Remove 'case {0}'"), caseLabel.Expression),
						scipt => {
							scipt.Remove(caseLabel);
						}
					) { IssueMarker = IssueMarker.GrayOut });
				}
			}
		}
	}
}
