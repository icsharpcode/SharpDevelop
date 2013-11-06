//
// UnassignedReadonlyFieldIssue.cs
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Analysis;
using System.Linq;
using System;
using ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Unassigned readonly field",
	                  Description = "Unassigned readonly field",
	                  Category = IssueCategories.CompilerWarnings,
	                  Severity = Severity.Warning,
	                  PragmaWarning = 649,
	                  AnalysisDisableKeyword = "UnassignedReadonlyField.Compiler")]
	public class UnassignedReadonlyFieldIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<UnassignedReadonlyFieldIssue>
		{
			readonly Stack<List<Tuple<VariableInitializer, IVariable>>> fieldStack = new Stack<List<Tuple<VariableInitializer, IVariable>>>();

			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			void Collect()
			{
				foreach (var varDecl in fieldStack.Peek()) {
					var resolveResult = ctx.Resolve(varDecl.Item1) as MemberResolveResult;
					if (resolveResult == null || resolveResult.IsError)
						continue;
					AddIssue(new CodeIssue(
						varDecl.Item1.NameToken,
						string.Format(ctx.TranslateString("Readonly field '{0}' is never assigned"), varDecl.Item1.Name),
						ctx.TranslateString("Initialize field from constructor parameter"),
						script => {
						script.InsertWithCursor(
							ctx.TranslateString("Create constructor"),
							resolveResult.Member.DeclaringTypeDefinition,
							(s, c) => {
							return new ConstructorDeclaration {
								Name = resolveResult.Member.DeclaringTypeDefinition.Name,
								Modifiers = Modifiers.Public,
								Body = new BlockStatement {
										new AssignmentExpression(
											new MemberReferenceExpression(new ThisReferenceExpression(), varDecl.Item1.Name),
											new IdentifierExpression(varDecl.Item1.Name)
										)
								},
								Parameters = {
									new ParameterDeclaration(c.CreateShortType(resolveResult.Type), varDecl.Item1.Name)
								}
							};
						}
						);
					}
					));
				}
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				var list = new List<Tuple<VariableInitializer, IVariable>>();
				fieldStack.Push(list);
				foreach (var fieldDeclaration in ConvertToConstantIssue.CollectFields(this, typeDeclaration)) {
					if (!fieldDeclaration.HasModifier(Modifiers.Readonly))
						continue;
//					var rr = ctx.Resolve(fieldDeclaration.ReturnType);
				
					if (fieldDeclaration.Variables.Count() > 1)
						continue;
					if (!fieldDeclaration.Variables.First().Initializer.IsNull)
						continue;
					var variable = fieldDeclaration.Variables.First();
					var mr = ctx.Resolve(variable) as MemberResolveResult;
					if (mr == null)
						continue;
					list.Add(Tuple.Create(variable, mr.Member as IVariable)); 
				}
				base.VisitTypeDeclaration(typeDeclaration);
				Collect();
				fieldStack.Pop();
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				var assignmentAnalysis = new ConvertToConstantIssue.VariableUsageAnalyzation(ctx);
				var newVars = new List<Tuple<VariableInitializer, IVariable>>();
				blockStatement.AcceptVisitor(assignmentAnalysis); 
				foreach (var variable in fieldStack.Pop()) {
					if (assignmentAnalysis.GetStatus(variable.Item2) == VariableState.Changed)
						continue;
					newVars.Add(variable);
				}
				fieldStack.Push(newVars);
			}
		}
	}
}

