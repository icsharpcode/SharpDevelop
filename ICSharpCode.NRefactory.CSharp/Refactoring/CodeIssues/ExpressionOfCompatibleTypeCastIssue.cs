// 
// ExpressionOfCompatibleTypeCastIssue.cs
// 
// Author:
//      Ciprian Khlud <ciprian.mustiata@yahoo.com>
// 
// Copyright (c) 2013 Ciprian Khlud <ciprian.mustiata@yahoo.com>
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Type cast expression of compatible type",
		Description = "Type cast expression of compatible type",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		IssueMarker = IssueMarker.Underline)]
	public class ExpressionOfCompatibleTypeCastIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase<CastExpressionOfIncompatibleTypeIssue>
		{
			readonly CSharpConversions conversion;

			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
				conversion = new CSharpConversions(ctx.Compilation);
			}

			public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
			{
				base.VisitAssignmentExpression(assignmentExpression);

				VisitTypeCastExpression(assignmentExpression, ctx.Resolve(assignmentExpression.Left).Type,
				                        ctx.Resolve(assignmentExpression.Right).Type);
			}

			private AstType CreateShortType(AstNode node, IType fullType)
			{
				var builder = ctx.CreateTypeSytemAstBuilder(node);
				return builder.ConvertType(fullType);
			}

			void VisitTypeCastExpression(AssignmentExpression expression, IType exprType, IType castToType)
			{
				if (exprType.Kind == TypeKind.Unknown || castToType.Kind == TypeKind.Unknown)
					return;
				var foundConversion = conversion.ExplicitConversion(exprType, castToType);
				if (foundConversion == Conversion.None)
					return;
				if (!foundConversion.IsExplicit)
					return;

				AddIssue(expression, string.Format(ctx.TranslateString("Cast to '{0}'"), castToType.Name),
				         script => {
					var right = expression.Right.Clone();
					var castRight = right.CastTo(CreateShortType(expression, castToType));
					script.Replace(expression.Right, castRight);
				});
			}
		}
	}
}