//
// RedundantExplicitArrayCreationIssue.cs
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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription(
		"Redundant explicit type in array creation",
		Description = "Redundant explicit type in array creation",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "RedundantExplicitArrayCreation")]
	public class RedundantExplicitArrayCreationIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantExplicitArrayCreationIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitSyntaxTree(SyntaxTree syntaxTree)
			{
				if (!ctx.Supports(new Version(3, 0)))
					return;
				base.VisitSyntaxTree(syntaxTree);
			}

			public override void VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression)
			{
				base.VisitArrayCreateExpression(arrayCreateExpression);
				if (arrayCreateExpression.Arguments.Count != 0)
					return;
				var arrayType = arrayCreateExpression.Type;
				if (arrayType.IsNull)
					return;
				var arrayTypeRR = ctx.Resolve(arrayType);
				if (arrayTypeRR.IsError)
					return;

				IType elementType = null;
				foreach (var element in arrayCreateExpression.Initializer.Elements) {
					var elementTypeRR = ctx.Resolve(element);
					if (elementTypeRR.IsError)
						return;
					if (elementType == null) {
						elementType = elementTypeRR.Type;
						continue;
					}
					if (elementType != elementTypeRR.Type)
						return;
				}
				if (elementType != arrayTypeRR.Type)
					return;

				AddIssue(
					new CodeIssue (
						arrayType,
						ctx.TranslateString("Redundant explicit array type specification"),
						ctx.TranslateString("Remove redundant type specification"),
						s => s.Remove(arrayType) 
					) { IssueMarker = IssueMarker.GrayOut }
				);
			}
		}
	}
}

