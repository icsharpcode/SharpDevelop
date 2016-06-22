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

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Entry point to the edge routing algorithm.
	/// </summary>
	public class EdgeRouter
	{
		/// <summary>
		/// Calculates routes for edges in a graph, so that they avoid nodes.
		/// </summary>
		public Dictionary<TEdge, RoutedEdge> RouteEdges<TEdge>(IEnumerable<IRect> nodes, IEnumerable<TEdge> edges)
			where TEdge : class, IEdge
		{
			var routeGraph = RouteGraph.InitializeVertices(nodes, edges, 0, 0);
			var routedEdges = new Dictionary<TEdge, RoutedEdge>();
			var occludedEdges = new List<TEdge>();
			foreach (var edge in edges) {
				var straightEdge = routeGraph.TryRouteEdgeStraight(edge);
				if (straightEdge != null) {
					routedEdges[edge] = straightEdge;
				} else {
					occludedEdges.Add(edge);
				}
			}
			if (occludedEdges.Count > 0)	{
				// there are some edges that couldn't be routed as straight lines
				routeGraph.ComputeVisibilityGraph();
				foreach (var edge in occludedEdges) {
					RoutedEdge routedEdge = routeGraph.RouteEdge(edge);
					routedEdges[edge] = routedEdge;
				}
			}
			return routedEdges;
		}
	}
}
