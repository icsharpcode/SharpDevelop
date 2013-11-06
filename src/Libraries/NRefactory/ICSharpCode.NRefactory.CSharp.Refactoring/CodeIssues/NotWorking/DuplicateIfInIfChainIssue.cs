//
// DuplicateIfInIfChainIssue.cs
//
// Author:
//       Ciprian Khlud <ciprian.mustiata@yahoo.com>
//
// Copyright (c) 2013 Ciprian Khlud
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
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
//	[IssueDescription("Else branch has some redundant if",
//	                  Description = "One Else-if was checked before so is not be true",
//	                  Category = IssueCategories.CodeQualityIssues,
//	                  Severity = Severity.Warning,
//	                  AnalysisDisableKeyword = "ConditionalTernaryEqualBranch")]
	public class DuplicateIfInIfChainIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<DuplicateIfInIfChainIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx)
				: base (ctx)
			{
			}
			
			public override void VisitIfElseStatement(IfElseStatement ifStatement)
			{
				base.VisitIfElseStatement(ifStatement);
				var parentExpression = ifStatement.Parent as IfElseStatement;
				//handle only parent sequence
				if (parentExpression != null)
					return;
				var expressions = GetExpressions(ifStatement);
				for(var i=0;i<expressions.Count-1;i++) {
					for (var j = i+1; j < expressions.Count; j++) {
						var leftCondition = expressions[i].Condition;
                        var rightIf = expressions[j];
                        var rightCondition = rightIf.Condition;
					    
						if (!leftCondition.IsMatch(rightCondition))
							continue;
						var action = new CodeAction(ctx.TranslateString("Remove redundant expression"),
                                                    script => RemoveRedundantIf(script, rightIf),
						                            rightCondition);
						
						AddIssue(new CodeIssue(rightCondition,
						         ctx.TranslateString(string.Format("The expression '{0}' is identical in the left branch",
								rightCondition)), action) { IssueMarker = IssueMarker.GrayOut });
						
					}
				}
			}

            private static void RemoveRedundantIf(Script script, IfElseStatement expressionRight)
            {
                var parent = expressionRight.Parent as IfElseStatement;
                if(parent==null) { //should never happen!
                    return;
                }
                if (expressionRight.FalseStatement.IsNull) {
                    script.Remove(parent.ElseToken);
                    script.Remove(parent.FalseStatement);
                    script.FormatText(parent);
				} else {
					script.Replace(parent.FalseStatement, expressionRight.FalseStatement.Clone());
				}
            }

		    private static List<IfElseStatement> GetExpressions(IfElseStatement expression)
			{
				var baseExpression = expression;
				var falseExpression = baseExpression.FalseStatement as IfElseStatement;
                var expressions = new List<IfElseStatement>();
				while (falseExpression != null) {
					expressions.Add(baseExpression);
					baseExpression = falseExpression;
					falseExpression = falseExpression.FalseStatement as IfElseStatement;
				}
				expressions.Add(baseExpression);
				return expressions;
			}
		}
	}
	
}
