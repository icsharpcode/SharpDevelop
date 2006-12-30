/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 03/11/2006
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Tools.Diagrams
{
	public interface IRouteShape
	{
		
	}

	public class Route
	{
		private IRectangle from;
		private IRectangle to;
		private LinkedList<RouteSegment> segments = new LinkedList<RouteSegment>();
		private IRouteShape startShape;
		private IRouteShape endShape;
		
		private enum ConnectionPoint {North, East, South, West, Center};
		
		public Route (IRectangle from, IRectangle to)
		{
			this.from = from;
			this.to = to;
		}
		
		public IRectangle From
		{
			get { return from; }
			set { from = value; }
		}

		public IRectangle To
		{
			get { return to; }
			set { to = value; }
		}
		
		public IRouteShape StartShape
		{
			get { return startShape; }
			set { startShape = value; }
		}

		public IRouteShape EndShape
		{
			get { return endShape; }
			set { endShape = value; }
		}
		
		private static PointF GetConnectionPointPosition (IRectangle rect, ConnectionPoint point)
		{
			switch (point)
			{
					case ConnectionPoint.North: return new PointF(rect.X + rect.ActualWidth / 2, rect.Y);
					case ConnectionPoint.East: return new PointF(rect.X + rect.ActualWidth, rect.Y + rect.ActualHeight / 2);
					case ConnectionPoint.South: return new PointF(rect.X + rect.ActualWidth / 2, rect.Y + rect.ActualHeight);
					case ConnectionPoint.West: return new PointF(rect.X, rect.Y + rect.ActualHeight / 2);
			}
			return new PointF(rect.X + rect.ActualWidth / 2, rect.Y + rect.ActualHeight / 2);
		}
		
		private void GetClosestConnectionPoints(out ConnectionPoint fromPoint, out ConnectionPoint toPoint)
		{
			float shortestDist = float.MaxValue;
			float dist = shortestDist;

			fromPoint = ConnectionPoint.Center;
			toPoint = ConnectionPoint.Center;
			
			for (int i = 0; i < 4; ++i)
			{
				PointF posF = GetConnectionPointPosition(from, (ConnectionPoint)i);
				for (int j = 0; j < 4; ++j)
				{
					PointF posT = GetConnectionPointPosition(to, (ConnectionPoint)j);
					float dx = posF.X - posT.X;
					float dy = posF.Y - posT.Y;
					dist = (dx*dx) + (dy*dy);
					if (dist < shortestDist)
					{
						shortestDist = dist;
						fromPoint = (ConnectionPoint)i;
						toPoint = (ConnectionPoint)j;
					}
				}
			}
		}
		
		public PointF GetStartPoint()
		{
			ConnectionPoint startPoint = ConnectionPoint.Center;
			ConnectionPoint endPoint = ConnectionPoint.Center;
			
			GetClosestConnectionPoints(out startPoint, out endPoint);

			return GetConnectionPointPosition(from, startPoint);
		}
		
		public PointF GetEndPoint()
		{
			PointF p = GetStartPoint();
			foreach (RouteSegment seg in segments)
			{
				p = seg.CreateDestinationPoint(p);
			}
			return p;
		}
		
		public Direction GetStartDirection()
		{
			return segments.First.Value.Direction;
		}
		
		public Direction GetEndDirection()
		{
			return segments.Last.Value.Direction;
		}
		
		public RouteSegment[] RouteSegments
		{
			get
			{
				RouteSegment[] rs = new RouteSegment[segments.Count];
				segments.CopyTo(rs, 0);
				return rs;
			}
		}

		private IRectangle FindClosestObstacle (RouteSegment seg, PointF origin, IList<IRectangle> rectangles)
		{
			IRectangle closestRect = null;
			float minDist = float.MaxValue;
			foreach (IRectangle rect in FindAllSegmentObstacles(seg, origin, rectangles))
			{
				if (closestRect != null)
				{
					float dist = seg.IntersectionDistance(origin, rect);
					if (minDist > dist)
					{
						minDist = dist;
						closestRect = rect;
					}
				}
				else
					closestRect = rect;
			}
			return closestRect;
		}

		private ICollection<IRectangle> FindAllSegmentObstacles (RouteSegment seg, PointF origin, IList<IRectangle> rectangles)
		{
			List<IRectangle> obstacles = new List<IRectangle>();
			foreach (IRectangle rect in rectangles)
			{
				if (seg.IntersectsWith(origin, rect))
					obstacles.Add(rect);
			}
			return obstacles;
		}
		
		private ICollection<IRectangle> FindAllRouteObstacles (IList<IRectangle> rectangles)
		{
			PointF fromPoint = GetStartPoint();
			PointF p = fromPoint;
			List<IRectangle> allobstacles = new List<IRectangle>();
			foreach (RouteSegment seg in segments)
			{
				ICollection<IRectangle> rects = FindAllSegmentObstacles(seg, p, rectangles);
				foreach (IRectangle rect in rects)
				{
					if (!allobstacles.Contains(rect))
						allobstacles.Add(rect);
				}
				p = seg.CreateDestinationPoint(p);
			}
			return allobstacles;
		}
		
		private void FixRouteSegment (RouteSegment seg)
		{
			
		}
		
		public void Recalc (IEnumerable<IRectangle> rectangles)
		{
			segments.Clear();
			
			ConnectionPoint startPoint = ConnectionPoint.Center;
			ConnectionPoint endPoint = ConnectionPoint.Center;
			
			GetClosestConnectionPoints(out startPoint, out endPoint);

			PointF posF = GetConnectionPointPosition(from, startPoint);
			PointF posT = GetConnectionPointPosition(to, endPoint);
			
			Direction dir1 = default (Direction);
			Direction dir2 = default (Direction);
			Direction dir3 = default (Direction);
			
			float l1 = 0;
			float l2 = 0;
			float l3 = 0;
			
			switch (startPoint)
			{
					case ConnectionPoint.North: dir1 = Direction.Up; break;
					case ConnectionPoint.East: dir1 = Direction.Right; break;
					case ConnectionPoint.South: dir1 = Direction.Down; break;
					case ConnectionPoint.West: dir1 = Direction.Left; break;
			}

			switch (endPoint)
			{
					case ConnectionPoint.North: dir3 = Direction.Down; break;
					case ConnectionPoint.East: dir3 = Direction.Left; break;
					case ConnectionPoint.South: dir3 = Direction.Up; break;
					case ConnectionPoint.West: dir3 = Direction.Right; break;
			}
			
			if ((dir1 == Direction.Down && dir3 == Direction.Up) ||
			    (dir3 == Direction.Down && dir1 == Direction.Up))
			{
				l1 = l3 = 20;
				float h = Math.Abs(posF.Y - posT.Y);
				if (posT.Y > posF.Y)
					l3 += h;
				else
					l1 += h;
				l2 = Math.Abs(posF.X - posT.X);
				dir2 = (posT.X > posF.X) ? Direction.Right : Direction.Left;
			}
			else if ((dir1 == Direction.Left && dir3 == Direction.Right) ||
			         (dir3 == Direction.Left && dir1 == Direction.Right))
			{
				l1 = l3 = 20;
				float w = Math.Abs(posF.X - posT.X);
				if (posT.X > posF.X)
					l3 += w;
				else
					l1 += w;
				l2 = Math.Abs(posF.Y - posT.Y);
				dir2 = (posT.Y > posF.Y) ? Direction.Down : Direction.Up;
			}
			else if ((dir1 == Direction.Down && dir3 == Direction.Down) ||
			         (dir1 == Direction.Up && dir3 == Direction.Up))
			{
				l1 = l3 = Math.Abs(posF.Y - posT.Y) / 2;
				l2 = Math.Abs(posF.X - posT.X);
				dir2 = (posT.X > posF.X) ? Direction.Right : Direction.Left;
			}
			else if ((dir1 == Direction.Left && dir3 == Direction.Left) ||
			         (dir1 == Direction.Right && dir3 == Direction.Right))
			{
				l1 = l3 = Math.Abs(posF.X - posT.X) / 2;
				l2 = Math.Abs(posF.Y - posT.Y);
				dir2 = (posT.Y > posF.Y) ? Direction.Down : Direction.Up;
			}
			else if ((dir1 == Direction.Left || dir1 == Direction.Right) && 
			         (dir3 == Direction.Up || dir3 == Direction.Down))
			{
				l1 = Math.Abs(posF.X - posT.X);
				l3 = Math.Abs(posF.Y - posT.Y);
			}
			else if ((dir3 == Direction.Left || dir3 == Direction.Right) && 
			         (dir1 == Direction.Up || dir1 == Direction.Down))
			{
				l3 = Math.Abs(posF.X - posT.X);
				l1 = Math.Abs(posF.Y - posT.Y);
			}
			
			RouteSegment seg2 = null;
			
			RouteSegment seg1 = new RouteSegment(l1, dir1);
			if (l2 > 0)
				seg2 = new RouteSegment(l2, dir2);
			RouteSegment seg3 = new RouteSegment(l3, dir3);
			
			segments.AddFirst(seg1);
			if (seg2 != null)
				segments.AddLast(seg2);
			segments.AddLast(seg3);
		}
	}
}
