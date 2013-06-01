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
using ICSharpCode.NRefactory.Refactoring;

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

		class GatherVisitor : GatherVisitorBase<ExpressionOfCompatibleTypeCastIssue>
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
				if (assignmentExpression.Operator != AssignmentOperatorType.Assign)
					return;
				var variableType = ctx.Resolve(assignmentExpression.Left).Type;
				VisitTypeCastExpression(variableType, assignmentExpression.Right);
			}
			
			public override void VisitVariableInitializer(VariableInitializer variableInitializer)
			{
				base.VisitVariableInitializer(variableInitializer);
				if (!variableInitializer.Initializer.IsNull) {
					var variableType = ctx.Resolve(variableInitializer).Type;
					VisitTypeCastExpression(variableType, variableInitializer.Initializer);
				}
			}
			
			private AstType CreateShortType(AstNode node, IType fullType)
			{
				var builder = ctx.CreateTypeSytemAstBuilder(node);
				return builder.ConvertType(fullType);
			}

			void VisitTypeCastExpression(IType variableType, Expression expression)
			{
				if (variableType.Kind == TypeKind.Unknown)
					return; // ignore error if the variable type is unknown
				if (ctx.GetConversion(expression).IsValid)
					return; // don't complain if the code is valid
				
				var rr = ctx.Resolve(expression);
				if (rr.Type.Kind == TypeKind.Unknown)
					return; // ignore error if expression type is unknown
				var foundConversion = conversion.ExplicitConversion(rr, variableType);
				if (!foundConversion.IsValid)
					return; // there's an error, but we can't fix it by introducing a cast
				
				AddIssue(expression, string.Format(ctx.TranslateString("Cast to '{0}'"), variableType.Name),
				         script => {
				         	var right = expression.Clone();
				         	var castRight = right.CastTo(CreateShortType(expression, variableType));
				         	script.Replace(expression, castRight);
				         });
			}
		}
	}
}