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
			get {
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
			get {
				foreach	(PositionedNode node in this.Nodes) {
					foreach (PositionedEdge edge in node.Edges) {
						yield return edge;
					}
				}
			}
		}
	}
}
