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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Debugger.AddIn.Visualizers.Graph;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using Debugger.AddIn.Visualizers.Graph.Layout;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Draws <see cref="PositionedGraph" /> on Canvas.
	/// Keeps the last displayed graph and does a smooth transition into the new graph.
	/// </summary>
	public class GraphDrawer
	{
		Canvas canvas;
		TextBlock edgeTooltip = new TextBlock();
		static double animationDurationSeconds = 0.5;
		
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
			// account for that the visual controls could have been reused (we are not reusing controls now - NodeControlCache does nothing)
			
			this.canvas.Width = newGraph.BoundingRect.Width;
			this.canvas.Height = newGraph.BoundingRect.Height;
			
			if (oldGraph == null) {
				Draw(newGraph);
				return;
			}
			
			var durationMove = new Duration(TimeSpan.FromSeconds(animationDurationSeconds));
			var durationFade = durationMove;
			
			DoubleAnimation fadeOutAnim = new DoubleAnimation(1.0, 0.0, durationFade);
			DoubleAnimation fadeInAnim = new DoubleAnimation(0.0, 1.0, durationFade);
			
			foreach	(UIElement drawing in canvas.Children) {
				var arrow = drawing as Path;
				if (arrow != null) {
					arrow.BeginAnimation(UIElement.OpacityProperty, fadeOutAnim);
				}
			}
			
			foreach	(PositionedEdge edge in newGraph.Edges) {
				AddEdgeToCanvas(edge).BeginAnimation(UIElement.OpacityProperty, fadeInAnim);
			}
			
			foreach	(PositionedNode removedNode in diff.RemovedNodes) {
				removedNode.NodeVisualControl.BeginAnimation(UIElement.OpacityProperty, fadeOutAnim);
			}
			
			foreach	(PositionedNode addedNode in diff.AddedNodes) {
				AddNodeToCanvas(addedNode).BeginAnimation(UIElement.OpacityProperty, fadeInAnim);
			}
			
			bool first = true;
			foreach	(PositionedNode node in diff.ChangedNodes) {
				var newNode = diff.GetMatchingNewNode(node);
				
				PointAnimation anim = new PointAnimation();
				if (first) {
					anim.Completed += (o, e) => {
						Draw(newGraph);
						if (oldGraph != null) {
							foreach (var oldNode in oldGraph.Nodes) {
								oldNode.ReleaseNodeVisualControl();
							}
						}
					};
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
			foreach	(PositionedNode node in posGraph.Nodes) {
				AddNodeToCanvas(node);
			}
			
			// draw edges
			foreach	(PositionedEdge edge in posGraph.Edges) {
				AddEdgeToCanvas(edge);
			}
			
			edgeTooltip.Visibility = Visibility.Hidden;
			edgeTooltip.Background = Brushes.White;
			canvas.Children.Add(edgeTooltip);
		}
		
		/// <summary>
		/// Clears the drawing Canvas.
		/// </summary>
		public void ClearCanvas()
		{
			canvas.Children.Clear();
		}
		
		PositionedGraphNodeControl AddNodeToCanvas(PositionedNode node)
		{
			canvas.Children.Add(node.NodeVisualControl);
			Canvas.SetLeft(node.NodeVisualControl, node.Left);
			Canvas.SetTop(node.NodeVisualControl, node.Top);
			return node.NodeVisualControl;
		}
		
		Path AddEdgeToCanvas(PositionedEdge edge)
		{
			var edgeSplineFigure = CreateEdgeSpline(edge);
			PathGeometry geometryVisible = new PathGeometry();
			geometryVisible.Figures.Add(edgeSplineFigure);
			geometryVisible.Figures.Add(CreateEdgeArrow(edge));
			
			Path pathVisible = new Path();
			pathVisible.Stroke = Brushes.Black;
			pathVisible.Fill = Brushes.Black;
			pathVisible.StrokeThickness = 1;
			pathVisible.Data = geometryVisible;
			
			// remember this spline Path at PositionedEdge to be able to highlight edge from PositionedNodeProperty
			edge.Spline = pathVisible;
			// and remember the the edge for the spline, so that we can get edge name on spline mouse-over
			pathVisible.Tag = edge;
			
			PathGeometry geometryInVisible = new PathGeometry();
			geometryInVisible.Figures.Add(edgeSplineFigure);
			
			Path pathInVisible = new Path();
			pathInVisible.Stroke = Brushes.Transparent;
			pathInVisible.Fill = Brushes.Transparent;
			pathInVisible.StrokeThickness = 16;
			pathInVisible.Data = geometryInVisible;
			
			pathInVisible.MouseEnter += delegate(object sender, MouseEventArgs e)
			{
				pathVisible.StrokeThickness = 2;
				this.edgeTooltip.Text = ((PositionedEdge)pathVisible.Tag).Name;
				Point mousePos = e.GetPosition(this.canvas);
				Canvas.SetLeft(this.edgeTooltip, mousePos.X - 5);
				Canvas.SetTop(this.edgeTooltip, mousePos.Y - 20);
				this.edgeTooltip.Visibility = Visibility.Visible;
			};
			pathInVisible.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				pathVisible.StrokeThickness = 1;
				this.edgeTooltip.Visibility = Visibility.Hidden;
			};
			pathInVisible.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				Point mousePos = e.GetPosition(this.canvas);
				Canvas.SetLeft(this.edgeTooltip, mousePos.X - 5);
				Canvas.SetTop(this.edgeTooltip, mousePos.Y - 20);
			};
			
			canvas.Children.Add(pathVisible);
			canvas.Children.Add(pathInVisible);
			
			return pathVisible;
		}
		
		PathFigure CreateEdgeSpline(PositionedEdge edge)
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
		
		PathFigure CreateEdgeArrow(PositionedEdge edge)
		{
			Point splineEndPoint = edge.SplinePoints[edge.SplinePoints.Count - 1];
			Point splineEndHandlePoint = edge.SplinePoints[edge.SplinePoints.Count - 2];
			
			Vector tangent = splineEndPoint - splineEndHandlePoint;
			tangent.Normalize();
			tangent = tangent * 20;
			Point basePoint = splineEndPoint - 0.4 * tangent;
			
			PathFigure arrowFigure = new PathFigure();
			arrowFigure.IsClosed = true;
			arrowFigure.IsFilled = true;

			arrowFigure.StartPoint = splineEndPoint;	// arrow tip
			Vector tangent2 = Rotate90(tangent);
			arrowFigure.Segments.Add(new LineSegment(basePoint + tangent2 * 0.15, true));
			arrowFigure.Segments.Add(new LineSegment(basePoint - tangent2 * 0.15, true));
			
			return arrowFigure;
		}
		
		static Vector Rotate90(Vector v)
		{
			// (x, y) -> (y, -x)
			double t = v.X;
			v.X = v.Y;
			v.Y = -t;
			return v;
		}
	}
}
