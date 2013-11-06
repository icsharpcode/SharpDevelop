// 
// RedundantInternalInspector.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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
using ICSharpCode.NRefactory.PatternMatching;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Finds redundant internal modifiers.
	/// </summary>
	[IssueDescription("Remove redundant 'internal' modifier",
	       Description="Removes 'internal' modifiers that are not required.", 
	       Category = IssueCategories.RedundanciesInCode,
	       Severity = Severity.Hint)]
	public class RedundantInternalIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this);
		}

		class GatherVisitor : GatherVisitorBase<RedundantInternalIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx, RedundantInternalIssue qualifierDirectiveEvidentIssueProvider) : base (ctx, qualifierDirectiveEvidentIssueProvider)
			{
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				foreach (var token_ in typeDeclaration.ModifierTokens) {
					var token = token_;
					if (token.Modifier == Modifiers.Internal) {
						AddIssue(new CodeIssue(token, ctx.TranslateString("Keyword 'internal' is redundant.  This is the default modifier."), ctx.TranslateString("Remove 'internal' modifier"), script => {
							int offset = script.GetCurrentOffset(token.StartLocation);
							int endOffset = script.GetCurrentOffset(token.GetNextNode().StartLocation);
							script.RemoveText(offset, endOffset - offset);
						}) { IssueMarker = IssueMarker.GrayOut });
					}
				}
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				// SKIP
			}
		}
	}
}

