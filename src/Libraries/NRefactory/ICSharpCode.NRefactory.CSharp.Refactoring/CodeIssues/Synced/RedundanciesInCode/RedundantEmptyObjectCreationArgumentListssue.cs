// 
// RedundantObjectCreationArgumentListIssue.cs
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
	[IssueDescription ("Redundant empty argument list on object creation expression",
	                   Description = "When object creation uses object or collection initializer, empty argument list is redundant.",
	                   Category = IssueCategories.RedundanciesInCode,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "RedundantEmptyObjectCreationArgumentList")]
	public class RedundantObjectCreationArgumentListIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantObjectCreationArgumentListIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
			{
				base.VisitObjectCreateExpression(objectCreateExpression);

				if (objectCreateExpression.Initializer.IsNull ||
					objectCreateExpression.Arguments.Count > 0 ||
					objectCreateExpression.LParToken.IsNull)
					return;

				AddIssue(new CodeIssue(objectCreateExpression.LParToken.StartLocation, objectCreateExpression.RParToken.EndLocation,
				         ctx.TranslateString("Empty argument list is redundant"),
				         ctx.TranslateString("Remove '()'"), script => {
					var l1 = objectCreateExpression.LParToken.GetPrevNode().EndLocation;
					var l2 = objectCreateExpression.RParToken.GetNextNode().StartLocation;
					var o1 = script.GetCurrentOffset(l1);
					var o2 = script.GetCurrentOffset(l2);

					script.Replace(o1, o2 - o1, " ");
					}) { IssueMarker = IssueMarker.GrayOut });
			}
		}
	}
}
