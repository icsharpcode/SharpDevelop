//
// ParameterCouldBeDeclaredWithBaseTypeIssue.cs
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using System;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("A parameter can be demoted to base class",
	                   Description = "Finds parameters that can be demoted to a base class.",
	                   Category = IssueCategories.Opportunities,
	                   Severity = Severity.Suggestion)]
	public class ParameterCanBeDemotedIssue : ICodeIssueProvider
	{
		#region ICodeIssueProvider implementation
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			var sw = new Stopwatch();
			sw.Start();
			var gatherer = new GatherVisitor(context, this);
			var issues = gatherer.GetIssues().ToList();
			sw.Stop();
			Console.WriteLine("Elapsed time for ParameterCanBeDemotedIssue: {0} (resolved {2} method bodies in file '{1}')", sw.Elapsed, context.ParsedFile.FileName, gatherer.MethodResolveCount);
			return issues;
		}
		#endregion

		class GatherVisitor : GatherVisitorBase
		{
			readonly BaseRefactoringContext context;
			
			public GatherVisitor(BaseRefactoringContext context, ParameterCanBeDemotedIssue inspector) : base (context)
			{
				this.context = context;
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				base.VisitMethodDeclaration(methodDeclaration);

				var declarationResolveResult = context.Resolve(methodDeclaration) as MemberResolveResult;
				if (declarationResolveResult == null)
					return;
				var member = declarationResolveResult.Member;
				if (member.IsOverride || member.IsOverridable)
					return;

				var collector = new TypeCriteriaCollector(context);
				methodDeclaration.AcceptVisitor(collector);
				
				foreach (var parameter in methodDeclaration.Parameters) {
					ProcessParameter(parameter, methodDeclaration.Body, collector);
				}
			}

			void ProcessParameter(ParameterDeclaration parameter, BlockStatement body, TypeCriteriaCollector collector)
			{
				var directionExpression = parameter.Parent as DirectionExpression;
				if (directionExpression != null && directionExpression.FieldDirection != FieldDirection.None)
					// That kind of dependency is out of our control. Better not mess with it.
					return;

				var localResolveResult = context.Resolve(parameter) as LocalResolveResult;
				var currentType = localResolveResult.Type;
				var candidateTypes = localResolveResult.Type.GetAllBaseTypes();
				var criterion = collector.GetCriterion(localResolveResult.Variable);
				if (criterion == null)
					// No usages in body
					return;

				var possibleTypes = 
					from type in candidateTypes
					where criterion.SatisfiedBy(type) && TypeChangeResolvesCorrectly(parameter, body, type)
					orderby GetInheritanceDepth(type) ascending
					select type;

				var suggestedTypes = possibleTypes.Where(type => !type.Equals(currentType));
				if (suggestedTypes.Any()) {
					AddIssue(parameter, context.TranslateString("Parameter can be demoted to base class"), GetActions(parameter, suggestedTypes));
				}
			}

			internal int MethodResolveCount = 0;

			bool TypeChangeResolvesCorrectly(ParameterDeclaration parameter, BlockStatement body, IType type)
			{
				MethodResolveCount++;
				var resolver = context.GetResolverStateBefore(body);
				resolver.AddVariable(new DefaultParameter(type, parameter.Name));
				var astResolver = new CSharpAstResolver(resolver, body, context.ParsedFile);
				var validator = new TypeChangeValidationNavigator();
				astResolver.ApplyNavigator(validator, context.CancellationToken);
				return !validator.FoundErrors;
			}

			IEnumerable<CodeAction> GetActions(ParameterDeclaration parameter, IEnumerable<IType> possibleTypes)
			{
				var csResolver = context.Resolver.GetResolverStateBefore(parameter);
				var astBuilder = new TypeSystemAstBuilder(csResolver);
				foreach (var type in possibleTypes) {
					var localType = type;
					var message = string.Format(context.TranslateString("Demote parameter to '{0}'"), type.FullName);
					yield return new CodeAction(message, script => {
						script.Replace(parameter.Type, astBuilder.ConvertType(localType));
					});
				}
			}

			int GetInheritanceDepth(IType declaringType)
			{
				var depth = 0;
				foreach (var baseType in declaringType.DirectBaseTypes) {
					var newDepth = GetInheritanceDepth(baseType);
					depth = Math.Max(depth, newDepth);
				}
				return depth;
			}
		}

		class TypeChangeValidationNavigator : IResolveVisitorNavigator
		{
			public bool FoundErrors { get; private set; }

			#region IResolveVisitorNavigator implementation
			public ResolveVisitorNavigationMode Scan(AstNode node)
			{
				return ResolveVisitorNavigationMode.Resolve;
			}
			public void Resolved(AstNode node, ResolveResult result)
			{
				FoundErrors |= result.IsError;
			}
			public void ProcessConversion(Expression expression, ResolveResult result, Conversion conversion, IType targetType)
			{
				// no-op
			}
			#endregion
			
		}
	}
}

