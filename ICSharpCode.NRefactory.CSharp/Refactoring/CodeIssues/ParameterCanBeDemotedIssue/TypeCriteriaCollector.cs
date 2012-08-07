//
// CriteriaCollector.cs
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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	public class TypeCriteriaCollector : DepthFirstAstVisitor
	{
		BaseRefactoringContext context;
		
		public TypeCriteriaCollector(BaseRefactoringContext context)
		{
			this.context = context;
			TypeCriteria = new Dictionary<IVariable, IList<ITypeCriterion>>();
		}

		IDictionary<IVariable, IList<ITypeCriterion>> TypeCriteria { get; set; }

		public ITypeCriterion GetCriterion(IVariable variable)
		{
			if (!TypeCriteria.ContainsKey(variable))
				return null;
			return new ConjunctionCriteria(TypeCriteria[variable]);
		}

		void AddCriterion(IVariable variable, ITypeCriterion criterion)
		{
			if (!TypeCriteria.ContainsKey(variable))
				TypeCriteria[variable] = new List<ITypeCriterion>();
			TypeCriteria[variable].Add(criterion);
		}

		public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			base.VisitMemberReferenceExpression(memberReferenceExpression);

			var targetResolveResult = context.Resolve(memberReferenceExpression.Target) as LocalResolveResult;
			if (targetResolveResult == null)
				return;
			var variable = targetResolveResult.Variable;
			var conversion = context.GetConversion(memberReferenceExpression);
			if (conversion.IsMethodGroupConversion) {
				AddCriterion(variable, new HasMemberCriterion(conversion.Method));
			} else {
				var resolveResult = context.Resolve(memberReferenceExpression);
				var memberResolveResult = resolveResult as MemberResolveResult;
				if (memberResolveResult != null)
					AddCriterion(variable, new HasMemberCriterion(memberResolveResult.Member));
			}
		}

		public override void VisitIndexerExpression(IndexerExpression indexerExpression)
		{
			base.VisitIndexerExpression(indexerExpression);

			var localResolveResult = context.Resolve(indexerExpression.Target) as LocalResolveResult;
			if (localResolveResult == null)
				return;
			var resolveResult = context.Resolve(indexerExpression);
			if (localResolveResult == null)
				return;
			var parent = indexerExpression.Parent;
			while (parent is ParenthesizedExpression)
				parent = parent.Parent;
			if (parent is DirectionExpression) {
				// The only types which are indexable and where the indexing expression
				// results in a variable is an actual array type
				AddCriterion(localResolveResult.Variable, new IsArrayTypeCriterion());
			} else if (resolveResult is ArrayAccessResolveResult) {
				var arrayResolveResult = (ArrayAccessResolveResult)resolveResult;
				var parameterTypes = arrayResolveResult.Indexes.Select(index => index.Type);
				var arrayType = arrayResolveResult.Array.Type as ArrayType;
				if (arrayType == null)
					return;
				var criterion = new SupportsIndexingCriterion(arrayType.ElementType, parameterTypes, CSharpConversions.Get(context.Compilation));
				AddCriterion(localResolveResult.Variable, criterion);
			} else if (resolveResult is CSharpInvocationResolveResult) {
				var invocationResolveResult = (CSharpInvocationResolveResult)resolveResult;
				var parameterTypes = invocationResolveResult.Arguments.Select(arg => arg.Type);
				var criterion = new SupportsIndexingCriterion(invocationResolveResult.Member.ReturnType, parameterTypes, CSharpConversions.Get(context.Compilation));
				AddCriterion(localResolveResult.Variable, criterion);
			}
		}

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			base.VisitInvocationExpression(invocationExpression);

			var resolveResult = context.Resolve(invocationExpression);
			var invocationResolveResult = resolveResult as InvocationResolveResult;
			if (invocationResolveResult == null)
				return;

			// invocationExpression.Target resolves to a method group and VisitMemberReferenceExpression
			// only handles members, so handle method groups here
			var targetResolveResult = invocationResolveResult.TargetResult as LocalResolveResult;
			if (targetResolveResult != null) {
				var variable = targetResolveResult.Variable;
				AddCriterion(variable, new HasMemberCriterion(invocationResolveResult.Member));
			}
		}

		protected override void VisitChildren(AstNode node)
		{
			if (node is Expression) {
				CollectRestrictionsFromNodes((Expression)node);
			}
			base.VisitChildren(node);
		}

		void CollectRestrictionsFromNodes(Expression expression)
		{
			var role = expression.Role;
			if (role != Roles.Expression && role != Roles.Argument)
				return;
			var resolveResult = context.Resolve(expression);
			var localResolveResult = resolveResult as LocalResolveResult;
			if (localResolveResult == null)
				return;
			var expectedType = context.GetExpectedType(expression);
			AddCriterion(localResolveResult.Variable, new IsTypeCriterion(expectedType));
		}

		class ConjunctionCriteria : ITypeCriterion
		{
			IList<ITypeCriterion> criteria;

			public ConjunctionCriteria(IList<ITypeCriterion> criteria)
			{
				this.criteria = criteria;
			}
			
			public bool SatisfiedBy(IType type)
			{
				foreach (var criterion in criteria) {
					if (!criterion.SatisfiedBy(type)) {
						return false;
					}
				}
				return true;
			}
		}
	}
}

