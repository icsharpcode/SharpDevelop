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

namespace Tools.Diagrams
{
	public class RouteSegment
	{
		private Direction direction;
		private float length;
		
		public RouteSegment (float length, Direction direction)
		{
			this.length = length;
			this.direction = direction;
		}
		
		public Direction Direction
		{
			get { return direction; }
			set { direction = value; }
		}
		
		public float Length
		{
			get { return length; }
			set { length = value; }
		}
		
		public PointF CreateDestinationPoint (PointF origin)
		{
			PointF dest	= new PointF (origin.X, origin.Y);
			
			switch (direction)
			{
				case Direction.Up: dest.Y -= length; break;
				case Direction.Down: dest.Y += length; break;
				case Direction.Left: dest.X -= length; break;
				case Direction.Right: dest.X += length; break;
			}
				
			return dest;
		}
		
		public bool IntersectsWith (PointF origin, IRectangle rect)
		{
			PointF dest = CreateDestinationPoint(origin);
			bool xIntersects = false;
			bool yIntersects = false;
			
			if (direction == Direction.Left || direction == Direction.Right)
			{
				float y = origin.Y;
				float x1 = Math.Min(origin.X, dest.X);
				float x2 = Math.Max(origin.X, dest.X);
				yIntersects = (rect.Y <= y && rect.Y + rect.ActualHeight >= y);
				xIntersects = (x2 < rect.X || x1 > rect.X + rect.ActualWidth);
			}
			else
			{
				float x = origin.X;
				float y1 = Math.Min(origin.Y, dest.Y);
				float y2 = Math.Max(origin.Y, dest.Y);
				xIntersects = (rect.X <= x && rect.X + rect.ActualWidth >= x);
				yIntersects = (y2 < rect.Y || y1 > rect.Y + rect.ActualHeight);
			}
			return xIntersects && yIntersects;
		}
		
		public float IntersectionDistance (PointF origin, IRectangle rect)
		{
			PointF dest = CreateDestinationPoint(origin);
			float dist = -1;
			switch (direction)
			{
				case Direction.Left:
					dist = (origin.X - rect.X + rect.ActualWidth);
					break;
				case Direction.Right:
					dist = rect.X - origin.X;
					break;
				case Direction.Up:
					dist = (origin.Y - rect.Y + rect.ActualHeight);
					break;
				case Direction.Down:
					dist = rect.Y - origin.Y;
					break;
			}
			return dist;
		}
	}
}
