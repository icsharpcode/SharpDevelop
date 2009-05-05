// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Graph with positioned nodes and edges.
	/// </summary>
	public class PositionedGraph
	{
		internal List<PositionedNode> nodes = new List<PositionedNode>();
		
		/// <summary>
		/// All nodes in the graph.
		/// </summary>
		public IEnumerable<PositionedNode> Nodes
		{
			get { return nodes; }
		}
		
		/// <summary>
		/// All edges in the graph.
		/// </summary>
		public IEnumerable<PositionedEdge> Edges
		{
			get
			{
				foreach	(PositionedNode node in this.Nodes)
				{
					foreach (PositionedEdge edge in node.Edges)
					{
						yield return edge;
					}
				}
			}
		}
	}
}
