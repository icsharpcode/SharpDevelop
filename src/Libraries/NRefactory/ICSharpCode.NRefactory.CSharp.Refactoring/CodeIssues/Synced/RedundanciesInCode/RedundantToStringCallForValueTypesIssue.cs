//
// RedundantToStringCallForValueTypesIssue.cs
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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant 'object.ToString()' call for value types",
		Description = "Finds value type calls to ToString() which would be generated automatically by the compiler.",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Hint)]
	public class RedundantToStringCallForValueTypesIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantToStringCallForValueTypesIssue>
		{
			static Tuple<int, int> onlyFirst = Tuple.Create (0, 0);

			static IDictionary<Tuple<string, int>, Tuple<int, int>> membersCallingToString = new Dictionary<Tuple<string, int>, Tuple<int, int>> {
				{ Tuple.Create("System.IO.TextWriter.Write", 1), onlyFirst },
				{ Tuple.Create("System.IO.TextWriter.WriteLine", 1), onlyFirst },
				{ Tuple.Create("System.Console.Write", 1), onlyFirst },
				{ Tuple.Create("System.Console.WriteLine", 1), onlyFirst }
			};

			public GatherVisitor (BaseRefactoringContext context) : base (context)
			{
				binOpVisitor = new BinaryExpressionVisitor (this);
			}

			HashSet<AstNode> processedNodes = new HashSet<AstNode>();

			void CheckExpressionInAutoCallContext(Expression expression)
			{
				if (expression is InvocationExpression && !processedNodes.Contains(expression)) {
					CheckInvocationInAutoCallContext((InvocationExpression)expression);
				}
			}

			void CheckInvocationInAutoCallContext(InvocationExpression invocationExpression)
			{
				var memberExpression = invocationExpression.Target as MemberReferenceExpression;
				if (memberExpression == null) {
					return;
				}
				if (memberExpression.MemberName != "ToString" || invocationExpression.Arguments.Any ()) {
					return;
				}

				var resolveResult = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (resolveResult == null)
					return;
				if (ctx.Resolve(memberExpression.Target).Type.Kind == TypeKind.Struct) 
					AddRedundantToStringIssue(memberExpression, invocationExpression);
			}

			void AddRedundantToStringIssue(MemberReferenceExpression memberExpression, InvocationExpression invocationExpression)
			{
				// Simon Lindgren 2012-09-14: Previously there was a check here to see if the node had already been processed
				// This has been moved out to the callers, to check it earlier for a 30-40% run time reduction
				processedNodes.Add(invocationExpression);

				AddIssue(new CodeIssue(memberExpression.DotToken.StartLocation, invocationExpression.RParToken.EndLocation,
				         ctx.TranslateString("Redundant ToString() call"), 
				         ctx.TranslateString("Remove redundant '.ToString()'"), script =>  {
					script.Replace(invocationExpression, memberExpression.Target.Clone());
					}) { IssueMarker = IssueMarker.GrayOut });
			}

			#region Binary operator
			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);

				if (binaryOperatorExpression.Operator != BinaryOperatorType.Add)
					return;
				binOpVisitor.Reset();
				binaryOperatorExpression.AcceptVisitor(binOpVisitor);
			}

			BinaryExpressionVisitor binOpVisitor;
			class BinaryExpressionVisitor : DepthFirstAstVisitor
			{
				GatherVisitor issue;
				int stringExpressionCount;
				Expression firstStringExpression;

				public BinaryExpressionVisitor(GatherVisitor issue)
				{
					this.issue = issue;
				}

				public void Reset()
				{
					stringExpressionCount = 0;
					firstStringExpression = null;
				}

				void Check (Expression expression)
				{
					if (expression is BinaryOperatorExpression) {
						expression.AcceptVisitor(this);
						return;
					}
					if (stringExpressionCount <= 1) {
						var resolveResult = issue.ctx.Resolve(expression);
						if (resolveResult.Type.IsKnownType(KnownTypeCode.String)) {
							stringExpressionCount++;
							if (stringExpressionCount == 1) {
								firstStringExpression = expression;
							} else {
								issue.CheckExpressionInAutoCallContext(firstStringExpression);
								issue.CheckExpressionInAutoCallContext(expression);
							}
						}
					} else {
						issue.CheckExpressionInAutoCallContext(expression);
					}
				}

				public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
				{
					Check(binaryOperatorExpression.Left);
					Check(binaryOperatorExpression.Right);
				}
			}

			#endregion

			#region Invocation expression
			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);

				//var target = invocationExpression.Target as MemberReferenceExpression;

				var invocationResolveResult = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (invocationResolveResult == null) {
					return;
				}
				IMember member = invocationResolveResult.Member;

				// Check list of members that call ToString() automatically
				CheckAutomaticToStringCallers(invocationExpression, member);

				// Check formatting calls
				CheckFormattingCall(invocationExpression, invocationResolveResult);
			}

			void CheckAutomaticToStringCallers(InvocationExpression invocationExpression, IMember member)
			{
				if (member.IsOverride) {
					member = InheritanceHelper.GetBaseMember(member);
					if (member == null) {
						return;
					}
				}
				var key = new Tuple<string, int>(member.ReflectionName, invocationExpression.Arguments.Count);
				Tuple<int, int> checkInfo;
				if (membersCallingToString.TryGetValue(key, out checkInfo)) {
					var arguments = invocationExpression.Arguments.ToList();
					for (int i = checkInfo.Item1; i < Math.Min(invocationExpression.Arguments.Count, checkInfo.Item2 + 1); ++i) {
						CheckExpressionInAutoCallContext(arguments[i]);
					}
				}
			}

			void CheckFormattingCall(InvocationExpression invocationExpression, CSharpInvocationResolveResult invocationResolveResult)
			{
				Expression formatArgument;
				IList<Expression> formatArguments;
				// Only check parameters that are of type object: String means it is neccessary, others
				// means that there is another problem (ie no matching overload of the method).
				Func<IParameter, Expression, bool> predicate = (parameter, argument) => {
					var type = parameter.Type;
					if (type is TypeWithElementType && parameter.IsParams) {
						type = ((TypeWithElementType)type).ElementType;
					}
					var typeDefinition = type.GetDefinition();
					if (typeDefinition == null)
					return false;
					return typeDefinition.IsKnownType(KnownTypeCode.Object);
				};
				if (FormatStringHelper.TryGetFormattingParameters(invocationResolveResult, invocationExpression,
				                                                  out formatArgument, out formatArguments, predicate)) {
					foreach (var argument in formatArguments) {
						CheckExpressionInAutoCallContext(argument);
					}
				}
			}
			#endregion
		}
	}
}

