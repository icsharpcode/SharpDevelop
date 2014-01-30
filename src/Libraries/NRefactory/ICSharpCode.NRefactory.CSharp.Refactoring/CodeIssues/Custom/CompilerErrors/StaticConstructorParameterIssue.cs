//
// 
// StaticConstructorParameterIssue.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
//      Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Ji Kun
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

using System.Linq;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Static constructor should be parameterless",
		Description = "Static constructor should be parameterless",
		Category = IssueCategories.CompilerErrors,
		Severity = Severity.Error)]
	public class StaticConstructorParameterIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<StaticConstructorParameterIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base(ctx)
			{
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				if (!constructorDeclaration.HasModifier(Modifiers.Static) || !constructorDeclaration.Parameters.Any())
					return;
				AddIssue(new CodeIssue(
					constructorDeclaration.Parameters.First().StartLocation,
					constructorDeclaration.Parameters.Last().EndLocation,
					ctx.TranslateString("Static constructor cannot take parameters"),
					ctx.TranslateString("Remove parameters"),
					s => {
						int o1 = ctx.GetOffset(constructorDeclaration.LParToken.EndLocation);
						int o2 = ctx.GetOffset(constructorDeclaration.RParToken.StartLocation);
						s.RemoveText(o1, o2 - o1);
					}
				));
			}
		}
	}
}
