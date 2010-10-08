// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Util;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Contains extension methods for the type system.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Gets all base types.
		/// </summary>
		/// <remarks>This is the reflexive and transitive closure of <see cref="IType.GetBaseTypes"/>.
		/// Note that this method does not return all supertypes - doing so is impossible due to contravariance
		/// (and underisable for covariance and the list could become very large).
		/// This method may return an infinite list for certain (invalid) class declarations like <c>class C{T} : C{C{T}}</c>
		/// TODO: we could ensure a finite list by filtering out cyclic inheritance
		/// </remarks>
		public static IEnumerable<IType> GetAllBaseTypes(this IType type, ITypeResolveContext context)
		{
			// Given types as nodes and GetBaseTypes() as edges, the type hierarchy forms a graph.
			// This method should return all nodes reachable from the given start node.
			
			// We perform this operation by converting the graph into a tree by making sure we return each node at most once.
			// Then we convert the tree into a flat list using the Flatten operation.
			
			HashSet<IType> visited = new HashSet<IType>();
			visited.Add(type);
			return TreeTraversal.PreOrder(type, t => t.GetBaseTypes(context).Where(visited.Add));
		}
	}
}
