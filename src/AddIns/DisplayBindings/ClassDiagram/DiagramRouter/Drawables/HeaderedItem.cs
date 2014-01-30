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
using System.Drawing;
using System.Drawing.Drawing2D;

using Tools.Diagrams;

namespace Tools.Diagrams.Drawables
{
	public class HeaderedItem : BaseRectangle, IDrawableRectangle
	{
		bool collapsed;
		IDrawableRectangle headerExpanded;
		IDrawableRectangle content;
		IDrawableRectangle headerCollapsed;
		
		public event EventHandler RedrawNeeded = delegate {};
		
		public HeaderedItem(IDrawableRectangle headerCollapsed,
		                    IDrawableRectangle headerExpanded,
		                    IDrawableRectangle content)
		{
			this.headerCollapsed = headerCollapsed;
			this.headerExpanded = headerExpanded;
			this.content = content;
			
			headerCollapsed.Container = this;
			headerExpanded.Container = this;
			content.Container = this;
			
			headerCollapsed.X = 0;
			headerCollapsed.Y = 0;
			
			headerExpanded.X = 0;
			headerExpanded.Y = 0;
				
			content.X = 0;
			content.Y = headerExpanded.GetAbsoluteContentHeight();
			
			headerExpanded.HeightChanged += delegate { content.Y = headerExpanded.GetAbsoluteContentHeight(); };
		}

		public IDrawableRectangle HeaderCollapsed
		{
			get { return headerCollapsed; }
		}
		
		public IDrawableRectangle HeaderExpanded
		{
			get { return headerExpanded; }
		}
		
		public IDrawableRectangle Content
		{
			get { return content; }
		}
		
		public bool Collapsed
		{
			get { return collapsed; }
			set 
			{
				collapsed = value;
				FireRedrawNeeded();
			}
		}
		
		protected void FireRedrawNeeded()
		{
			RedrawNeeded(this, EventArgs.Empty);
		}
		
		#region Geometry
		
		public override float ActualHeight
		{
			get { return GetAbsoluteContentHeight(); }
			set { base.Height = value; }
		}
		
		public override float ActualWidth
		{
			get { return GetAbsoluteContentWidth(); }
			set { base.Width = value; }
		}
		
		protected override void OnWidthChanged()
		{
			headerCollapsed.Width = base.Width;
			headerExpanded.Width = base.Width;
			content.Width = base.Width;
		}
		
		protected override void OnHeightChanged()
		{
			headerCollapsed.Height = headerCollapsed.GetAbsoluteContentHeight();
			headerExpanded.Height = headerExpanded.GetAbsoluteContentHeight();
			content.Height = content.GetAbsoluteContentHeight();
		}
		
		protected override void OnActualWidthChanged()
		{
			headerCollapsed.ActualWidth = base.ActualWidth;
			headerExpanded.ActualWidth = base.ActualWidth;
			content.ActualWidth = base.ActualWidth;
		}
		
		protected override void OnActualHeightChanged()
		{
			headerCollapsed.ActualHeight = headerCollapsed.GetAbsoluteContentHeight();
			headerExpanded.ActualHeight = headerExpanded.GetAbsoluteContentHeight();
			content.ActualHeight = content.GetAbsoluteContentHeight();
		}

		#endregion
		
		public void DrawToGraphics(Graphics graphics)
		{
			if (!collapsed)
			{
				//TODO - add orientation, so the header could also be on the side.
				headerExpanded.DrawToGraphics(graphics);
				content.DrawToGraphics(graphics);
			}
			else
			{
				headerCollapsed.DrawToGraphics(graphics);
			}
		}
		
		public override float GetAbsoluteContentWidth()
		{
			float width = 0;
			if (!collapsed)
			{
				width = Math.Max(width, headerExpanded.GetAbsoluteContentWidth());
				width = Math.Max(width, content.GetAbsoluteContentWidth() + content.Padding * 2);
			}
			else
				width = headerCollapsed.GetAbsoluteContentWidth();
			
			return width;
		}
		
		public override float GetAbsoluteContentHeight()
		{
			float height = 0;
			if (!collapsed)
			{
				height = headerExpanded.GetAbsoluteContentHeight();
				height += content.GetAbsoluteContentHeight();
				height += content.Padding * 2;
			}
			else
			{
				height = headerCollapsed.GetAbsoluteContentHeight();
			}
			return height;
		}
	}
}
