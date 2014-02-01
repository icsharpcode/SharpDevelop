// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
