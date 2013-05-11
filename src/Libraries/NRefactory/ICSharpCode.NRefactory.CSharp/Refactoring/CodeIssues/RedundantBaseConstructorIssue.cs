// 
// RedundantBaseConstructorIssue.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Remove redundant base call",
	                   Description = "Remove base constructor call that is redundant",
	                   Category = IssueCategories.Redundancies,
	                   Severity = Severity.Suggestion,
	                   IssueMarker = IssueMarker.GrayOut, 
	                   ResharperDisableKeyword = "RedundantBaseConstructorCall")]
	public class RedundantBaseConstructorIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase<RedundantAttributeParenthesesIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				base.VisitConstructorDeclaration(constructorDeclaration);

				if (constructorDeclaration.Initializer.ConstructorInitializerType != ConstructorInitializerType.Base)
					return;
				if (constructorDeclaration.Initializer.IsNull)
					return;
				if (constructorDeclaration.Initializer.Arguments.Count != 0)
					return;
				AddIssue(constructorDeclaration.Initializer.StartLocation, constructorDeclaration.Initializer.EndLocation,
				         ctx.TranslateString("Remove redundant base constructor"),
				         script => {
					var clone = (ConstructorDeclaration)constructorDeclaration.Clone();
					script.Replace(clone.ColonToken, CSharpTokenNode.Null.Clone());
					script.Replace(constructorDeclaration.Initializer, ConstructorInitializer.Null.Clone());
				});
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				//ignore properties
			}

			public override void  VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				//ignore fields
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				//ignore method declarations
			}
		}
	}
}
