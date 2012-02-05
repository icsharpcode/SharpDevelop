// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
