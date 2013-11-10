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
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("CS0127: A method with a void return type cannot return a value.",
	                  Description = "Since 'function' returns void, a return keyword must not be followed by an object expression",
	                  Category = IssueCategories.CompilerErrors,
	                  Severity = Severity.Error)]
	public class CS0127ReturnMustNotBeFollowedByAnyExpression : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CS0127ReturnMustNotBeFollowedByAnyExpression>
		{
			bool skip;

			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static bool AnonymousMethodMayReturnNonVoid(BaseRefactoringContext ctx, Expression anonymousMethodExpression)
			{
				foreach (var type in TypeGuessing.GetValidTypes(ctx.Resolver, anonymousMethodExpression)) {
					if (type.Kind != TypeKind.Delegate)
						continue;
					var invoke = type.GetDelegateInvokeMethod();
					if (invoke != null && !invoke.ReturnType.IsKnownType(KnownTypeCode.Void))
						return true;
				}
				return false;
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				var primitiveType = methodDeclaration.ReturnType as PrimitiveType;
				if (primitiveType == null || primitiveType.Keyword != "void")
					return;
				base.VisitMethodDeclaration(methodDeclaration);
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
			}

			public override void VisitAccessor(Accessor accessor)
			{
				if (accessor.Role == PropertyDeclaration.SetterRole || 
				    accessor.Role == IndexerDeclaration.SetterRole )
				base.VisitAccessor(accessor);
			}
	
				
			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
			}

			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
				bool old = skip;
				skip = AnonymousMethodMayReturnNonVoid(ctx, anonymousMethodExpression);
				base.VisitAnonymousMethodExpression(anonymousMethodExpression);
				skip = old;
			}

			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
				bool old = skip;
				skip = AnonymousMethodMayReturnNonVoid(ctx, lambdaExpression);
				base.VisitLambdaExpression(lambdaExpression);
				skip = old;
			}

			public override void VisitReturnStatement(ReturnStatement returnStatement)
			{
				base.VisitReturnStatement(returnStatement);
				if (skip)
					return;

				if (!returnStatement.Expression.IsNull) {
					var actions = new List<CodeAction>();
					actions.Add(new CodeAction(ctx.TranslateString("Remove returned expression"), script => {
						script.Replace(returnStatement, new ReturnStatement());
					}, returnStatement));

					var method = returnStatement.GetParent<MethodDeclaration>();
					if (method != null) {
						var rr = ctx.Resolve(returnStatement.Expression);
						if (rr != null && !rr.IsError) {
							actions.Add(new CodeAction(ctx.TranslateString("Change return type of method."), script => {
								script.Replace(method.ReturnType, ctx.CreateTypeSystemAstBuilder(method).ConvertType(rr.Type));
							}, returnStatement));
						}
					}

					AddIssue(new CodeIssue(
						returnStatement, 
						ctx.TranslateString("Return type is 'void'"),
						actions
					));
				}
			}
		}
	}
}

