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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Compare floating point numbers with equality operator",
					   Description = "Comparison of floating point numbers with equality operator.",
					   Category = IssueCategories.CodeQualityIssues,
					   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "CompareOfFloatsByEqualityOperator")]
	public class CompareOfFloatsByEqualityOperatorIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CompareOfFloatsByEqualityOperatorIssue>
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

			bool IsConstantInfinity(AstNode node)
			{
				ResolveResult rr = ctx.Resolve(node);
				if (!rr.IsCompileTimeConstant)
					return false;
				if (rr.ConstantValue is float)
					return float.IsInfinity((float)rr.ConstantValue);
				if (rr.ConstantValue is double)
					return double.IsInfinity((double)rr.ConstantValue);
				return false;
			}

			string GetFloatType(Expression left, Expression right)
			{
				var rr = ctx.Resolve (left);
				if (rr.IsCompileTimeConstant)
					return rr.ConstantValue is float ? "float" : "double";
				
				rr = ctx.Resolve (right);
				if (rr.IsCompileTimeConstant)
					return rr.ConstantValue is float ? "float" : "double";
				return "double";
			}

			bool IsNaN (AstNode node)
			{
				var rr = ctx.Resolve (node);
				if (!rr.IsCompileTimeConstant)
					return false;

				return rr.ConstantValue is double && double.IsNaN((double)rr.ConstantValue) || 
					   rr.ConstantValue is float && float.IsNaN((float)rr.ConstantValue);
			}

			static bool IsZero (AstNode node)
			{
				var pe = node as PrimitiveExpression;
				if (pe == null)
					return false;
				if (pe.Value is char || pe.Value is string || pe.Value is bool)
					return false;

				if (pe.Value is double && (double)pe.Value == 0d || 
					pe.Value is float && (float)pe.Value == 0f || 
					pe.Value is decimal && (decimal)pe.Value == 0m)
					return true;

				return !pe.LiteralValue.Any(c => char.IsDigit(c) && c != '0');
			}

			bool IsNegativeInfinity (AstNode node)
			{
				var rr = ctx.Resolve (node);
				if (!rr.IsCompileTimeConstant)
					return false;
				return rr.ConstantValue is double && double.IsNegativeInfinity((double)rr.ConstantValue) ||
					   rr.ConstantValue is float && float.IsNegativeInfinity((float)rr.ConstantValue);
			}

			bool IsPositiveInfinity (AstNode node)
			{
				var rr = ctx.Resolve (node);
				if (!rr.IsCompileTimeConstant)
					return false;

				return rr.ConstantValue is double && double.IsPositiveInfinity ((double)rr.ConstantValue) ||
					rr.ConstantValue is float && float.IsPositiveInfinity((float)rr.ConstantValue);
			}

			void AddIsNaNIssue(BinaryOperatorExpression binaryOperatorExpression, Expression argExpr, string floatType)
			{
				if (!ctx.Resolve(argExpr).Type.IsKnownType(KnownTypeCode.Single))
					floatType = "double";
				AddIssue(new CodeIssue(
					binaryOperatorExpression, 
					ctx.TranslateString ("NaN doesn't equal to any floating point number including to itself. Use 'IsNaN' instead."),
					binaryOperatorExpression.Operator == BinaryOperatorType.InEquality ? 
						string.Format(ctx.TranslateString ("Replace with '!{0}.IsNaN(...)' call"), floatType) :
						string.Format(ctx.TranslateString ("Replace with '{0}.IsNaN(...)' call"), floatType),
					script => {
						Expression expr = new PrimitiveType(floatType).Invoke("IsNaN", argExpr.Clone());
						if (binaryOperatorExpression.Operator == BinaryOperatorType.InEquality)
							expr = new UnaryOperatorExpression (UnaryOperatorType.Not, expr);
						script.Replace (binaryOperatorExpression, expr);
					}
				));
			}

			void AddIsNegativeInfinityIssue(BinaryOperatorExpression binaryOperatorExpression, Expression argExpr, string floatType)
			{
				if (!ctx.Resolve(argExpr).Type.IsKnownType(KnownTypeCode.Single))
					floatType = "double";
				AddIssue(new CodeIssue(
					binaryOperatorExpression, 
					ctx.TranslateString ("Comparison of floating point numbers with equality operator. Use 'IsNegativeInfinity' method."),
					binaryOperatorExpression.Operator == BinaryOperatorType.InEquality ? 
					string.Format(ctx.TranslateString ("Replace with '!{0}.IsNegativeInfinity(...)' call"), floatType) :
					string.Format(ctx.TranslateString ("Replace with '{0}.IsNegativeInfinity(...)' call"), floatType),
					script => {
						Expression expr = new PrimitiveType(floatType).Invoke("IsNegativeInfinity", argExpr.Clone());
						if (binaryOperatorExpression.Operator == BinaryOperatorType.InEquality)
							expr = new UnaryOperatorExpression (UnaryOperatorType.Not, expr);
						script.Replace (binaryOperatorExpression, expr);
					}
				));
			}

			void AddIsPositiveInfinityIssue(BinaryOperatorExpression binaryOperatorExpression, Expression argExpr, string floatType)
			{
				if (!ctx.Resolve(argExpr).Type.IsKnownType(KnownTypeCode.Single))
					floatType = "double";
				AddIssue(new CodeIssue(
					binaryOperatorExpression, 
					ctx.TranslateString ("Comparison of floating point numbers with equality operator. Use 'IsPositiveInfinity' method."),
					binaryOperatorExpression.Operator == BinaryOperatorType.InEquality ? 
					string.Format(ctx.TranslateString ("Replace with '!{0}.IsPositiveInfinity(...)' call"), floatType) :
					string.Format(ctx.TranslateString ("Replace with '{0}.IsPositiveInfinity(...)' call"), floatType),
					script => {
						Expression expr = new PrimitiveType(floatType).Invoke("IsPositiveInfinity", argExpr.Clone());
						if (binaryOperatorExpression.Operator == BinaryOperatorType.InEquality)
							expr = new UnaryOperatorExpression (UnaryOperatorType.Not, expr);
						script.Replace (binaryOperatorExpression, expr);
					}
				));
			}

			void AddIsZeroIssue(BinaryOperatorExpression binaryOperatorExpression, Expression argExpr)
			{
				AddIssue(new CodeIssue(
					binaryOperatorExpression, 
					ctx.TranslateString ("Comparison of floating point numbers can be unequal due to the differing precision of the two values."),
					ctx.TranslateString ("Fix floating point number comparing. Compare a difference with epsilon."),
					script => {
						var builder = ctx.CreateTypeSystemAstBuilder(binaryOperatorExpression);
					var diff = argExpr.Clone();
						var abs = builder.ConvertType(new TopLevelTypeName("System", "Math")).Invoke("Abs", diff);
						var op = binaryOperatorExpression.Operator == BinaryOperatorType.Equality ? BinaryOperatorType.LessThan : BinaryOperatorType.GreaterThan;
						var epsilon = new IdentifierExpression ("EPSILON");
						var compare = new BinaryOperatorExpression (abs, op, epsilon);
						script.Replace (binaryOperatorExpression, compare);
						script.Select (epsilon);
					}
				));
			}

			public override void VisitBinaryOperatorExpression (BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression (binaryOperatorExpression);

				if (binaryOperatorExpression.Operator != BinaryOperatorType.Equality &&
					binaryOperatorExpression.Operator != BinaryOperatorType.InEquality)
					return;

				string floatType = GetFloatType (binaryOperatorExpression.Left, binaryOperatorExpression.Right);


				if (IsNaN(binaryOperatorExpression.Left)) {
					AddIsNaNIssue (binaryOperatorExpression, binaryOperatorExpression.Right, floatType);
				} else if (IsNaN (binaryOperatorExpression.Right)) {
					AddIsNaNIssue (binaryOperatorExpression, binaryOperatorExpression.Left, floatType);
				} else if (IsPositiveInfinity(binaryOperatorExpression.Left)) {
					AddIsPositiveInfinityIssue (binaryOperatorExpression, binaryOperatorExpression.Right, floatType);
				} else if (IsPositiveInfinity (binaryOperatorExpression.Right)) {
					AddIsPositiveInfinityIssue (binaryOperatorExpression, binaryOperatorExpression.Left, floatType);
				} else if (IsNegativeInfinity (binaryOperatorExpression.Left)) {
					AddIsNegativeInfinityIssue (binaryOperatorExpression, binaryOperatorExpression.Right, floatType);
				} else if (IsNegativeInfinity (binaryOperatorExpression.Right)) {
					AddIsNegativeInfinityIssue (binaryOperatorExpression, binaryOperatorExpression.Left, floatType);
				} else if (IsFloatingPoint(binaryOperatorExpression.Left) || IsFloatingPoint(binaryOperatorExpression.Right)) {
					if (IsConstantInfinity(binaryOperatorExpression.Left) || IsConstantInfinity(binaryOperatorExpression.Right))
						return;
					if (IsZero(binaryOperatorExpression.Left)) {
						AddIsZeroIssue(binaryOperatorExpression, binaryOperatorExpression.Right);
						return;
					}
					if (IsZero(binaryOperatorExpression.Right)) {
						AddIsZeroIssue(binaryOperatorExpression, binaryOperatorExpression.Left);
						return;
					}
					AddIssue(new CodeIssue(
						binaryOperatorExpression, 
						ctx.TranslateString("Comparison of floating point numbers can be unequal due to the differing precision of the two values."),
						ctx.TranslateString("Fix floating point number comparing. Compare a difference with epsilon."),
						script => {
							// Math.Abs(diff) op EPSILON
							var builder = ctx.CreateTypeSystemAstBuilder(binaryOperatorExpression);
							var diff = new BinaryOperatorExpression(binaryOperatorExpression.Left.Clone(),
							                                         BinaryOperatorType.Subtract, binaryOperatorExpression.Right.Clone());
							var abs = builder.ConvertType(new TopLevelTypeName("System", "Math")).Invoke("Abs", diff);
							var op = binaryOperatorExpression.Operator == BinaryOperatorType.Equality ?
								BinaryOperatorType.LessThan : BinaryOperatorType.GreaterThan;
							var epsilon = new IdentifierExpression("EPSILON");
							var compare = new BinaryOperatorExpression(abs, op, epsilon);
							script.Replace(binaryOperatorExpression, compare);
							script.Select(epsilon);
						}
					));
				}
			}
		}
	}
}
