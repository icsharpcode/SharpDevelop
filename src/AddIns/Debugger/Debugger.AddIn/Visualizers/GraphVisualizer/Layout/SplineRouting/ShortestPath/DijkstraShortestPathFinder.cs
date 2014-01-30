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
			while (!reached) {
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
