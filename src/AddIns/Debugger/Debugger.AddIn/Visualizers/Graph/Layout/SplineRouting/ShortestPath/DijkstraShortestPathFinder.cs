// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Finds shortest paths in the <see cref="RouteGraph" />.
	/// </summary>
	public class DijkstraShortestPathFinder
	{
		RouteGraph graph;
		
		public DijkstraShortestPathFinder(RouteGraph routeGraph)
		{
			this.graph = routeGraph;
		}
		
		public List<RouteVertex> FindShortestPath(RouteVertex start, RouteVertex end)
		{
			start.IsEdgeEndpoint = false;
			end.IsEdgeEndpoint = false;
			foreach (var vertex in this.graph.Vertices) {
				vertex.Reset();
			}
			start.Distance = 0;
			bool reached = false;
			while (!reached)
			{
				RouteVertex minVertex = null;
				foreach (var minCandidate in graph.Vertices.Where(v => !v.IsPermanent && v.IsAvailable)) {
					if (minVertex == null || minCandidate.Distance < minVertex.Distance) {
						minVertex = minCandidate;
					}
				}
				minVertex.IsPermanent = true;
				if (minVertex == end) {
					reached = true;
				}
				foreach (var edge in minVertex.Neighbors) {
					double newDist = minVertex.Distance + edge.Length + edge.EndVertex.Penalization;
					if (newDist < edge.EndVertex.Distance) {
						edge.EndVertex.Distance = newDist;
						edge.EndVertex.Predecessor = minVertex;
					}
				}
			}
			List<RouteVertex> path = new List<RouteVertex>();
			RouteVertex pathVertex = end;
			while (pathVertex != start) {
				if (pathVertex == null)
					break;
				path.Add(pathVertex);
				//pathVertex.Penalization += 16;	// penalize path vertices so that next path tend to choose different vertices
				pathVertex = pathVertex.Predecessor;
			}
			path.Add(start);
			start.IsEdgeEndpoint = true;
			end.IsEdgeEndpoint = true;
			path.Reverse();
			return path;
		}
	}
}
