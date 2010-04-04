// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// The result of the main method EdgeRouter.RouteEdges(), passed back to the user.
	/// </summary>
	public class RoutedEdge
	{
		public double Lenght {
			get {
				double len = 0;
				for (int i = 1; i < this.Points.Count; i++) {
					len += GeomUtils.LineLenght(this.Points[i - 1], this.Points[i]);
				}
				return len;
			}
		}
		
		List<Point2D> points;
		public List<Point2D> Points
		{
			get { return this.points; }
		}
		
		public RoutedEdge()
		{
			this.points = new List<Point2D>();
		}
		
		ReadOnlyCollection<Point2D> splinePoints;
		public ReadOnlyCollection<Point2D> SplinePoints
		{
			get
			{
				if (this.splinePoints == null)
				{
					List<Point2D> sPoints = RouteSpline(this.Points);
					splinePoints = sPoints.AsReadOnly();
				}
				return this.splinePoints;
			}
		}
		
		private static double tBend = 0.4;
		
		private List<Point2D> RouteSpline(List<Point2D> anchorPoints)
		{
			var result = new List<Point2D>();
			if (anchorPoints.Count == 0)
				return new List<Point2D>();
			Point2D point1 = anchorPoints[0];
			result.Add(point1);
			for (int i = 2; i < anchorPoints.Count; i++) {
				var point2 = anchorPoints[i - 1];
				var point3 = anchorPoints[i];
				var anchor1 = GeomUtils.Interpolate(point1, point2, 1 - tBend);
				var anchor2 = GeomUtils.Interpolate(point2, point3, tBend);
				// straight segment
				result.Add(anchor1);	// guide point1 ->  anchor1
				result.Add(point1);
				result.Add(anchor1);	// guide anchor1 -> point2
				// bend
				result.Add(point2);	// guide anchor1 ->  point2 (more carved ?)
				result.Add(point2);
				result.Add(anchor2);	// guide point2 ->  anchor2
				point1 = anchor2;
			}
			// last straight segment
			var lastPoint = anchorPoints[anchorPoints.Count - 1];
			result.Add(lastPoint);
			result.Add(point1);
			result.Add(lastPoint);
			return result;
		}
	}
}
