//
// StaticEventSubscriptionIssue.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.PatternMatching;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using Mono.CSharp;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Static event removal check",
		Description = "Checks if static events are removed",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning)]
	public class StaticEventSubscriptionIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<StaticEventSubscriptionIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitSyntaxTree(SyntaxTree syntaxTree)
			{
				base.VisitSyntaxTree(syntaxTree);

				foreach (var assignedEvent in assignedEvents) {
					addedEvents.Remove(assignedEvent); 
				}

				foreach (var hs in removedEvents) {
					HashSet<Tuple<IMember, AssignmentExpression>> h;
					if (!addedEvents.TryGetValue(hs.Key, out h))
						continue;
					foreach (var evt in hs.Value) {
						restart:
						foreach (var m in h) {
							if (m.Item1 == evt) {
								h.Remove(m);
								goto restart;
							}
						}
					}
				}

				foreach (var added in addedEvents) {
					foreach (var usage in added.Value) {
						AddIssue(new CodeIssue(usage.Item2.OperatorToken,
							ctx.TranslateString("Subscription to static events without unsubscription may cause memory leaks."))); 
					}
				}
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				if (methodDeclaration.HasModifier(Modifiers.Static))
					return;

				base.VisitMethodDeclaration(methodDeclaration);
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				if (constructorDeclaration.HasModifier(Modifiers.Static))
					return;
				base.VisitConstructorDeclaration(constructorDeclaration);
			}
			readonly Dictionary<IMember, HashSet<Tuple<IMember, AssignmentExpression>>> addedEvents = new Dictionary<IMember, HashSet<Tuple<IMember, AssignmentExpression>>>();
			readonly Dictionary<IMember, HashSet<IMember>> removedEvents = new Dictionary<IMember, HashSet<IMember>>();
			readonly HashSet<IMember> assignedEvents = new HashSet<IMember> ();

			public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
			{
				base.VisitAssignmentExpression(assignmentExpression);

				if (assignmentExpression.Operator == AssignmentOperatorType.Add) {
					var left = ctx.Resolve(assignmentExpression.Left) as MemberResolveResult;
					if (left == null || left.Member.SymbolKind != SymbolKind.Event || !left.Member.IsStatic)
						return;

					if (assignmentExpression.Right is AnonymousMethodExpression || assignmentExpression.Right is LambdaExpression) {
						AddIssue(new CodeIssue(assignmentExpression.OperatorToken,
							ctx.TranslateString("Subscription to static events with an anonymous method may cause memory leaks."))); 
					}


					var right = ctx.Resolve(assignmentExpression.Right) as MethodGroupResolveResult;
					if (right == null || right.Methods.Count() != 1)
						return;
					HashSet<Tuple<IMember, AssignmentExpression>> hs;
					if (!addedEvents.TryGetValue(left.Member, out hs))
						addedEvents[left.Member] = hs = new HashSet<Tuple<IMember, AssignmentExpression>>();
					hs.Add(Tuple.Create((IMember)right.Methods.First(), assignmentExpression));
				} else if (assignmentExpression.Operator == AssignmentOperatorType.Subtract) {
					var left = ctx.Resolve(assignmentExpression.Left) as MemberResolveResult;
					if (left == null || left.Member.SymbolKind != SymbolKind.Event || !left.Member.IsStatic)
						return;
					var right = ctx.Resolve(assignmentExpression.Right) as MethodGroupResolveResult;
					if (right == null || right.Methods.Count() != 1)
						return;
					HashSet<IMember> hs;
					if (!removedEvents.TryGetValue(left.Member, out hs))
						removedEvents[left.Member] = hs = new HashSet<IMember>();
					hs.Add(right.Methods.First());
				} else if (assignmentExpression.Operator == AssignmentOperatorType.Assign) {
					var left = ctx.Resolve(assignmentExpression.Left) as MemberResolveResult;
					if (left == null || left.Member.SymbolKind != SymbolKind.Event || !left.Member.IsStatic)
						return;
					assignedEvents.Add(left.Member); 
				}
			}
		}
	}
}

