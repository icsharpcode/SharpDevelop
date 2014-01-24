//
// CS0126ReturnMustBeFollowedByAnyExpression.cs
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
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("CS0126: A method with return type cannot return without value.",
	                  Description = "Since 'function' doesn't return void, a return keyword must be followed by an object expression",
	                  Category = IssueCategories.CompilerErrors,
	                  Severity = Severity.Error)]
	public class CS0126ReturnMustBeFollowedByAnyExpression : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		internal static IType GetRequestedReturnType (BaseRefactoringContext ctx, AstNode returnStatement, out AstNode entityNode)
		{
			entityNode = returnStatement.GetParent(p => p is LambdaExpression || p is AnonymousMethodExpression || !(p is Accessor) && p is EntityDeclaration);
			if (entityNode == null)
				return SpecialType.UnknownType;
			if (entityNode is EntityDeclaration) {
				var rr = ctx.Resolve(entityNode) as MemberResolveResult;
				if (rr == null)
					return SpecialType.UnknownType;
				if (((EntityDeclaration)entityNode).HasModifier(Modifiers.Async))
					return TaskType.UnpackTask(ctx.Compilation, rr.Member.ReturnType);
				return rr.Member.ReturnType;
			}
			bool isAsync = false;
			if (entityNode is LambdaExpression)
				isAsync = ((LambdaExpression)entityNode).IsAsync;
			if (entityNode is AnonymousMethodExpression)
				isAsync = ((AnonymousMethodExpression)entityNode).IsAsync;
			foreach (var type in TypeGuessing.GetValidTypes(ctx.Resolver, entityNode)) {
				if (type.Kind != TypeKind.Delegate)
					continue;
				var invoke = type.GetDelegateInvokeMethod();
				if (invoke != null) {
					return isAsync ? TaskType.UnpackTask(ctx.Compilation, invoke.ReturnType) : invoke.ReturnType;
				}
			}
			return SpecialType.UnknownType;
		}


		class GatherVisitor : GatherVisitorBase<CS0126ReturnMustBeFollowedByAnyExpression>
		{
			string currentMethodName;

			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			bool skip;

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				var primitiveType = methodDeclaration.ReturnType as PrimitiveType;
				skip = (primitiveType != null && primitiveType.Keyword == "void");
				currentMethodName = methodDeclaration.Name;
				base.VisitMethodDeclaration(methodDeclaration);
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				currentMethodName = constructorDeclaration.Name;
				skip = true;
				base.VisitConstructorDeclaration(constructorDeclaration);
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				currentMethodName = "~" + destructorDeclaration.Name;
				skip = true;
				base.VisitDestructorDeclaration(destructorDeclaration);
			}

			public override void VisitAccessor(Accessor accessor)
			{
				bool old = skip; 
				skip = accessor.Role != PropertyDeclaration.GetterRole && accessor.Role != IndexerDeclaration.GetterRole;
				base.VisitAccessor(accessor);
				skip = old;
			}

			static bool AnonymousMethodMayReturnVoid(BaseRefactoringContext ctx, Expression anonymousMethodExpression)
			{
				foreach (var type in TypeGuessing.GetValidTypes(ctx.Resolver, anonymousMethodExpression)) {
					if (type.Kind != TypeKind.Delegate)
						continue;
					var invoke = type.GetDelegateInvokeMethod();
					if (invoke != null && invoke.ReturnType.IsKnownType(KnownTypeCode.Void))
						return true;
				}
				return false;
			}


			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
				bool old = skip;
				skip = AnonymousMethodMayReturnVoid(ctx, anonymousMethodExpression);
				base.VisitAnonymousMethodExpression(anonymousMethodExpression);
				skip = old;
			}

			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
				bool old = skip;
				skip = AnonymousMethodMayReturnVoid(ctx, lambdaExpression);
				base.VisitLambdaExpression(lambdaExpression);
				skip = old;
			}

			public override void VisitReturnStatement(ReturnStatement returnStatement)
			{
				base.VisitReturnStatement(returnStatement);
				if (skip)
					return;

				if (returnStatement.Expression.IsNull) {
					var entity = returnStatement.GetParent<EntityDeclaration>();
					if (entity is Accessor)
						entity = entity.GetParent<EntityDeclaration>();
					if (entity == null)
						return;
					AstNode entityNode;
					var rr = GetRequestedReturnType (ctx, returnStatement, out entityNode);
					if (rr.Kind == TypeKind.Void)
						return;
					var actions = new List<CodeAction>();
					if (rr.Kind != TypeKind.Unknown) {
						actions.Add(new CodeAction(ctx.TranslateString("Return default value"), script => {
							Expression p;
							if (rr.IsKnownType(KnownTypeCode.Boolean)) {
								p = new PrimitiveExpression(false);
							} else if (rr.IsKnownType(KnownTypeCode.String)) {
								p = new PrimitiveExpression("");
							} else if (rr.IsKnownType(KnownTypeCode.Char)) {
								p = new PrimitiveExpression(' ');
							} else if (rr.IsReferenceType == true) {
								p = new NullReferenceExpression();
							} else if (rr.GetDefinition() != null &&
								rr.GetDefinition().KnownTypeCode < KnownTypeCode.DateTime) {
								p = new PrimitiveExpression(0x0);
							} else {
								p = new DefaultValueExpression(ctx.CreateTypeSystemAstBuilder(returnStatement).ConvertType(rr));
							}

							script.Replace(returnStatement, new ReturnStatement(p));
						}, returnStatement));
					}
					var method = returnStatement.GetParent<MethodDeclaration>();
					if (method != null) {
						actions.Add(new CodeAction(ctx.TranslateString("Change method return type to 'void'"), script => {
							script.Replace(method.ReturnType, new PrimitiveType("void"));
						}, returnStatement));
					}

					AddIssue(new CodeIssue(
						returnStatement, 
						string.Format(ctx.TranslateString("`{0}': A return keyword must be followed by any expression when method returns a value"), currentMethodName),
						actions
					));
				}
			}
		}
	}
}

