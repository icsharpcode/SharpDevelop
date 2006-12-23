// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using Tools.Diagrams;
using Tools.Diagrams.Drawables;

namespace ClassDiagram
{
	public class InteractiveHeaderedItem : HeaderedItem, IMouseInteractable, IHitTestable
	{
		public InteractiveHeaderedItem(IDrawableRectangle headerCollapsed,
		                                IDrawableRectangle headerExpanded,
		                                IDrawableRectangle content)
			: base (headerCollapsed, headerExpanded, content) {}
		
		public event EventHandler HeaderClicked = delegate {};
		public event EventHandler ContentClicked = delegate {};
		public event EventHandler HeaderMouseDown = delegate {};
		public event EventHandler ContentMouseDown = delegate {};		
		public event EventHandler HeaderMouseMove = delegate {};
		public event EventHandler ContentMouseMove = delegate {};		
		public event EventHandler HeaderMouseUp = delegate {};
		public event EventHandler ContentMouseUp = delegate {};		
		
		private void HandleMouseEvent(PointF pos, EventHandler headerEvent, EventHandler contentEvent)
		{
			if (Collapsed)
			{
				if (pos.X >= HeaderCollapsed.AbsoluteX &&
					pos.X <= HeaderCollapsed.AbsoluteX + HeaderCollapsed.ActualWidth &&
					pos.Y >= HeaderCollapsed.AbsoluteY &&
					pos.Y <= HeaderCollapsed.AbsoluteY + HeaderCollapsed.ActualHeight)
				{
					headerEvent(this, EventArgs.Empty);
				}
			}
			else
			{
				if (pos.X >= HeaderExpanded.AbsoluteX &&
					pos.X <= HeaderExpanded.AbsoluteX + HeaderExpanded.ActualWidth &&
					pos.Y >= HeaderExpanded.AbsoluteY &&
					pos.Y <= HeaderExpanded.AbsoluteY + HeaderExpanded.ActualHeight)
				{
					headerEvent(this, EventArgs.Empty);
				}
				else if (pos.X >= Content.AbsoluteX &&
						pos.X <= Content.AbsoluteX + Content.ActualWidth &&
						pos.Y >= Content.AbsoluteY &&
						pos.Y <= Content.AbsoluteY + Content.ActualHeight)
				{
					contentEvent(this, EventArgs.Empty);
				}
			}
		}

		public void HandleMouseClick(PointF pos)
		{
			HandleMouseEvent (pos, HeaderClicked, ContentClicked);
		}
		
		public void HandleMouseDown(PointF pos)
		{
			HandleMouseEvent (pos, HeaderMouseDown, ContentMouseDown);
		}
		
		public void HandleMouseMove(PointF pos)
		{
			HandleMouseEvent (pos, HeaderMouseMove, ContentMouseMove);
		}
		
		public void HandleMouseUp(PointF pos)
		{
			HandleMouseEvent (pos, HeaderMouseUp, ContentMouseUp);
		}
		
		public void HandleMouseLeave()
		{
			// TODO implement HandleMouseLeave
		}
		
		public bool HitTest(PointF pos)
		{
			return (pos.X >= AbsoluteX && pos.X <= AbsoluteX + ActualWidth  &&
					pos.Y >= AbsoluteY && pos.Y <= AbsoluteY + ActualHeight);
		}
		
		public bool HeaderHitTest(PointF pos)
		{
			if (Collapsed)
			{
				return (pos.X >= HeaderCollapsed.AbsoluteX && pos.X <= HeaderCollapsed.AbsoluteX + HeaderCollapsed.ActualWidth  &&
						pos.Y >= HeaderCollapsed.AbsoluteY && pos.Y <= HeaderCollapsed.AbsoluteY + HeaderCollapsed.ActualHeight);
			}
			else
			{
				return (pos.X >= HeaderExpanded.AbsoluteX && pos.X <= HeaderExpanded.AbsoluteX + HeaderExpanded.ActualWidth  &&
						pos.Y >= HeaderExpanded.AbsoluteY && pos.Y <= HeaderExpanded.AbsoluteY + HeaderExpanded.ActualHeight);		
			}
		}
	}
}
