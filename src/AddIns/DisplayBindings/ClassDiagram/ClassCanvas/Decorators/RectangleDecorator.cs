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
