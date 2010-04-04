// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Description of AStarShortestPathFinder.
	/// </summary>
	public class AStarShortestPathFinder
	{
		RouteGraph graph;
		
		public AStarShortestPathFinder(RouteGraph routeGraph)
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
					if (minVertex.Distance + edge.Length < edge.EndVertex.Distance) {
						edge.EndVertex.Distance = minVertex.Distance + edge.Length;
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
				//pathVertex.IsUsed = true;
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
