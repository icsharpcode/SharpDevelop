//
// BaseMethodParameterNameMismatchIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription(
		"Parameter name differs in base declaration",
		Description = "Parameter name differs in base declaration",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning)]
	public class BaseMethodParameterNameMismatchIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<BaseMethodParameterNameMismatchIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				// skip
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				// skip
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
				// skip
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				// skip
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				// skip
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				// SKIP
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
				Check (indexerDeclaration);
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				Check (methodDeclaration);
			}

			void Check (EntityDeclaration entity)
			{
				if (!entity.HasModifier(Modifiers.Override))
					return;
				var rr = ctx.Resolve(entity) as MemberResolveResult;
				if (rr == null || rr.IsError)
					return;
				var method = rr.Member as IParameterizedMember;
				if (method == null)
					return;
				var baseMethod = InheritanceHelper.GetBaseMember(method) as IParameterizedMember;
				if (baseMethod == null)
					return;

				for (int i = 0; i < Math.Min (method.Parameters.Count, baseMethod.Parameters.Count); i++) {
					var arg     = method.Parameters[i];
					var baseArg = baseMethod.Parameters[i];

					if (arg.Name != baseArg.Name) {
						int _i = i;
						var parameters = entity.GetChildrenByRole (Roles.Parameter);
						AddIssue(new CodeIssue(
							parameters.ElementAt(_i).NameToken,
							ctx.TranslateString("Parameter name differs in base method declaration"),
							string.Format(ctx.TranslateString("Rename to '{0}'"), baseArg.Name),
							s => {
								s.Rename(arg, baseArg.Name);
							}
						));
					}
				}
			}
		}
	}
}

