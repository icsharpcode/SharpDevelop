// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
		
		public NestedNode AddChild(NestedNode child)
		{
			this.children.Add(child);
			return child;
		}
		
		public AbstractNode()
		{
		}
		
		/// <summary>
		/// Returns properties nodes from this tree.
		/// </summary>
		public IEnumerable<PropertyNode> FlattenPropertyNodes()
		{
			return Utils.TreeFlattener.Flatten(this).Where((node) => { return node is PropertyNode; }).Cast<PropertyNode>();
		}
	}
}
