//
// ReplaceWithIsOperatorIssue.cs
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

using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Operator 'is' can be used",
		Description = "Operator Is can be used instead of comparing object GetType() and instances of System.Type object.",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "OperatorIsCanBeUsed")]
	public class OperatorIsCanBeUsedIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		sealed class GatherVisitor : GatherVisitorBase<OperatorIsCanBeUsedIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base(ctx)
			{
			}

			static readonly AstNode pattern = 
				PatternHelper.CommutativeOperatorWithOptionalParentheses(
					new InvocationExpression(new MemberReferenceExpression(new AnyNode("a"), "GetType"), null),
					BinaryOperatorType.Equality,
					new TypeOfExpression(new AnyNode("b"))
				);

			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);

				var m = pattern.Match(binaryOperatorExpression);
				if (!m.Success)
					return;

				Expression identifier = m.Get<Expression>("a").Single();
				AstType type = m.Get<AstType>("b").Single();
				
				var typeResolved = ctx.Resolve(type) as TypeResolveResult;
				if (typeResolved == null)
					return;

				if (typeResolved.Type.Kind == TypeKind.Class) {
					if (!typeResolved.Type.GetDefinition().IsSealed) {
						return;
					}
				}

				AddIssue(new CodeIssue(
					binaryOperatorExpression, 
					ctx.TranslateString("Operator 'is' can be used"), 
					ctx.TranslateString("Replace with 'is' operator"), 
					script => {
						var isExpr = new IsExpression(identifier.Clone(), type.Clone());
						script.Replace(binaryOperatorExpression, isExpr);
					}
				));
			}
		}
	}
}

