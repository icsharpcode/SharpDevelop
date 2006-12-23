/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using Tools.Diagrams;

namespace ClassDiagram
{
	[Flags]
	public enum SizeGripPositions {
		None = 0,
		North = 1,
		East = 2,
		South = 4,
		West = 8,
		NorthSouth = 5,
		EastWest = 10,
		All = 15
	};

	public class SizeGripEventArgs : EventArgs
	{
		SizeGripPositions gripPosition;
		
		public SizeGripEventArgs (SizeGripPositions gripPosition)
		{
			this.gripPosition = gripPosition;
		}
		
		public SizeGripPositions GripPosition
		{
			get { return gripPosition; }
		}
	}
	
	public class SizeGripDecorator : RectangleDecorator
	{
		public SizeGripDecorator (IRectangle rectangle)
			: base (rectangle)
		{
			if (rectangle.IsHResizable)
				this.gripPositions |= SizeGripPositions.EastWest;

			if (rectangle.IsVResizable)
				this.gripPositions |= SizeGripPositions.NorthSouth;
		}
		
		private SizeGripPositions gripPositions;
		private SizeGripPositions grabbedGrip;
		private SizeGripPositions highlightedGrip;
		
		private PointF dragPos = new Point(0, 0);
		private PointF dragOfst = new Point(0, 0);
		private float dragOrigWidth;
		private float dragOrigHeight;
		
		private PointF GetGripPosition (SizeGripPositions grip)
		{
			PointF pos = new PointF(0, 0);
			if (grip == SizeGripPositions.North)
			{
				pos.X = Rectangle.ActualWidth / 2;
				pos.Y = -4;
			}
			else if (grip == SizeGripPositions.East)
			{
				pos.X = Rectangle.ActualWidth + 4;
				pos.Y = Rectangle.ActualHeight / 2;
			}
			else if (grip == SizeGripPositions.South)
			{
				pos.X = Rectangle.ActualWidth / 2;
				pos.Y = Rectangle.ActualHeight + 4;
			}
			else if (grip == SizeGripPositions.West)
			{
				pos.X = - 4;
				pos.Y = Rectangle.ActualHeight / 2;
			}
			
			pos.X += Rectangle.AbsoluteX;
			pos.Y += Rectangle.AbsoluteY;
			
			return pos;
		}
		
		private void DrawGripRect (Graphics graphics, SizeGripPositions grip)
		{
			Pen pen = Pens.Gray;
			if (grip == highlightedGrip)
				pen = Pens.Black;
			
			PointF pos = GetGripPosition (grip);
			graphics.FillRectangle(Brushes.White, pos.X - 3, pos.Y - 3, 6, 6);
			graphics.DrawRectangle(pen, pos.X - 3, pos.Y - 3, 6, 6);
		}
		
		private static bool IsInGrip (PointF pos, PointF gripPos)
		{
			return (pos.X >= gripPos.X - 3 && pos.Y >= gripPos.Y - 3 &&
			        pos.X <= gripPos.X + 3 && pos.Y <= gripPos.Y + 3);
		}
		
		public override void DrawToGraphics(Graphics graphics)
		{
			if (graphics == null) return;
			if ((gripPositions & SizeGripPositions.North) == SizeGripPositions.North)
				DrawGripRect (graphics, SizeGripPositions.North);
			
			if ((gripPositions & SizeGripPositions.East) == SizeGripPositions.East)
				DrawGripRect (graphics, SizeGripPositions.East);
			
			if ((gripPositions & SizeGripPositions.South) == SizeGripPositions.South)
				DrawGripRect (graphics, SizeGripPositions.South);
			
			if ((gripPositions & SizeGripPositions.West) == SizeGripPositions.West)
				DrawGripRect (graphics, SizeGripPositions.West);
		}
		
		public SizeGripPositions GripPositions
		{
			get { return gripPositions; }
			set { gripPositions = value; }
		}
		
		private bool InternalHitTest(PointF pos, SizeGripPositions grip)
		{
			if ((gripPositions & grip) == grip)
				if (IsInGrip (pos, GetGripPosition(grip)))
				return true;
			return false;
		}
		
		private SizeGripPositions InternalHitTest(PointF pos)
		{
			if (InternalHitTest (pos, SizeGripPositions.North))
				return SizeGripPositions.North;
			
			if (InternalHitTest (pos, SizeGripPositions.East))
				return SizeGripPositions.East;
			
			if (InternalHitTest (pos, SizeGripPositions.South))
				return SizeGripPositions.South;
			
			if (InternalHitTest (pos, SizeGripPositions.West))
				return SizeGripPositions.West;
			
			return SizeGripPositions.None;
		}
		
		public override bool HitTest(PointF pos)
		{
			return InternalHitTest(pos) != SizeGripPositions.None;
		}

		public override void HandleMouseClick(PointF pos) { }
		
		public override void HandleMouseDown(PointF pos)
		{
			grabbedGrip = InternalHitTest(pos);
			dragPos = pos;
			dragOfst.X = Rectangle.X - dragPos.X;
			dragOfst.Y = Rectangle.Y - dragPos.Y;
			
			dragOrigWidth = Rectangle.ActualWidth;
			dragOrigHeight = Rectangle.ActualHeight;
		}
		
		public override void HandleMouseMove(PointF pos)
		{
			SizeGripPositions newGrip = InternalHitTest(pos);

			if (grabbedGrip == SizeGripPositions.None)
			{
				if (highlightedGrip != newGrip)
				{
					SizeGripMouseLeave (this, new SizeGripEventArgs(highlightedGrip));
					highlightedGrip = newGrip;
					SizeGripMouseEnter (this, new SizeGripEventArgs(highlightedGrip));
					EmitRedraw();
				}
			}

			if (grabbedGrip != SizeGripPositions.None)
			{
				if (grabbedGrip == SizeGripPositions.North)
				{
					if (pos.Y >= 40)
					{
						Rectangle.Y = dragOfst.Y + pos.Y;
						Rectangle.Height = dragOrigHeight - pos.Y + dragPos.Y;
					}
				}
				else if (grabbedGrip == SizeGripPositions.East)
				{
					Rectangle.Width = dragOrigWidth + pos.X - dragPos.X;
				}
				else if (grabbedGrip == SizeGripPositions.South)
				{
					Rectangle.Height = dragOrigHeight + pos.Y - dragPos.Y;
				}
				else if (grabbedGrip == SizeGripPositions.West)
				{
					if (pos.X >= 40)
					{
						Rectangle.X = dragOfst.X + pos.X;
						Rectangle.Width = dragOrigWidth - pos.X + dragPos.X;
					}
				}
			}
		}
		
		public override void HandleMouseUp(PointF pos)
		{
			grabbedGrip = SizeGripPositions.None;
		}
		
		public override void HandleMouseLeave()
		{
			if (grabbedGrip == SizeGripPositions.None)
			{
				SizeGripMouseLeave (this, new SizeGripEventArgs(highlightedGrip));
				highlightedGrip = SizeGripPositions.None;
				EmitRedraw();
			}
		}
		
		public event EventHandler<SizeGripEventArgs> SizeGripMouseEnter = delegate { };
		public event EventHandler<SizeGripEventArgs> SizeGripMouseLeave = delegate { };
	}
}
