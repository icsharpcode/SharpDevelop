//
// DuplicateExpressionsInConditionsIssue.cs
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
//    [IssueDescription("Expression has some redundant items",
//		                   Description = "Expression has some redundant items",
//		                   Category = IssueCategories.CodeQualityIssues,
//		                   Severity = Severity.Warning,
//		                   AnalysisDisableKeyword = "ConditionalTernaryEqualBranch")]
	public class DuplicateExpressionsInConditionsIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

        static readonly List<BinaryOperatorType> SupportedOperators = new List<BinaryOperatorType>();
        static DuplicateExpressionsInConditionsIssue()
        {
            SupportedOperators.Add(BinaryOperatorType.BitwiseAnd);
            SupportedOperators.Add(BinaryOperatorType.BitwiseOr);
            SupportedOperators.Add(BinaryOperatorType.ConditionalAnd);
            SupportedOperators.Add(BinaryOperatorType.ConditionalOr);
        }
		
		class GatherVisitor : GatherVisitorBase<DuplicateExpressionsInConditionsIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

            public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
            {
                var expression = binaryOperatorExpression;
                base.VisitBinaryOperatorExpression(expression);
                if(!SupportedOperators.Contains(expression.Operator) )
                    return;
                var parentExpression = expression.Parent as BinaryOperatorExpression;
                if(parentExpression!=null && parentExpression.Operator==expression.Operator)
                {
                    //handle only parent sequence
                    return;
                }
                var expressions = GetExpressions(binaryOperatorExpression, expression);
                for(var i=0;i<expressions.Count-1;i++)
                {
                    for (var j = i+1; j < expressions.Count; j++)
                    {
                        var expressionLeft = expressions[i];
                        var expressionRight = expressions[j];
                        if (!expressionLeft.IsMatch(expressionRight))
                            continue;
                        var action = new CodeAction(ctx.TranslateString("Remove redundant expression"),
                                                    script => RemoveRedundantExpression(script, expressionRight),
                                                    expressionRight);
                    
						AddIssue(
							new CodeIssue(expressionRight, 
								ctx.TranslateString(string.Format("The expression '{0}' is identical in the left branch", expressionRight)), 
								action
							) { IssueMarker = IssueMarker.GrayOut }
						);


                    }
                }
            }

		    private static void RemoveRedundantExpression(Script script, AstNode expressionRight)
		    {
                var parent = expressionRight.Parent as BinaryOperatorExpression;
                if(parent==null) //should never happen!
                    return;
                script.Replace(parent, parent.Left.Clone());
		    }

		    private static List<Expression> GetExpressions(BinaryOperatorExpression binaryOperatorExpression,
		                                       BinaryOperatorExpression expression)
		    {
		        var baseExpression = expression;
		        var leftExpression = baseExpression.FirstChild as BinaryOperatorExpression;
		        var expressions = new List<Expression>();
		        while (leftExpression != null && binaryOperatorExpression.Operator == leftExpression.Operator)
		        {
		            expressions.Add(baseExpression.Right);
		            baseExpression = leftExpression;
		            leftExpression = leftExpression.Left as BinaryOperatorExpression;
		        }
		        expressions.Add(baseExpression.Right);
		        expressions.Add(baseExpression.Left);
		        expressions.Reverse();
		        return expressions;
		    }
		}
	}	
}
