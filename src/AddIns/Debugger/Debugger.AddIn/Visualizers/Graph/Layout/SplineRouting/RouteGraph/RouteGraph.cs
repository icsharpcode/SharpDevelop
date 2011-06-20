// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// The visibility graph in which <see cref="EdgeRouter" /> searches shortest paths.
	/// Vertices of the graph are corners of boxes in the original graph.
	/// Edges connect vertices which can be connected by a line without intersecting boxes.
	/// </summary>
	public class RouteGraph
	{
		static readonly double boxPadding = 15;
		//static readonly double boxSafetyMargin = 5;	// inflate boxes for collision testing
		static readonly double multiEdgeGap = 15;
		
		List<IRect> boxes = new List<IRect>();
		public List<IRect> Boxes {
			get { return boxes; }
		}
		
		List<RouteVertex> vertices = new List<RouteVertex>();
		public List<RouteVertex> Vertices {
			get { return vertices; }
		}
		
		DijkstraShortestPathFinder pathFinder;
		
		public RouteGraph()
		{
			pathFinder = new DijkstraShortestPathFinder(this);
		}
		
		/// <summary>
		/// Initializes the RouteGraph by vertices close to corners of all nodes.
		/// </summary>
		/// <param name="boundX">X coordinates of vertices cannot be lower than this value (so that edges stay in boundaries).</param>
		/// <param name="boundY">Y coordinates of vertices cannot be lower than this value (so that edges stay in boundaries).</param>
		public static RouteGraph InitializeVertices(IEnumerable<IRect> nodes, IEnumerable<IEdge> edges, int boundX, int boundY)
		{
			var graph = new RouteGraph();
			// add vertices for node corners
			foreach (var node in nodes) {
				graph.Boxes.Add(node);
				foreach (var vertex in GetRectCorners(node, boxPadding)) {
					if (vertex.X >= boundX && vertex.Y >= boundY) {
						graph.Vertices.Add(vertex);
					}
				}
			}
			// add vertices for egde endpoints
			foreach (var multiEdgeGroup in edges.GroupBy(edge => GetStartEnd(edge))) {
				int multiEdgeCount = multiEdgeGroup.Count();
				IRect fromRect = multiEdgeGroup.First().From;
				IRect toRect = multiEdgeGroup.First().To;
				var sourceCenter = GeomUtils.RectCenter(fromRect);
				var targetCenter = GeomUtils.RectCenter(toRect);
				if (Math.Abs(sourceCenter.X - targetCenter.X) > Math.Abs(sourceCenter.Y - targetCenter.Y) ||
				    (fromRect == toRect))
				{
					// the line is horizontal
					double multiEdgeSpanSource = GetMultiEdgeSpan(fromRect.Height, multiEdgeCount, multiEdgeGap);
					double multiEdgeSpanTarget = GetMultiEdgeSpan(toRect.Height, multiEdgeCount, multiEdgeGap);
					double originSourceCurrentY = sourceCenter.Y - multiEdgeSpanSource / 2;
					double originTargetCurrentY = targetCenter.Y - multiEdgeSpanTarget / 2;
					foreach (var edge in multiEdgeGroup) {
						Point2D sourceOrigin = new Point2D(sourceCenter.X, originSourceCurrentY);
						Point2D targetOrigin = new Point2D(targetCenter.X, originTargetCurrentY);
						// Here user could provide custom edgeStart and edgeEnd
						// inflate boxes a little so that edgeStart and edgeEnd are a little outside of the box (to prevent floating point errors)
						if (edge.From == edge.To) {
							// special case - self edge
							var edgeStart = new Point2D(fromRect.Left + fromRect.Width + 0.01, originSourceCurrentY);
							var edgeEnd = new Point2D(fromRect.Left + fromRect.Width / 2, fromRect.Top);
							graph.AddEdgeEndpointVertices(edge, edgeStart, edgeEnd);
						} else {
							var edgeStart = GeomUtils.LineRectIntersection(sourceOrigin, targetOrigin, edge.From.Inflated(1e-3));
							var edgeEnd = GeomUtils.LineRectIntersection(sourceOrigin, targetOrigin, edge.To.Inflated(1e-3));
							graph.AddEdgeEndpointVertices(edge, edgeStart, edgeEnd);
						}
						originSourceCurrentY += multiEdgeSpanSource / (multiEdgeCount - 1);
						originTargetCurrentY += multiEdgeSpanTarget / (multiEdgeCount - 1);
					}
				}
				else
				{
					// the line is vertical
					double multiEdgeSpanSource = GetMultiEdgeSpan(fromRect.Width, multiEdgeCount, multiEdgeGap);
					double multiEdgeSpanTarget = GetMultiEdgeSpan(toRect.Width, multiEdgeCount, multiEdgeGap);
					double originSourceCurrentX = sourceCenter.X - multiEdgeSpanSource / 2;
					double originTargetCurrentX = targetCenter.X - multiEdgeSpanTarget / 2;
					foreach (var edge in multiEdgeGroup) {
						Point2D sourceOrigin = new Point2D(originSourceCurrentX, sourceCenter.Y);
						Point2D targetOrigin = new Point2D(originTargetCurrentX, targetCenter.Y);
						// Here user could provide custom edgeStart and edgeEnd
						// inflate boxes a little so that edgeStart and edgeEnd are a little outside of the box (to prevent floating point errors)
						var edgeStart = GeomUtils.LineRectIntersection(sourceOrigin, targetOrigin, edge.From.Inflated(1e-3));
						var edgeEnd = GeomUtils.LineRectIntersection(sourceOrigin, targetOrigin, edge.To.Inflated(1e-3));
						graph.AddEdgeEndpointVertices(edge, edgeStart, edgeEnd);
						originSourceCurrentX += multiEdgeSpanSource / (multiEdgeCount - 1);
						originTargetCurrentX += multiEdgeSpanTarget / (multiEdgeCount - 1);
					}
				}
			}
			return graph;
		}
		
		void AddEdgeEndpointVertices(IEdge edge, Point2D? edgeStart, Point2D? edgeEnd)
		{
			if (edgeStart == null || edgeEnd == null) {
				// should not happen
				throw new System.Exception("The line between box centers does not intersect the boxes!");
			}
			var startPoint = new RouteVertex(edgeStart.Value.X, edgeStart.Value.Y);
			startPoint.IsEdgeEndpoint = true;
			var endPoint = new RouteVertex(edgeEnd.Value.X, edgeEnd.Value.Y);
			endPoint.IsEdgeEndpoint = true;
			this.vertices.Add(startPoint);
			this.vertices.Add(endPoint);
			// remember what RouteVertices we created for this user edge
			this.setStartVertex(edge, startPoint);
			this.setEndVertex(edge, endPoint);
		}
		
		static IEnumerable<RouteVertex> GetRectCorners(IRect rect, double padding)
		{
			double left = rect.Left - padding;
			double top = rect.Top - padding;
			double right = left + rect.Width + 2 * padding;
			double bottom = top + rect.Height + 2 * padding;
			yield return new RouteVertex(left, top);
			yield return new RouteVertex(right, top);
			yield return new RouteVertex(right, bottom);
			yield return new RouteVertex(left, bottom);
		}
		
		public void ComputeVisibilityGraph()
		{
			for (int i = 0; i < this.Vertices.Count; i++) {
				for (int j = i + 1; j < this.Vertices.Count; j++) {
					var vertex = this.Vertices[i];
					var vertex2 = this.Vertices[j];
					if (Visible(vertex, vertex2))
					{
						// bidirectional edge
						vertex.AddNeighbor(vertex2);
						vertex2.AddNeighbor(vertex);
					}
				}
			}
		}
		
		public bool Visible(IPoint vertex, IPoint vertex2)
		{
			// test for intersection with every box
			foreach (var rect in this.Boxes) {
				if (GeomUtils.LineRectIntersection(vertex, vertex2, rect) != null)
					return false;
			}
			return true;
		}
		
		public RoutedEdge RouteEdge(IEdge edge)
		{
			var pathVertices = pathFinder.FindShortestPath(getStartVertex(edge), getEndVertex(edge));
			return BuildRoutedEdge(pathVertices);
		}
		
		public RoutedEdge TryRouteEdgeStraight(IEdge edge)
		{
			var startVertex = getStartVertex(edge);
			var endVertex = getEndVertex(edge);
			if (Visible(startVertex, endVertex)) {
				// route the edge straight
				return BuildRoutedEdge(new [] {startVertex, endVertex });
			} else
				return null;
		}
		
		public RoutedEdge BuildRoutedEdge(IEnumerable<IPoint> points)
		{
			var routedEdge = new RoutedEdge();
			foreach (var point in points) {
				routedEdge.Points.Add(new Point2D(point.X, point.Y));
			}
			return routedEdge;
		}
		
		Dictionary<IEdge, RouteVertex> edgeStarts = new Dictionary<IEdge, RouteVertex>();
		Dictionary<IEdge, RouteVertex> edgeEnds = new Dictionary<IEdge, RouteVertex>();
		
		RouteVertex getStartVertex(IEdge edge)
		{
			return edgeStarts[edge];
		}
		RouteVertex getEndVertex(IEdge edge)
		{
			return edgeEnds[edge];
		}
		void setStartVertex(IEdge edge, RouteVertex value)
		{
			edgeStarts[edge] = value;
		}
		void setEndVertex(IEdge edge, RouteVertex value)
		{
			edgeEnds[edge] = value;
		}
		
		static EdgeStartEnd GetStartEnd(IEdge edge)
		{
			return new EdgeStartEnd { From = edge.From, To = edge.To };
		}
		
		/// <summary>
		/// Calculates space needed for given number of parallel edges coming from one node.
		/// </summary>
		static double GetMultiEdgeSpan(double maxSpace, int multiEdgeCount, double multiEdgeGap)
		{
			if (multiEdgeCount <= 1) {
				// 1 edge, no spacing needed
				return 0;
			}
			if ((multiEdgeCount + 1) * multiEdgeGap < maxSpace) {
				// the edges fit, maintain the gap
				return (multiEdgeCount - 1) * multiEdgeGap;
			} else {
				// there are too many edges, we have to make smaller gaps to fit edges into given space
				return maxSpace - multiEdgeGap;
			}
		}
	}
}
