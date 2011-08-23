// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using Debugger.AddIn.Visualizers.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Description of AbstractNode.
	/// </summary>
	public class AbstractNode : Utils.ITreeNode<AbstractNode>
	{
		private List<AbstractNode> children = new List<AbstractNode>();
		
		public IEnumerable<AbstractNode> Children
		{
			get 
			{
				foreach (var child in this.children) 
					yield return child;
			}
		}
		
		public AbstractNode AddChild(AbstractNode child)
		{
			this.children.Add(child);
			return child;
		}
		
		public AbstractNode()
		{
		}
		
		/// <summary>
		/// Returns property nodes from this tree.
		/// </summary>
		public IEnumerable<PropertyNode> FlattenPropertyNodes()
		{
			return Utils.TreeFlattener.Flatten(this).OfType<PropertyNode>();
		}
	}
}
