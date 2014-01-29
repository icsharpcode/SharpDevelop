// 
// OptionalParameterValueIssueMismatch.cs
// 
// Author:
//      Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis
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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Mismatch optional parameter value in overridden method",
	                   Description = "The value of an optional parameter in a method does not match the base method.",
	                   Category = IssueCategories.CodeQualityIssues,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "OptionalParameterHierarchyMismatch")]
	public class OptionalParameterHierarchyMismatchIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<OptionalParameterHierarchyMismatchIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			//Delegate declarations are not visited even though they can have optional
			//parameters because they can not be overriden.

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				VisitParameterizedEntityDeclaration("method", methodDeclaration, methodDeclaration.Parameters);
			}

			void VisitParameterizedEntityDeclaration(string memberType, EntityDeclaration entityDeclaration, AstNodeCollection<ParameterDeclaration> parameters)
			{
				// Ignore explicit interface implementations (those should have no optional parameters as there can't be any direct calls) 
				if (!entityDeclaration.GetChildByRole(EntityDeclaration.PrivateImplementationTypeRole).IsNull)
					return;
				//Override is not strictly necessary because methodDeclaration
				//might still implement an interface member
				var memberResolveResult = ctx.Resolve(entityDeclaration) as MemberResolveResult;
				if (memberResolveResult == null) {
					return;
				}
				var member = (IParameterizedMember)memberResolveResult.Member;
				var baseMembers = InheritanceHelper.GetBaseMembers(member, true).ToList();
				foreach (IParameterizedMember baseMember in baseMembers) {
					if (baseMember.IsOverride || baseMember.DeclaringType.Kind == TypeKind.Interface)
						continue;
					CompareMethods(memberType, parameters, member, baseMember);
					return;
				}
				// only check 1 interface method -> multiple interface implementations could lead to deault value conflicts
				// possible other solutions: Skip the interface check entirely
				var interfaceBaseMethods = baseMembers.Where(b => b.DeclaringType.Kind == TypeKind.Interface).ToList();
				if (interfaceBaseMethods.Count == 1) {
					foreach (IParameterizedMember baseMember in interfaceBaseMethods) {
						if (baseMember.DeclaringType.Kind == TypeKind.Interface) {
							CompareMethods(memberType, parameters, member, baseMember);
						}
					}
				}
			}

			static Expression CreateDefaultValueExpression(BaseRefactoringContext ctx, AstNode node, IType type, object constantValue)
			{
				var astBuilder = ctx.CreateTypeSystemAstBuilder(node);
				return astBuilder.ConvertConstantValue(type, constantValue); 
			}

			void CompareMethods(string memberType, AstNodeCollection<ParameterDeclaration> parameters, IParameterizedMember overridenMethod, IParameterizedMember baseMethod)
			{
				var parameterEnumerator = parameters.GetEnumerator();
				for (int parameterIndex = 0; parameterIndex < overridenMethod.Parameters.Count; parameterIndex++) {
					parameterEnumerator.MoveNext();

					var baseParameter = baseMethod.Parameters [parameterIndex];

					var overridenParameter = overridenMethod.Parameters [parameterIndex];

					string parameterName = overridenParameter.Name;
					var parameterDeclaration = parameterEnumerator.Current;

					if (overridenParameter.IsOptional) {
						if (!baseParameter.IsOptional) {
							AddIssue(new CodeIssue(parameterDeclaration,
							         string.Format(ctx.TranslateString("Optional parameter value {0} differs from base " + memberType + " '{1}'"), parameterName, baseMethod.DeclaringType.FullName),
							         ctx.TranslateString("Remove parameter default value"),
							         script => {
								script.Remove(parameterDeclaration.AssignToken);
								script.Remove(parameterDeclaration.DefaultExpression);
								script.FormatText(parameterDeclaration);
								}));
						} else if (!object.Equals(overridenParameter.ConstantValue, baseParameter.ConstantValue)) {
							AddIssue(new CodeIssue(parameterDeclaration,
							         string.Format(ctx.TranslateString("Optional parameter value {0} differs from base " + memberType + " '{1}'"), parameterName, baseMethod.DeclaringType.FullName),
							         string.Format(ctx.TranslateString("Change default value to {0}"), baseParameter.ConstantValue),
								script => script.Replace(parameterDeclaration.DefaultExpression, CreateDefaultValueExpression(ctx, parameterDeclaration, baseParameter.Type, baseParameter.ConstantValue))));
						}
					} else {
						if (!baseParameter.IsOptional)
							continue;
						AddIssue(new CodeIssue(parameterDeclaration,
							string.Format(ctx.TranslateString("Parameter {0} has default value in base method '{1}'"), parameterName, baseMethod.FullName),
							string.Format(ctx.TranslateString("Add default value from base '{0}'"), CreateDefaultValueExpression(ctx, parameterDeclaration, baseParameter.Type, baseParameter.ConstantValue)),
							script => {
								var newParameter = (ParameterDeclaration)parameterDeclaration.Clone();
								newParameter.DefaultExpression = CreateDefaultValueExpression(ctx, parameterDeclaration, baseParameter.Type, baseParameter.ConstantValue);
								script.Replace(parameterDeclaration, newParameter);
							}
						));
					}
				}
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
				VisitParameterizedEntityDeclaration("indexer", indexerDeclaration, indexerDeclaration.Parameters);
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				//No need to visit statements
			}
		}
	}
}

