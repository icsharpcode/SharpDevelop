//
// ConvertNullableToShortFormIssue.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Convert 'Nullable<T>' to 'T?'",
	                  Description = "Convert 'Nullable<T>' to the short form 'T?'",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "ConvertNullableToShortForm")]
	public class ConvertNullableToShortFormIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConvertNullableToShortFormIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}
			static readonly AstType nullType = new SimpleType("");

			void CheckType(AstType simpleType, AstType arg)
			{
				if (arg == null || nullType.IsMatch(arg))
					return;
				var rr = ctx.Resolve(simpleType);
				if (rr == null || rr.IsError || rr.Type.Namespace != "System" || rr.Type.Name != "Nullable")
					return;

				AddIssue(new CodeIssue(
					simpleType,
					string.Format(ctx.TranslateString("Type can be simplified to '{0}?'"), arg), 
					string.Format(ctx.TranslateString("Rewrite to '{0}?'"), arg),
					script =>  {
						script.Replace(simpleType, arg.Clone().MakeNullableType());
					}
				));
			}

			public override void VisitSimpleType(SimpleType simpleType)
			{
				CheckType(simpleType, simpleType.TypeArguments.FirstOrDefault());
			}

			public override void VisitMemberType(MemberType memberType)
			{
				CheckType(memberType, memberType.TypeArguments.FirstOrDefault());
			}
		}
	}
}

