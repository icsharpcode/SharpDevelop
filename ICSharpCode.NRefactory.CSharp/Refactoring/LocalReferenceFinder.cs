//
// LocalReferenceFinder.cs
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
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using ICSharpCode.NRefactory.Utils;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{

	/// <summary>
	/// Finds references to <see cref="IVariable">IVariables</see>.
	/// </summary>
	/// <remarks>
	/// This class is more efficient than <see cref="FindReferences"/>
	/// if there is already a resolved tree or if multiple searches needs
	/// to be performed.
	/// </remarks>		
	public class LocalReferenceFinder
	{
		LocalReferenceLocator locator;

		MultiDictionary<IVariable, Tuple<AstNode, LocalResolveResult>> references = new MultiDictionary<IVariable, Tuple<AstNode, LocalResolveResult>>();
		
		public LocalReferenceFinder(CSharpAstResolver resolver)
		{
			locator = new LocalReferenceLocator(resolver, this);
		}

		public LocalReferenceFinder(BaseRefactoringContext context) : this(context.Resolver)
		{
		}

		HashSet<AstNode> visitedRoots = new HashSet<AstNode>();

		void VisitIfNeccessary(AstNode rootNode)
		{
			// If any of the parent nodes are recorded as visited,
			// we don't need to traverse rootNode this time.
			var tmpRoot = rootNode;
			while(tmpRoot != null){
				if (visitedRoots.Contains(tmpRoot))
					return;
				tmpRoot = tmpRoot.Parent;
			}

			rootNode.AcceptVisitor(locator);
			visitedRoots.Add(rootNode);
		}

		/// <summary>
		/// Finds the references to <paramref name="variable"/>.
		/// </summary>
		/// <param name='rootNode'>
		/// Root node for the search.
		/// </param>
		/// <param name='variable'>
		/// The variable to find references for.
		/// </param>
		/// <param name='action'>
		/// Will be called for each reference found.
		/// </param>
		/// <remarks>
		/// When a single <see cref="LocalReferenceFinder"/> is reused for multiple
		/// searches, which references outside of <paramref name="rootNode"/> are
		/// or are not reported is undefined.
		/// </remarks>
		public void FindReferences (AstNode rootNode, IVariable variable, FoundReferenceCallback action)
		{
			VisitIfNeccessary(rootNode);
			var lookup = (ILookup<IVariable, Tuple<AstNode, LocalResolveResult>>)references;
			if (!lookup.Contains(variable))
				return;
			var iList = references[variable];
			foreach (var reference in iList) {
				action(reference.Item1, reference.Item2);
			}
		}

		class LocalReferenceLocator : DepthFirstAstVisitor
		{
			CSharpAstResolver resolver;

			LocalReferenceFinder referenceFinder;

			public LocalReferenceLocator(CSharpAstResolver resolver, LocalReferenceFinder referenceFinder)
			{
				this.resolver = resolver;
				this.referenceFinder = referenceFinder;
			}

			IList<IVariable> processedVariables = new List<IVariable>();

			protected override void VisitChildren(AstNode node)
			{
				var localResolveResult = resolver.Resolve(node) as LocalResolveResult;
				if (localResolveResult != null && !processedVariables.Contains(localResolveResult.Variable)) {
					referenceFinder.references.Add(localResolveResult.Variable, Tuple.Create(node, localResolveResult));

					processedVariables.Add(localResolveResult.Variable);
					base.VisitChildren(node);
					Debug.Assert(processedVariables.Contains(localResolveResult.Variable), "Variable should still be in the list of processed variables.");
					processedVariables.Remove(localResolveResult.Variable);
				} else {
					base.VisitChildren(node);
				}
			}
		}
	}
}
