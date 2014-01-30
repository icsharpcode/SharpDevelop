//
// RedundantNotNullAttributeInNonNullableTypeIssue.cs
//
// Author:
//      Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis
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
//

using System;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Use of NotNullAttribute in non-nullable type is redundant.",
		Description = "Detects unnecessary usages of the NotNullAttribute.",
		Category = IssueCategories.RedundanciesInDeclarations,
		Severity = Severity.Warning)]
	public class RedundantNotNullAttributeInNonNullableTypeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantNotNullAttributeInNonNullableTypeIssue>
		{
			public GatherVisitor(BaseRefactoringContext context) : base(context)
			{
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				// This is just here to prevent the visitor from unnecessarily visiting the children.
			}

			public override void VisitAttribute(Attribute attribute)
			{
				var attributeType = ctx.Resolve(attribute).Type;
				if (attributeType.FullName == AnnotationNames.NotNullAttribute) {
					if (IsAttributeRedundant(attribute)) {
						AddIssue(new CodeIssue(attribute,
							ctx.TranslateString("NotNullAttribute is not needed for non-nullable types"),
							ctx.TranslateString("Remove redundant NotNullAttribute"),
							script => script.RemoveAttribute(attribute)));
					}
				}
			}

			bool IsAttributeRedundant(Attribute attribute)
			{
				var section = attribute.GetParent<AttributeSection>();
				var targetDeclaration = section.Parent;

				if (targetDeclaration is FixedFieldDeclaration) {
					//Fixed fields are never null
					return true;
				}

				var fieldDeclaration = targetDeclaration as FieldDeclaration;
				if (fieldDeclaration != null) {
					return fieldDeclaration.Variables.All(variable => NullableType.IsNonNullableValueType(ctx.Resolve(variable).Type));
				}

				var resolveResult = ctx.Resolve(targetDeclaration);
				var memberResolveResult = resolveResult as MemberResolveResult;
				if (memberResolveResult != null) {
					return NullableType.IsNonNullableValueType(memberResolveResult.Member.ReturnType);
				}
				var localResolveResult = resolveResult as LocalResolveResult;
				if (localResolveResult != null) {
					return NullableType.IsNonNullableValueType(localResolveResult.Type);
				}

				return false;
			}
		}
	}
}

