// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Description of GeomUtils.
	/// </summary>
	public class GeomUtils
	{
		static readonly double eps = 1e-8;
		
		public GeomUtils()
		{
		}
		
		public static IPoint RectCenter(IRect rect)
		{
			return new Point2D(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
		}
		
		public static double LineLenght(IPoint lineStart, IPoint lineEnd)
		{
			var dx = lineEnd.X - lineStart.X;
			var dy = lineEnd.Y - lineStart.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		
		public static Point2D? LineRectIntersection(IPoint lineStart, IPoint lineEnd, IRect rect)
		{
			double vx = lineEnd.X - lineStart.X;
			double vy = lineEnd.Y - lineStart.Y;
			double right = rect.Left + rect.Width;
			double bottom = rect.Top + rect.Height;
			
			var isectTop = IsectHoriz(lineStart, vx, vy, rect.Top, rect.Left, right);
			if (isectTop != null)
				return isectTop;
			var isectBottom = IsectHoriz(lineStart, vx, vy, bottom, rect.Left, right);
			if (isectBottom != null)
				return isectBottom;
			var isectLeft = IsectVert(lineStart, vx, vy, rect.Left, rect.Top, bottom);
			if (isectLeft != null)
				return isectLeft;
			var isectRight = IsectVert(lineStart, vx, vy, right, rect.Top, bottom);
			if (isectRight != null)
				return isectRight;
			
			return null;
		}
		
		static Point2D? IsectHoriz(IPoint lineStart, double vx, double vy, double yBound, double left, double right)
		{
			if (Math.Abs(vy) < eps)
				return null;
			double t = (yBound - lineStart.Y) / vy;
			if (t > 0 && t < 1)
			{
				double iX = (lineStart.X + t * vx);
				if (iX >= left && iX <= right)
					return new Point2D(iX, yBound);
			}
			return null;
		}
		
		static Point2D? IsectVert(IPoint lineStart, double vx, double vy, double xBound, double top, double bottom)
		{
			if (Math.Abs(vx) < eps)
				return null;
			double t = (xBound - lineStart.X) / vx;
			if (t > 0 && t < 1)
			{
				double iY = (lineStart.Y + t * vy);
				if (iY >= top && iY <= bottom)
					return new Point2D(xBound, iY);
			}
			return null;
		}
		
		public static Box InflateRect(IRect rect, double padding)
		{
			return new Box(rect.Left - padding, rect.Top - padding, rect.Width + padding * 2, rect.Height + padding * 2);
		}
		
		public static Point2D Interpolate(Point2D pointA, Point2D pointB, double t)
		{
			double b = 1 - t;
			return new Point2D(pointA.X * t + pointB.X * b, pointA.Y * t + pointB.Y * b);
		}
		
		/*public static double LineLineIntersection(Point line1Start, Point line1End, Point line2Start, Point line2End)
		{
			
		}*/
	}
}
