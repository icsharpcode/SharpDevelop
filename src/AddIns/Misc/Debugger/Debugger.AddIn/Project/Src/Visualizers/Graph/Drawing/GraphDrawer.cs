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
using Debugger.AddIn.Visualizers.Graph.Drawing;
using Debugger.AddIn.Visualizers.Graph.Layout;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Draws <see cref="PositionedGraph"></see> on Canvas.
	/// </summary>
    public class GraphDrawer
    {
        public GraphDrawer()
        {
        }
        
        /// <summary>
        /// Draws <see cref="PositionedGraph"></see> on Canvas.
        /// </summary>
        /// <param name="posGraph">Graph to draw.</param>
        /// <param name="canvas">Destination Canvas.</param>
        public static void Draw(PositionedGraph posGraph, Canvas canvas)
        {
        	canvas.Children.Clear();
        	
        	// draw nodes
        	foreach	(PositionedNode node in posGraph.Nodes)
        	{
        		canvas.Children.Add(node.NodeVisualControl);
        		Canvas.SetLeft(node.NodeVisualControl, node.Left);
        		Canvas.SetTop(node.NodeVisualControl, node.Top);
        	}
        	
        	// draw edges
        	foreach	(PositionedEdge edge in posGraph.Edges)
        	{
        		Path edgePath = createEdgeWithArrow(edge);
        		canvas.Children.Add(edgePath);
        	}
        }
        
        private static Path createEdgeWithArrow(PositionedEdge edge)
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
        
        private static PathFigure createEdgeSpline(PositionedEdge edge)
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
        
        private static PathFigure createEdgeArrow(PositionedEdge edge)
        {
			PathFigure arrowFigure = new PathFigure();
            arrowFigure.IsClosed = true;
            arrowFigure.IsFilled = true;
            
            Point endPoint = edge.SplinePoints[edge.SplinePoints.Count - 1];
            Point endHandlePoint = edge.SplinePoints[edge.SplinePoints.Count - 2];
            
            Vector tangent = endPoint - endHandlePoint;
            tangent.Normalize();
            tangent = tangent * 20;
            Point basePoint = endPoint - 0.2 * tangent;

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
