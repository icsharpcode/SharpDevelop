// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
	/// Draws <see cref="PositionedGraph"></see> on Canvas.
	/// </summary>
	public class GraphDrawer
	{
		Canvas canvas;
		TextBlock edgeTooltip = new TextBlock();
		
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
			if (oldGraph != null)
			{
				foreach	(var oldNode in oldGraph.Nodes)
				{
					foreach	(var newNode in newGraph.Nodes)
					{
						if (oldNode.NodeVisualControl == newNode.NodeVisualControl)
						{
							ClearCanvas();
						}
					}
				}
			}
			
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
			
			/*try
			{
			    // why do the controls disappear?
				var n1 = posGraph.Nodes.First().NodeVisualControl;
				var n2 = posGraph.Nodes.Skip(1).First().NodeVisualControl;
				var n3 = posGraph.Nodes.Skip(2).First().NodeVisualControl;
				if (n1 == n2 || n1 == n3 || n2 == n3)
				{
					ClearCanvas();
				}
			}
			catch{}*/
			
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
		
		private PositionedGraphNodeControl addNodeToCanvas(PositionedGraphNode node)
		{
			canvas.Children.Add(node.NodeVisualControl);
			Canvas.SetLeft(node.NodeVisualControl, node.Left);
			Canvas.SetTop(node.NodeVisualControl, node.Top);
			return node.NodeVisualControl;
		}
		
		private Path addEdgeToCanvas(PositionedEdge edge)
		{
			PathFigure edgeSplineFigure = createEdgeSpline(edge);

			PathGeometry geometryVisible = new PathGeometry();
			geometryVisible.Figures.Add(edgeSplineFigure);
			geometryVisible.Figures.Add(createEdgeArrow(edge));
			
			Path pathVisible = new Path();
			pathVisible.Stroke = Brushes.Black;
			pathVisible.Fill = Brushes.Black;
			pathVisible.StrokeThickness = 1;
			pathVisible.Data = geometryVisible;
			
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
			
			// remember this spline Path at PositionedEdge to be able to highlight edge from PositionedNodeProperty
			edge.Spline = pathVisible;
			
			canvas.Children.Add(pathVisible);
			canvas.Children.Add(pathInVisible);
			pathVisible.Tag = edge;
			return pathVisible;
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
