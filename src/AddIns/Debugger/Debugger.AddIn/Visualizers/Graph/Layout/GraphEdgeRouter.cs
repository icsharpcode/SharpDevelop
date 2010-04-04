// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Debugger.AddIn.Visualizers.Graph.SplineRouting;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Adapts generic <see cref="EdgeRouter" /> to work with <see cref="PositionedGraph" />.
	/// </summary>
	public class GraphEdgeRouter
	{
		EdgeRouter router = new EdgeRouter();
		
		public GraphEdgeRouter()
		{
		}
		
		public PositionedGraph RouteEdges(PositionedGraph posGraph)
		{
			List<RoutedEdge> routedEdges = router.RouteEdges(posGraph.Nodes, posGraph.Edges);
			int i = 0;
			// assume routedEdges come in the same order as posGraph.Edges
			foreach (var edge in posGraph.Edges) {
				SetEdgeSplinePoints(edge, routedEdges[i]);
				i++;
			}
			return posGraph;
		}
		
		void SetEdgeSplinePoints(PositionedEdge edge, RoutedEdge routedEdge)
		{
			foreach (Point2D point in routedEdge.SplinePoints) {
				edge.SplinePoints.Add(new Point(point.X, point.Y));
			}
		}
	}
}
