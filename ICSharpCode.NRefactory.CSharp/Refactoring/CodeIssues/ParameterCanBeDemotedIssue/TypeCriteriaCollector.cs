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
		
		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			base.VisitInvocationExpression(invocationExpression);
			var resolveResult = context.Resolve(invocationExpression);
			var invocationResolveResult = resolveResult as InvocationResolveResult;
			if (invocationResolveResult == null)
				return;
			var parameterResolveResult = invocationResolveResult.TargetResult as LocalResolveResult;
			if (parameterResolveResult == null)
				return;
			var variable = parameterResolveResult.Variable;
			if (!TypeCriteria.ContainsKey(variable))
				TypeCriteria[variable] = new List<ITypeCriterion>();
			TypeCriteria[variable].Add(new HasMemberCriterion(invocationResolveResult.Member));
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

