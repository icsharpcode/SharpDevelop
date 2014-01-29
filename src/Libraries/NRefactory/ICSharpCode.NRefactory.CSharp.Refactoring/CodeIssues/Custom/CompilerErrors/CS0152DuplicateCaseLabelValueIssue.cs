//
// DuplicateCaseLabelValueIssue.cs
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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.PatternMatching;
using Mono.CSharp;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Duplicate case label value issue",
	                   Description = "A case label value is duplicate.",
	                   Category = IssueCategories.CompilerErrors,
	                   Severity = Severity.Error)]
	public class CS0152DuplicateCaseLabelValueIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CS0152DuplicateCaseLabelValueIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitSwitchSection(SwitchSection switchSection)
			{
				base.VisitSwitchSection(switchSection);

				var parent = switchSection.Parent as SwitchStatement;
				if (parent == null)
					return;

				foreach (var curLabel in switchSection.CaseLabels) {
					if (curLabel.Expression.IsNull)
						continue;
					var curValue = ctx.Resolve(curLabel.Expression).ConstantValue;
					if (curValue == null)
						continue;
					foreach (var sect in parent.SwitchSections) {
						if (sect.CaseLabels.Any(label => label != curLabel && Equals(curValue, ctx.Resolve(label.Expression).ConstantValue))) {
							AddIssue(new CodeIssue(curLabel, string.Format(ctx.TranslateString("Duplicate case label value '{0}'"), curValue)));
							break;
						}
					}
				}

			}
		}
	}
}

