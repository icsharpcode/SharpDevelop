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

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using Tools.Diagrams;

namespace ClassDiagram
{
	public abstract class RectangleDecorator : IInteractiveDrawable
	{
		private IRectangle rect;
		private bool active;
		
		protected RectangleDecorator (IRectangle rectangle)
		{
			this.rect = rectangle;
		}
		
		public IRectangle Rectangle
		{
			get { return rect; }
		}

		public bool Active

		{
			get { return active; }
			set
			{
				active = value;
				EmitRedraw();
			}
		}

		public event EventHandler RedrawNeeded = delegate { };
		
		public abstract void DrawToGraphics(Graphics graphics);
		
		public virtual bool HitTest(PointF pos)
		{
			return (pos.X >= rect.AbsoluteX && pos.X <= rect.AbsoluteX + rect.ActualWidth && 
			        pos.Y >= rect.AbsoluteY && pos.Y <= rect.AbsoluteY + rect.ActualHeight);
		}
		
		public abstract void HandleMouseClick(PointF pos);
		public abstract void HandleMouseDown(PointF pos);
		public abstract void HandleMouseMove(PointF pos);
		public abstract void HandleMouseUp(PointF pos);
		public abstract void HandleMouseLeave();
		
		protected void EmitRedraw ()
		{
			RedrawNeeded (this, EventArgs.Empty);
		}
	}
}

