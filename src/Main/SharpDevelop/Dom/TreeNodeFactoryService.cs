// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class TreeNodeFactoryService : ITreeNodeFactory
	{
		readonly IReadOnlyList<ITreeNodeFactory> factories;
		
		public TreeNodeFactoryService()
		{
			factories = SD.AddInTree.BuildItems<ITreeNodeFactory>("/SharpDevelop/TreeNodeFactories", this, false);
		}
		
		public TreeNodeFactoryService(IReadOnlyList<ITreeNodeFactory> factories)
		{
			if (factories == null)
				throw new ArgumentNullException("factories");
			this.factories = factories;
		}
		
		Tuple<Type, ITreeNodeFactory> FindFactoryFor(object model)
		{
			if (model == null)
				return null;
			var tuples = new List<Tuple<Type, ITreeNodeFactory>>();
			foreach (var factory in factories) {
				Type type = factory.GetSupportedType(model);
				// Consider the new factory only if no existing type is derived from it:
				if (type != null && !tuples.Any(t => IsDerivedFrom(t.Item1, type))) {
					// Remove all existing candidates from which the new type is derived
					tuples.RemoveAll(t => IsDerivedFrom(type, t.Item1));
					// Add the new candidate
					tuples.Add(Tuple.Create(type, factory));
				}
			}
			// Use the surviving candidate that is first in the AddIn-Tree
			return tuples.FirstOrDefault();
		}
		
		/// <summary>
		/// Returns true if derivedType is derived from baseType, and the types are not equal.
		/// </summary>
		bool IsDerivedFrom(Type derivedType, Type baseType)
		{
			return derivedType != baseType && baseType.IsAssignableFrom(derivedType);
		}
		
		public Type GetSupportedType(object model)
		{
			var tuple = FindFactoryFor(model);
			if (tuple != null)
				return tuple.Item1;
			else
				return null;
		}
		
		public SharpTreeNode CreateTreeNode(object model)
		{
			var tuple = FindFactoryFor(model);
			if (tuple != null)
				return tuple.Item2.CreateTreeNode(model);
			else
				return null;
		}
	}
}
