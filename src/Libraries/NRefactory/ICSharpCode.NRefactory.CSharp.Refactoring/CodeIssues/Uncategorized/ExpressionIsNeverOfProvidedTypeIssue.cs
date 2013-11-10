// 
// ExpressionIsNeverOfProvidedTypeIssue.cs
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
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("CS0184:Given expression is never of the provided type",
		Description = "CS0184:Given expression is never of the provided type.",
		Category = IssueCategories.CompilerWarnings,
		Severity = Severity.Warning)]
	public class ExpressionIsNeverOfProvidedTypeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ExpressionIsNeverOfProvidedTypeIssue>
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

//				var conversions = CSharpConversions.Get(ctx.Compilation);
				var exprType = ctx.Resolve(isExpression.Expression).Type;
				var providedType = ctx.ResolveType(isExpression.Type);

				if (exprType.Kind == TypeKind.Unknown || providedType.Kind == TypeKind.Unknown)
					return;
				if (IsValidReferenceOrBoxingConversion(exprType, providedType))
					return;
				
				var exprTP = exprType as ITypeParameter;
				var providedTP = providedType as ITypeParameter;
				if (exprTP != null) {
					if (IsValidReferenceOrBoxingConversion(exprTP.EffectiveBaseClass, providedType)
					    && exprTP.EffectiveInterfaceSet.All(i => IsValidReferenceOrBoxingConversion(i, providedType)))
						return;
				}
				if (providedTP != null) {
					if (IsValidReferenceOrBoxingConversion(exprType, providedTP.EffectiveBaseClass))
						return;
				}
				
				AddIssue(new CodeIssue(isExpression, ctx.TranslateString("Given expression is never of the provided type")));
			}

			bool IsValidReferenceOrBoxingConversion(IType fromType, IType toType)
			{
				Conversion c = conversions.ExplicitConversion(fromType, toType);
				return c.IsValid && (c.IsIdentityConversion || c.IsReferenceConversion || c.IsBoxingConversion || c.IsUnboxingConversion);
			}
		}
	}
}
