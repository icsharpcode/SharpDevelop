//
// CS0127ReturnMustNotBeFollowedByAnyExpression.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("CS0127: A method with a void return type cannot return a value.",
	                  Description = "Since 'function' returns void, a return keyword must not be followed by an object expression",
	                  Category = IssueCategories.CompilerErrors,
	                  Severity = Severity.Error)]
	public class CS0127ReturnMustNotBeFollowedByAnyExpression : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase<CS0127ReturnMustNotBeFollowedByAnyExpression>
		{
			string currentMethodName;

			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}
			
			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				var primitiveType = methodDeclaration.ReturnType as PrimitiveType;
				if (primitiveType == null || primitiveType.Keyword != "void")
					return;
				currentMethodName = methodDeclaration.Name;
				base.VisitMethodDeclaration(methodDeclaration);
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				currentMethodName = constructorDeclaration.Name;
				base.VisitConstructorDeclaration(constructorDeclaration);
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				currentMethodName = "~" + destructorDeclaration.Name;
				base.VisitDestructorDeclaration(destructorDeclaration);
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
			}

			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
			}

			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
			}

			public override void VisitReturnStatement(ReturnStatement returnStatement)
			{
				if (!returnStatement.Expression.IsNull) {
					AddIssue(
						returnStatement, 
						string.Format (ctx.TranslateString("`{0}': A return keyword must not be followed by any expression when method returns void"), currentMethodName),
						new CodeAction (
							ctx.TranslateString("Remove returned expression"),
							script => {
								script.Remove(returnStatement.Expression); 
							},
							returnStatement
						)
					);
				}
			}
		}
	}
}

