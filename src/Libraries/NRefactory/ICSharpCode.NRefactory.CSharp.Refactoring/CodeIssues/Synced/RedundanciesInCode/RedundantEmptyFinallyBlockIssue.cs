//
// RedundantEmptyFinallyBlockIssue.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription(
		"Redundant empty finally block",
		Description = "Redundant empty finally block",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "RedundantEmptyFinallyBlock")]
	public class RedundantEmptyFinallyBlockIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantEmptyFinallyBlockIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			static bool IsEmpty (BlockStatement blockStatement)
			{
				return !blockStatement.Descendants.Any(s => s is Statement && !(s is EmptyStatement || s is BlockStatement));
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				base.VisitBlockStatement(blockStatement);
				if (blockStatement.Role != TryCatchStatement.FinallyBlockRole || !IsEmpty (blockStatement))
					return;
				var tryCatch = blockStatement.Parent as TryCatchStatement;
				if (tryCatch == null)
					return;
				AddIssue(new CodeIssue(
					tryCatch.FinallyToken.StartLocation,
					blockStatement.EndLocation,
					ctx.TranslateString("Redundant empty finally block"),
					ctx.TranslateString("Remove 'finally'"),
					s => {
						if (tryCatch.CatchClauses.Any()) {
							s.Remove(tryCatch.FinallyToken);
							s.Remove(blockStatement); 
							s.FormatText(tryCatch);
							return;
						}
						s.Remove(tryCatch.TryToken);
						s.Remove(tryCatch.TryBlock.LBraceToken);
						s.Remove(tryCatch.TryBlock.RBraceToken);
						s.Remove(tryCatch.FinallyToken);
						s.Remove(tryCatch.FinallyBlock); 
						s.FormatText(tryCatch.Parent);
					}
				) { IssueMarker = IssueMarker.GrayOut });
			}
		}
	}
}