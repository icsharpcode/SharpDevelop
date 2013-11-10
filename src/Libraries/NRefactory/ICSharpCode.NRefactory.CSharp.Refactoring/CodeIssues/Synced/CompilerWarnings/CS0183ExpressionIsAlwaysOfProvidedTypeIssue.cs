// 
// ExpressionIsAlwaysOfProvidedTypeIssue.cs
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("CS0183:Given expression is always of the provided type",
		Description = "CS0183:Given expression is always of the provided type.",
		Category = IssueCategories.CompilerWarnings,
		Severity = Severity.Warning,
		PragmaWarning = 183,
		AnalysisDisableKeyword = "CSharpWarnings::CS0183"
	)]
	public class CS0183ExpressionIsAlwaysOfProvidedTypeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CS0183ExpressionIsAlwaysOfProvidedTypeIssue>
		{
			readonly CSharpConversions conversions;

			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
				conversions = CSharpConversions.Get(ctx.Compilation);
			}

			public override void VisitIsExpression(IsExpression isExpression)
			{
				base.VisitIsExpression(isExpression);

				var type = ctx.Resolve(isExpression.Expression).Type;
				var providedType = ctx.ResolveType(isExpression.Type);

				if (type.Kind == TypeKind.Unknown || providedType.Kind == TypeKind.Unknown)
					return;
//				var foundConversion = conversions.ImplicitConversion(type, providedType);
				if (!IsValidReferenceOrBoxingConversion(type, providedType))
					return;

				var action = new CodeAction(
					             ctx.TranslateString("Compare with 'null'"), 
					             script => script.Replace(isExpression, new BinaryOperatorExpression(
						             isExpression.Expression.Clone(), BinaryOperatorType.InEquality, new PrimitiveExpression(null))),
					             isExpression
				             );
				AddIssue(new CodeIssue(isExpression, ctx.TranslateString("Given expression is always of the provided type. Consider comparing with 'null' instead"), new [] { action }));
			}

			bool IsValidReferenceOrBoxingConversion(IType fromType, IType toType)
			{
				Conversion c = conversions.ImplicitConversion(fromType, toType);
				return c.IsValid && (c.IsIdentityConversion || c.IsReferenceConversion || c.IsBoxingConversion);
			}
		}
	}
}
