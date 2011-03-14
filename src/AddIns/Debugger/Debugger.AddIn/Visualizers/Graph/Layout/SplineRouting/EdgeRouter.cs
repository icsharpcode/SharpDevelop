// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Description of EdgeRouter.
	/// </summary>
	public class EdgeRouter
	{
		public EdgeRouter()
		{
		}
		
		public Dictionary<TEdge, RoutedEdge> RouteEdges<TEdge>(IEnumerable<IRect> nodes, IEnumerable<TEdge> edges) where TEdge : class, IEdge
		{
			var routeGraph = RouteGraph.InitializeVertices(nodes, edges);
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
