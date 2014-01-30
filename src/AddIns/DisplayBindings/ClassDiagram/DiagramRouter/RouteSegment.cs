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
