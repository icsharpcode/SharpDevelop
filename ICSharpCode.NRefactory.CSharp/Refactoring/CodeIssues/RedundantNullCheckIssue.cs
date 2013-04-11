//
// RedundantNullCheckIssue.cs
//
// Author:
//	   Ji Kun <jikun.nus@gmail.com>
//
// Copyright (c) 2013 Ji Kun
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
// THE SOFTWARE.using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant null check",
					  Description = "When 'is' keyword is used, which implicitly check null.",
					  Category = IssueCategories.Redundancies,
					  Severity = Severity.Suggestion,
	                  ResharperDisableKeyword = "RedundantNullCheck",
					  IssueMarker = IssueMarker.GrayOut)]
	public class RedundantNullCheckIssue : ICodeIssueProvider
	{
		private static readonly Pattern pattern1
			= new Choice {
			//  x is Record && x!= null	 
			new BinaryOperatorExpression(
				new IsExpression
					{
						Expression = new AnyNode("a"),
						Type = new AnyNode("t")
					}, BinaryOperatorType.ConditionalAnd,
				PatternHelper.CommutativeOperator(new Backreference("a"),
													BinaryOperatorType.
														InEquality,
													new NullReferenceExpression
														())
				)
		};
		private static readonly Pattern pattern2
		   = new Choice {
			//  x != null && x is Record
			new BinaryOperatorExpression (
				PatternHelper.CommutativeOperator (new AnyNode("a"), 
														BinaryOperatorType.InEquality, 
													new NullReferenceExpression())			
				, BinaryOperatorType.ConditionalAnd,
				new IsExpression {
					Expression = new Backreference("a"),
					Type = new AnyNode("t")
				}
			)	
		};

		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase<RedundantNullCheckIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);
				Match m1 = pattern1.Match(binaryOperatorExpression);
				if (m1.Success) {
					AddIssue(binaryOperatorExpression, ctx.TranslateString("Remove redundant IsNULL check"), script => {
						Expression expr = binaryOperatorExpression.Left;
						script.Replace(binaryOperatorExpression, expr);
					});
					return;
				}

				Match m2 = pattern2.Match(binaryOperatorExpression);
				if (m2.Success) {
					AddIssue(binaryOperatorExpression, ctx.TranslateString("Remove redundant IsNULL check"), script => {
						Expression expr = binaryOperatorExpression.Right;
						script.Replace(binaryOperatorExpression, expr);
					});
					return;
				}
			}
		}
	}
}

