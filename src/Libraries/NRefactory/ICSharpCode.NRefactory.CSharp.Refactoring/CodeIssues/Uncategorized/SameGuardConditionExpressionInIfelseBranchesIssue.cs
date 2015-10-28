//
// Author:
//       Ji Kun <jikun.nus@gmail.com>
//
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
//	[IssueDescription("Same guard condition expression in different if else branch",
//	                  Description = "A warning should be given for the case: if (condition) {…} else if (condition) {…}.",
//	                  Category = IssueCategories.Notifications,
//	                  Severity = Severity.Warning)]
	public class SameGuardConditionExpressionInIfelseBranchesIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<SameGuardConditionExpressionInIfelseBranchesIssue>
		{
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			bool IsSafeExpression(Expression expression, BaseRefactoringContext context)
			{
				var components = expression.DescendantsAndSelf;
				foreach (var c in components) {
					if (c is AssignmentExpression)
						return false;
					else if (c is UnaryOperatorExpression) {
						var ope = ((UnaryOperatorExpression)c).Operator;
						if (ope == UnaryOperatorType.Decrement || ope == UnaryOperatorType.Increment 
							|| ope == UnaryOperatorType.PostDecrement || ope == UnaryOperatorType.PostIncrement)
							return false;
					} else if (c is IdentifierExpression) {
						var result = context.Resolve(c);
						if (result.IsError)
							return false;
						if (!(result is LocalResolveResult))
							return false;
						if ((((LocalResolveResult)result).IsParameter))
							return false;
					}
				}
				return true;
			}

			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);
				
				HashSet<string> conditions = new HashSet<string>();
				conditions.Add(ifElseStatement.Condition.ToString());
				var temp = ifElseStatement.FalseStatement;
				AstNode redundantCondition = null;
				while (temp is IfElseStatement) {
					var tempCondition = ((IfElseStatement)temp).Condition;
					if (conditions.Contains(tempCondition.ToString())) {
						if (IsSafeExpression(tempCondition, ctx)) {
							redundantCondition = tempCondition;
							break;
						}
					}
					conditions.Add(tempCondition.ToString());
					temp = ((IfElseStatement)temp).FalseStatement;
				}
				
				if (redundantCondition == null)
					return;
				
				AddIssue(new CodeIssue(redundantCondition, ctx.TranslateString("Found duplicate condition")));
			}
		}
	}
}