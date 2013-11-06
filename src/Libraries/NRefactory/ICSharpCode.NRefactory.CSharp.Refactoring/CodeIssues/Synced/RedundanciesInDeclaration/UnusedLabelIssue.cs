//
// UnusedLabelIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription(
		"Unused label",
		Description = "Label is never referenced",
		Category = IssueCategories.RedundanciesInDeclarations,
		Severity = Severity.Warning,
		PragmaWarning = 164,
		AnalysisDisableKeyword = "UnusedLabel")]
	public class UnusedLabelIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<UnusedLabelIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			class LabelDescriptor 
			{
				public List<LabelStatement> LabelStatement = new List<ICSharpCode.NRefactory.CSharp.LabelStatement>();
				public bool IsUsed;

				public LabelDescriptor(LabelStatement labelStatement)
				{
					this.LabelStatement.Add(labelStatement);
				}
			}

			readonly Dictionary<string, LabelDescriptor> labels = new Dictionary<string, LabelDescriptor> ();

			void GatherLabels(BlockStatement body)
			{
				foreach (var node in body.Descendants) {
					var labelStatement = node as LabelStatement;
					if (labelStatement == null)
						continue;
					// note: duplicate labels are checked by the parser.
					LabelDescriptor desc;
					if (!labels.TryGetValue(labelStatement.Label, out desc)) {
						labels[labelStatement.Label] = new LabelDescriptor(labelStatement);
					} else {
						desc.LabelStatement.Add(labelStatement);
					}
				}
			}

			void CheckLables()
			{
				foreach (var label in labels.Values) {
					if (label.IsUsed)
						continue;
					foreach (var stmt in label.LabelStatement) {
						AddIssue(new CodeIssue(
							stmt.LabelToken.StartLocation,
							stmt.ColonToken.EndLocation,
							ctx.TranslateString("Label is unused"),
							ctx.TranslateString("Remove unused label"),
							s => { s.Remove(stmt); s.FormatText(stmt.Parent); }
						) { IssueMarker = IssueMarker.GrayOut });
					}
				}

				labels.Clear();
			}

			public override void VisitGotoStatement(GotoStatement gotoStatement)
			{
				LabelDescriptor desc;
				if (labels.TryGetValue(gotoStatement.Label, out desc)) {
					desc.IsUsed = true;
				}
			}

			public override void VisitLabelStatement(LabelStatement labelStatement)
			{
				if (IsSuppressed(labelStatement.StartLocation)) {
					LabelDescriptor desc;
					if (labels.TryGetValue(labelStatement.Label, out desc)) 
						desc.LabelStatement.Remove(labelStatement);
				}
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				GatherLabels(methodDeclaration.Body);
				base.VisitMethodDeclaration(methodDeclaration);
				CheckLables();
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				GatherLabels(constructorDeclaration.Body);
				base.VisitConstructorDeclaration(constructorDeclaration);
				CheckLables();
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				GatherLabels(destructorDeclaration.Body);
				base.VisitDestructorDeclaration(destructorDeclaration);
				CheckLables();
			}

			public override void VisitAccessor(Accessor accessor)
			{
				GatherLabels(accessor.Body);
				base.VisitAccessor(accessor);
				CheckLables();
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
				GatherLabels(operatorDeclaration.Body);
				base.VisitOperatorDeclaration(operatorDeclaration);
				CheckLables();
			}

		}
	}
}

