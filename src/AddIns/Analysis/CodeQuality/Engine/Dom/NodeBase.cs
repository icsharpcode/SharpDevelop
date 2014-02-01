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
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public abstract class NodeBase
	{
		public abstract string Name { get; }
		public abstract IList<NodeBase> Children { get; }
		public NodeBase Parent { get; private set; }
		
		public static readonly ReadOnlyCollection<NodeBase> EmptyChildren = new ReadOnlyCollection<NodeBase>(Enumerable.Empty<NodeBase>().ToList());
		
		internal IEnumerable<NodeBase> AncestorsAndSelf {
			get {
				var node = this;
				while (node != null) {
					yield return node;
					node = node.Parent;
				}
			}
		}
		
		protected Dictionary<NodeBase, int> relationships = new Dictionary<NodeBase, int>();
		
		public void AddRelationship(NodeBase reference)
		{
			if (!relationships.ContainsKey(reference))
				relationships[reference] = 0;
			relationships[reference]++;
		}
		
		public void AddChild(NodeBase child)
		{
			child.Parent = this;
			Children.Add(child);
		}
		
		public int GetUses(NodeBase value)
		{
			if (this == value)
				return -1;
			int uses;
			if (relationships.TryGetValue(value, out uses))
				return uses;
			return 0;
		}
	}
}
