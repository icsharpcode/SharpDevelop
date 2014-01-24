//
// ConvertToAutoPropertyIssue.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Convert property to auto property",
		Description = "Convert property to auto property",
		Category = IssueCategories.Opportunities,
		Severity = Severity.Suggestion,
		AnalysisDisableKeyword = "ConvertToAutoProperty")]
	public class ConvertToAutoPropertyIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConvertToAutoPropertyIssue>
		{
			readonly Stack<TypeDeclaration> typeStack = new Stack<TypeDeclaration>();

			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				// SKIP
			}

			bool IsValidField(IField field)
			{
				if (field == null || field.Attributes.Count > 0 || field.IsVolatile)
					return false;
				foreach (var m in typeStack.Peek().Members.OfType<FieldDeclaration>()) {
					foreach (var i in m.Variables) {
						if (i.StartLocation == field.BodyRegion.Begin) {
							if (!i.Initializer.IsNull)
								return false;
							break;
						}
					}
				}
				return true;
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				var field = RemoveBackingStoreAction.GetBackingField(ctx, propertyDeclaration);
				if (!IsValidField(field))
					return;
				AddIssue(new CodeIssue(
					propertyDeclaration.NameToken,
					ctx.TranslateString("Convert to auto property")
				) {
					ActionProvider = { typeof (RemoveBackingStoreAction) }
				}
				);
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				typeStack.Push(typeDeclaration); 
				base.VisitTypeDeclaration(typeDeclaration);
				typeStack.Pop();
			}
		}
	}
}

