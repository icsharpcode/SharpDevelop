// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
		
		public void RouteEdges(PositionedGraph posGraph)
		{
			Dictionary<PositionedEdge, RoutedEdge> routedEdges = router.RouteEdges(posGraph.Nodes, posGraph.Edges);
			foreach (var edgePair in routedEdges) {
				SetEdgeSplinePoints(edgePair.Key, edgePair.Value);
			}
		}
		
		void SetEdgeSplinePoints(PositionedEdge edge, RoutedEdge routedEdge)
		{
			foreach (Point2D point in routedEdge.SplinePoints) {
				edge.SplinePoints.Add(new Point(point.X, point.Y));
			}
		}
	}
}
