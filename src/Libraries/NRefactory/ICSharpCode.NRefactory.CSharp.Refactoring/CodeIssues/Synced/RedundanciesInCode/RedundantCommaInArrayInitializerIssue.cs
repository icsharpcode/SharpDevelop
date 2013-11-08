// 
// RedundantCommaInArrayInitializerIssue.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Redundant comma in array initializer",
						Description = "Redundant comma in array initializer.",
						Category = IssueCategories.RedundanciesInCode,
						Severity = Severity.Warning,
                        AnalysisDisableKeyword = "RedundantCommaInArrayInitializer")]
	public class RedundantCommaInArrayInitializerIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantCommaInArrayInitializerIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression)
			{
				base.VisitArrayInitializerExpression(arrayInitializerExpression);

				if (arrayInitializerExpression.IsSingleElement)
					return;

				var commaToken = arrayInitializerExpression.RBraceToken.PrevSibling as CSharpTokenNode;
				if (commaToken == null || commaToken.ToString() != ",")
					return;
				string issueDescription;
				if (arrayInitializerExpression.Parent is ObjectCreateExpression) {
					if (arrayInitializerExpression.Elements.FirstOrNullObject() is NamedExpression) {
						issueDescription = ctx.TranslateString("Redundant comma in object initializer");
					} else {
						issueDescription = ctx.TranslateString("Redundant comma in collection initializer");
					}
				} else {
					issueDescription = ctx.TranslateString("Redundant comma in array initializer");
				}
				AddIssue(new CodeIssue(commaToken,
				         issueDescription,
				         ctx.TranslateString("Remove ','"),
					script => script.Remove(commaToken)) { IssueMarker = IssueMarker.GrayOut });
			}
		}
	}
}
