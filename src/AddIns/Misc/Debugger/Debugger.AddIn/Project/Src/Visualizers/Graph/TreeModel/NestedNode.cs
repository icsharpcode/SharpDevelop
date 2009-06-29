// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Description of NestedNode.
	/// </summary>
	public class NestedNode : AbstractNode
	{
		private List<AbstractNode> children = new List<AbstractNode>();
		
		private NestedNodeType nodeType;
		public NestedNodeType NodeType
		{
			get { return nodeType; }
		}
		
		public IEnumerable<AbstractNode> Children
		{
			get 
			{
				foreach (var child in this.children) 
					yield return child;
			}
		}
		
		public void AddChild(AbstractNode child)
		{
			this.children.Add(child);
		}
		
		public NestedNode(NestedNodeType nodeType)
		{
			this.nodeType = nodeType;
		}
	}
}
