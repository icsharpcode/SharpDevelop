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
			return new GatherVisitor(context, this).GetIssues();
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
					var localResolveResult = context.Resolve(parameter) as LocalResolveResult;
					var variable = localResolveResult.Variable;
					var currentType = localResolveResult.Type;
					var candidateTypes = localResolveResult.Type.GetAllBaseTypes();
					var typeCriterion = collector.GetCriterion(variable);
					if (typeCriterion == null)
						// No usages in body
						return;
					var possibleTypes = GetPossibleTypes(candidateTypes, typeCriterion);
					var suggestedTypes = possibleTypes.Where(t => t != currentType);
					if (suggestedTypes.Any()) {
						AddIssue(parameter, context.TranslateString("Parameter can be demoted to base class"),
						         GetActions(parameter, suggestedTypes));
					}
				}
			}

			IEnumerable<CodeAction> GetActions(ParameterDeclaration parameter, IEnumerable<IType> possibleTypes)
			{
				foreach (var type in possibleTypes) {
					var typeName = type.Name;
					var message = string.Format("{0} '{1}'", context.TranslateString("Demote parameter to "), typeName);
					yield return new CodeAction(message, script => {
						script.Replace(parameter.Type, new SimpleType(typeName));
					});
				}
			}
			
			IEnumerable<IType> GetPossibleTypes(IEnumerable<IType> types, ITypeCriterion criterion)
			{
				return from type in types
					where criterion.SatisfiedBy(type)
						orderby GetInheritanceDepth(type) ascending
						select type;
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
	}
}

