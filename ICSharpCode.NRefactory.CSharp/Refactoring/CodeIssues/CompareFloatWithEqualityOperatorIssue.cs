// 
// CompareFloatWithEqualityOperatorIssue.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Compare floating point numbers with equality operator",
					   Description = "Comparison of floating point numbers with equality operator.",
					   Category = IssueCategories.CodeQualityIssues,
					   Severity = Severity.Warning,
					   IssueMarker = IssueMarker.Underline)]
	public class CompareFloatWithEqualityOperatorIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues (BaseRefactoringContext context)
		{
			return new GatherVisitor (context).GetIssues ();
		}

		class GatherVisitor : GatherVisitorBase
		{
			public GatherVisitor (BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			static bool IsFloatingPointType (IType type)
			{
				var typeDef = type.GetDefinition ();
				return typeDef != null &&
					(typeDef.KnownTypeCode == KnownTypeCode.Single || typeDef.KnownTypeCode == KnownTypeCode.Double);
			}

			bool IsFloatingPoint(AstNode node)
			{
				return IsFloatingPointType (ctx.Resolve (node).Type);
			}

			bool IsNaN (AstNode node)
			{
				var memberReferenceExpr = node as MemberReferenceExpression;
				if (memberReferenceExpr == null)
					return false;
				var typeReferenceExpr = memberReferenceExpr.Target as TypeReferenceExpression;
				if (typeReferenceExpr == null)
					return false;
				return IsFloatingPointType (ctx.ResolveType (typeReferenceExpr.Type)) && 
					memberReferenceExpr.MemberName == "NaN";
			}

			void AddIsNaNIssue(BinaryOperatorExpression binaryOperatorExpr, Expression argExpr)
			{
				AddIssue (binaryOperatorExpr, ctx.TranslateString ("Use double.IsNan()"), script => {
					Expression expr = new InvocationExpression (new MemberReferenceExpression (
						new TypeReferenceExpression (new PrimitiveType ("double")), "IsNaN"), argExpr.Clone ());
					if (binaryOperatorExpr.Operator == BinaryOperatorType.InEquality)
						expr = new UnaryOperatorExpression (UnaryOperatorType.Not, expr);
					script.Replace (binaryOperatorExpr, expr);
				});
			}

			public override void VisitBinaryOperatorExpression (BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression (binaryOperatorExpression);

				if (binaryOperatorExpression.Operator != BinaryOperatorType.Equality &&
					binaryOperatorExpression.Operator != BinaryOperatorType.InEquality)
					return;

				if (IsNaN(binaryOperatorExpression.Left)) {
					AddIsNaNIssue (binaryOperatorExpression, binaryOperatorExpression.Right);
				} else if (IsNaN (binaryOperatorExpression.Right)) {
					AddIsNaNIssue (binaryOperatorExpression, binaryOperatorExpression.Left);
				} else if (IsFloatingPoint(binaryOperatorExpression.Left) || IsFloatingPoint(binaryOperatorExpression.Right)) {
					AddIssue (binaryOperatorExpression, ctx.TranslateString ("Compare a difference with EPSILON"),
						script =>
						{
							// Math.Abs(diff) op EPSILON
							var diff = new BinaryOperatorExpression (binaryOperatorExpression.Left.Clone (),
								BinaryOperatorType.Subtract, binaryOperatorExpression.Right.Clone ());
							var abs = new InvocationExpression (
								new MemberReferenceExpression (
									new TypeReferenceExpression (new SimpleType("System.Math")), 
									"Abs"),
								diff);
							var op = binaryOperatorExpression.Operator == BinaryOperatorType.Equality ?
								BinaryOperatorType.LessThan : BinaryOperatorType.GreaterThan;
							var epsilon = new IdentifierExpression ("EPSILON");
							var compare = new BinaryOperatorExpression (abs, op, epsilon);
							script.Replace (binaryOperatorExpression, compare);
							script.Link (epsilon);
						});
				}
			}
		}
	}
}
