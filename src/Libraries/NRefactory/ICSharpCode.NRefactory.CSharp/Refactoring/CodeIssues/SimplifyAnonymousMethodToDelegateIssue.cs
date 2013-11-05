//
// SimplifyAnonymousMethodToDelegateIssue.cs
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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Simplify anonymous method to delegate",
	                  Description = "Shows anonymous methods that can be simplified.",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning,
                      ResharperDisableKeyword = "ConvertClosureToMethodGroup")]
	public class SimplifyAnonymousMethodToDelegateIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase<SimplifyAnonymousMethodToDelegateIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}
			
			static Pattern pattern = new Choice {
				new BlockStatement {
					new ReturnStatement (new AnyNode ("invoke")) 
				},
				new AnyNode ("invoke")
			};

			static InvocationExpression AnalyzeBody(AstNode body)
			{
				var m = pattern.Match(body);
				if (m.Success)
					return m.Get("invoke").Single () as InvocationExpression;
				return null;
			}

			static bool IsSimpleTarget(Expression target)
			{
				if (target is IdentifierExpression)
					return true;
				var mref = target as MemberReferenceExpression;
				if (mref != null)
					return IsSimpleTarget (mref.Target);
				var pref = target as PointerReferenceExpression;
				if (pref != null)
					return IsSimpleTarget (pref.Target);
				return false;
			}

			void AnalyzeExpression(AstNode expression, AstNode body, AstNodeCollection<ParameterDeclaration> parameters)
			{
				var invocation = AnalyzeBody(body);
				if (invocation == null)
					return;
				if (!IsSimpleTarget (invocation.Target))
					return;
				var rr = ctx.Resolve (invocation) as CSharpInvocationResolveResult;
				if (rr == null)
					return;
				var lambdaParameters = parameters.ToList();
				var arguments = rr.GetArgumentsForCall();
				if (lambdaParameters.Count != arguments.Count)
					return;
				for (int i = 0; i < arguments.Count; i++) {
					var arg = UnpackImplicitIdentityOrReferenceConversion(arguments[i]) as LocalResolveResult;
					if (arg == null || arg.Variable.Name != lambdaParameters[i].Name)
						return;
				}
				var returnConv = ctx.GetConversion(invocation);
				if (returnConv.IsExplicit || !(returnConv.IsIdentityConversion || returnConv.IsReferenceConversion))
					return;
				AddIssue(expression, ctx.TranslateString("Expression can be reduced to delegate"), script =>  {
					var validTypes = CreateFieldAction.GetValidTypes (ctx.Resolver, expression).ToList ();
					if (validTypes.Any (t => t.FullName == "System.Func" && t.TypeParameterCount == 1 + parameters.Count) && validTypes.Any (t => t.FullName == "System.Action")) {
						if (rr != null && rr.Member.ReturnType.Kind != TypeKind.Void) {
							var builder = ctx.CreateTypeSystemAstBuilder (expression);
							var type = builder.ConvertType(new TopLevelTypeName("System", "Func", 1));
							var args = type.GetChildrenByRole(Roles.TypeArgument);
							args.Clear ();
							foreach (var pde in parameters) {
								args.Add (builder.ConvertType (ctx.Resolve (pde).Type));
							}
							args.Add (builder.ConvertType (rr.Member.ReturnType));
							script.Replace(expression, new CastExpression (type, invocation.Target.Clone()));
							return;
						}
					}
					script.Replace(expression, invocation.Target.Clone());
				});
			}
			
			static ResolveResult UnpackImplicitIdentityOrReferenceConversion(ResolveResult rr)
			{
				var crr = rr as ConversionResolveResult;
				if (crr != null && crr.Conversion.IsImplicit && (crr.Conversion.IsIdentityConversion || crr.Conversion.IsReferenceConversion))
					return crr.Input;
				return rr;
			}

			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
				base.VisitLambdaExpression(lambdaExpression);
				AnalyzeExpression(lambdaExpression, lambdaExpression.Body, lambdaExpression.Parameters);
			}

			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
				base.VisitAnonymousMethodExpression(anonymousMethodExpression);
				AnalyzeExpression(anonymousMethodExpression, anonymousMethodExpression.Body, anonymousMethodExpression.Parameters);
			}
		}
	}
}

