using System;
// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Text;
using Debugger.AddIn.Visualizers.Graph;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using Debugger.AddIn.Visualizers.Graph.Layout;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Draws <see cref="PositionedGraph"></see> on Canvas.
	/// </summary>
	public class GraphDrawer
	{
		Canvas canvas;
		
		public GraphDrawer(Canvas canvas)
		{
			this.canvas = canvas;
		}
		
		/// <summary>
		/// Starts animation from oldGraph to newGraph.
		/// </summary>
		/// <param name="oldGraph"></param>
		/// <param name="newGraph"></param>
		/// <param name="diff"></param>
		public void StartAnimation(PositionedGraph oldGraph, PositionedGraph newGraph, GraphDiff diff)
		{
			this.canvas.Width = newGraph.BoundingRect.Width;
			this.canvas.Height = newGraph.BoundingRect.Height;
			
			if (oldGraph == null)
			{
				Draw(newGraph);
				return;
			}
			
			double seconds = 0.5;
			var durationMove = new Duration(TimeSpan.FromSeconds(seconds));
			var durationFade = durationMove;
			
			DoubleAnimation fadeOutAnim = new DoubleAnimation(1.0, 0.0, durationFade);
			DoubleAnimation fadeInAnim = new DoubleAnimation(0.0, 1.0, durationFade);
			
			foreach	(UIElement drawing in canvas.Children)
			{
				var arrow = drawing as Path;
				if (arrow != null)
				{
					arrow.BeginAnimation(UIElement.OpacityProperty, fadeOutAnim);
				}
			}
			
			foreach	(PositionedEdge edge in newGraph.Edges)
			{
				addEdgeToCanvas(edge).BeginAnimation(UIElement.OpacityProperty, fadeInAnim);
			}
			
			foreach	(PositionedGraphNode removedNode in diff.RemovedNodes)
			{
				removedNode.NodeVisualControl.BeginAnimation(UIElement.OpacityProperty, fadeOutAnim);
			}
			
			foreach	(PositionedGraphNode addedNode in diff.AddedNodes)
			{
				addNodeToCanvas(addedNode).BeginAnimation(UIElement.OpacityProperty, fadeInAnim);
			}
			
			bool first = true;
			foreach	(PositionedGraphNode node in diff.ChangedNodes)
			{
				var newNode = diff.GetMatchingNewNode(node);
				
				PointAnimation anim = new PointAnimation();
				if (first)
				{
					anim.Completed += new EventHandler((o, e) => { Draw(newGraph); });
					first = false;
				}
				anim.From = node.LeftTop;
				
				anim.To = newNode.LeftTop;
				anim.DecelerationRatio = 0.3;
				anim.AccelerationRatio = 0.3;
				anim.Duration = durationMove;
				node.NodeVisualControl.BeginAnimation(CanvasLocationAdapter.LocationProperty, anim);
			}
		}
		
		/// <summary>
		/// Draws <see cref="PositionedGraph"></see> on Canvas.
		/// </summary>
		/// <param name="posGraph">Graph to draw.</param>
		/// <param name="canvas">Destination Canvas.</param>
		public void Draw(PositionedGraph posGraph)
		{
			canvas.Children.Clear();
			
			// draw nodes
			foreach	(PositionedGraphNode node in posGraph.Nodes)
			{
				addNodeToCanvas(node);
			}
			
			// draw edges
			foreach	(PositionedEdge edge in posGraph.Edges)
			{
				addEdgeToCanvas(edge);
			}
		}
		
		private PositionedGraphNodeControl addNodeToCanvas(PositionedGraphNode node)
		{
			canvas.Children.Add(node.NodeVisualControl);
			Canvas.SetLeft(node.NodeVisualControl, node.Left);
			Canvas.SetTop(node.NodeVisualControl, node.Top);
			return node.NodeVisualControl;
		}
		
		private Path addEdgeToCanvas(PositionedEdge edge)
		{
			Path edgePath = createEdgeWithArrow(edge);
			canvas.Children.Add(edgePath);
			return edgePath;
		}
		
		private Path createEdgeWithArrow(PositionedEdge edge)
		{
			Path path = new Path();
			path.Stroke = Brushes.Black;
			path.Fill = Brushes.Black;
			path.StrokeThickness = 1;

			PathGeometry geometry = new PathGeometry();

			geometry.Figures.Add(createEdgeSpline(edge));
			geometry.Figures.Add(createEdgeArrow(edge));
			
			path.Data = geometry;
			return path;
		}
		
		private PathFigure createEdgeSpline(PositionedEdge edge)
		{
			PathFigure figure = new PathFigure();
			figure.IsClosed = false;
			figure.IsFilled = false;
			
			figure.StartPoint = edge.SplinePoints[0];
			for (int i = 1; i < edge.SplinePoints.Count; i += 3)
			{
				figure.Segments.Add(new BezierSegment(edge.SplinePoints[i], edge.SplinePoints[i + 1], edge.SplinePoints[i + 2], true));
			}
			
			return figure;
		}
		
		private PathFigure createEdgeArrow(PositionedEdge edge)
		{
			Point splineEndPoint = edge.SplinePoints[edge.SplinePoints.Count - 1];
			Point splineEndHandlePoint = edge.SplinePoints[edge.SplinePoints.Count - 2];
			
			Vector tangent = splineEndPoint - splineEndHandlePoint;
			tangent.Normalize();
			tangent = tangent * 20;
			Point basePoint = splineEndPoint - 0.2 * tangent;
			
			PathFigure arrowFigure = new PathFigure();
			arrowFigure.IsClosed = true;
			arrowFigure.IsFilled = true;

			arrowFigure.StartPoint = basePoint + tangent * 0.4;	// arrow tip
			Vector tangent2 = rotate90(tangent);
			arrowFigure.Segments.Add(new LineSegment(basePoint + tangent2 * 0.15, true));
			arrowFigure.Segments.Add(new LineSegment(basePoint - tangent2 * 0.15, true));
			
			return arrowFigure;
		}
		
		private static Vector rotate90(Vector v)
		{
			// (x, y) -> (y, -x)
			double t = v.X;
			v.X = v.Y;
			v.Y = -t;
			return v;
		}
	}
}
