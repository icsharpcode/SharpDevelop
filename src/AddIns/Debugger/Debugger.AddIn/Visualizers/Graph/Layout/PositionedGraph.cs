// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Graph with positioned nodes and edges.
	/// </summary>
	public class PositionedGraph
	{
		private List<PositionedNode> nodes = new List<PositionedNode>();
		
		public PositionedNode Root { get; set; }
		
		public System.Windows.Rect BoundingRect
		{
			get
			{
				double minX = nodes.Select(node => node.Left).Min();
				double maxX = nodes.Select(node => node.Left + node.Width).Max();
				double minY = nodes.Select(node => node.Top).Min();
				double maxY = nodes.Select(node => node.Top + node.Height).Max();
				
				return new Rect(minX, minY, maxX - minX, maxY - minY);
			}
		}
		
		/// <summary>
		/// All nodes in the graph.
		/// </summary>
		public IEnumerable<PositionedNode> Nodes
		{
			get { return nodes; }
		}
		
		internal void AddNode(PositionedNode node)
		{
			this.nodes.Add(node);
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
