//
// RedundantToStringIssue.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant call to ToString()",
	                  Description = "Finds calls to ToString() which would occur anyway.",
	                  Category = IssueCategories.Redundancies,
	                  Severity = Severity.Suggestion,
	                  IssueMarker = IssueMarker.GrayOut)]
	public class RedundantToStringIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}
		
		class GatherVisitor : GatherVisitorBase
		{
			IType stringType;

			IType objectType;

			static Tuple<int, int> onlyFirst = Tuple.Create (0, 0);

			static IDictionary<Tuple<string, int>, Tuple<int, int>> membersCallingToString = new Dictionary<Tuple<string, int>, Tuple<int, int>> {
				{ Tuple.Create("System.IO.TextWriter.Write", 1), onlyFirst },
				{ Tuple.Create("System.IO.TextWriter.WriteLine", 1), onlyFirst },
				{ Tuple.Create("System.Console.Write", 1), onlyFirst },
				{ Tuple.Create("System.Console.WriteLine", 1), onlyFirst }
			};
						
			public GatherVisitor (BaseRefactoringContext context) : base (context)
			{
				stringType = context.Compilation.FindType(KnownTypeCode.String);
				objectType = context.Compilation.FindType(KnownTypeCode.Object);
			}
			
			HashSet<AstNode> processedNodes = new HashSet<AstNode>();
			
			void AddRedundantToStringIssue(MemberReferenceExpression memberExpression, InvocationExpression invocationExpression)
			{
				if (processedNodes.Contains(invocationExpression)) {
					return;
				}
				processedNodes.Add(invocationExpression);
				
				AddIssue(memberExpression.DotToken.StartLocation, invocationExpression.RParToken.EndLocation,
				         ctx.TranslateString("Remove redundant call to ToString()"), script =>  {
					script.Replace(invocationExpression, memberExpression.Target.Clone());
				});
			}

			#region Binary operator
			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);

				if (binaryOperatorExpression.Operator != BinaryOperatorType.Add)
					return;

				var expressions = FlattenBinaryOperator(binaryOperatorExpression);

				int stringExpressionCount = 0;
				foreach (var expression in expressions) {
					var resolveResult = ctx.Resolve(expression);
					if (resolveResult.Type == stringType) {
						stringExpressionCount++;
					}
					if (stringExpressionCount > 1) {
						break;
					}
				}
				if (stringExpressionCount <= 1) {
					return;
				}

				foreach (var expression in expressions) {
					CheckExpressionInAutoCallContext(expression);
				}
			}

			IList<Expression> FlattenBinaryOperator(BinaryOperatorExpression binaryOperatorExpression)
			{
				var expressions = new List<Expression>();
				FlattenBinaryOperator(binaryOperatorExpression, expressions);
				return expressions;
			}

			void FlattenBinaryOperator(BinaryOperatorExpression binaryOperatorExpression, IList<Expression> expressions)
			{
				FlattenExpression(binaryOperatorExpression.Left, expressions);
				FlattenExpression(binaryOperatorExpression.Right, expressions);
			}

			void FlattenExpression(Expression expression, IList<Expression> expressions)
			{
				if (expression is BinaryOperatorExpression) {
					FlattenBinaryOperator((BinaryOperatorExpression)expression, expressions);
				}
				else {
					expressions.Add(expression);
				}
			}
			
			void CheckExpressionInAutoCallContext(Expression expression)
			{
				if (expression is InvocationExpression) {
					CheckInvocationInAutoCallContext((InvocationExpression)expression);
				}
			}
			
			void CheckInvocationInAutoCallContext(InvocationExpression invocationExpression)
			{
				var memberExpression = invocationExpression.Target as MemberReferenceExpression;
				if (memberExpression == null) {
					return;
				}
				var resolveResult = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (resolveResult == null) {
					return;
				}
				if (resolveResult.Member.Name != "ToString") {
					return;
				}
				AddRedundantToStringIssue(memberExpression, invocationExpression);
			}
			#endregion

			#region Invocation expressions
			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);

				var memberExpression = invocationExpression.Target as MemberReferenceExpression;
				if (memberExpression == null) {
					return;
				}
				var targetResolveResult = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (targetResolveResult == null) {
					return;
				}
				if (targetResolveResult.TargetResult.Type == stringType && targetResolveResult.Member.Name == "ToString") {
					AddRedundantToStringIssue(memberExpression, invocationExpression);
				}
				IMember member = targetResolveResult.Member;
				if (member.IsOverride) {
					member = InheritanceHelper.GetBaseMember(member);
					if (member == null) {
						return;
					}
				}
				var key = new Tuple<string, int>(member.ReflectionName, invocationExpression.Arguments.Count);
				Tuple<int, int> checkInfo;
				if (!membersCallingToString.TryGetValue(key, out checkInfo)) {
					return;
				}
				var arguments = invocationExpression.Arguments.ToList();
				for (int i = checkInfo.Item1; i < Math.Min(invocationExpression.Arguments.Count, checkInfo.Item2 + 1); ++i) {
					CheckExpressionInAutoCallContext(arguments[i]);
				}
			}
			#endregion
		}
	}
}

