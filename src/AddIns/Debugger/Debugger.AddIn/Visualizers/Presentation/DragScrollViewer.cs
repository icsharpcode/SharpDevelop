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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Debugger.AddIn.Visualizers.Controls
{
	/// <summary>
	/// ScrollViewer with support for drag-scrolling.
	/// </summary>
	public class DragScrollViewer : ScrollViewer
	{
		private bool isScrolling = false;
		private Point startDragPos;
		private Point scrollStartOffset;

		public DragScrollViewer() : base()
		{
			this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			
			this.PreviewMouseDown += DragScrollViewer_PreviewMouseDown;
			this.PreviewMouseMove += DragScrollViewer_PreviewMouseMove;
			this.PreviewMouseUp += DragScrollViewer_PreviewMouseUp;
			this.MouseWheel += DragScrollViewer_MouseWheel;
		}

		void DragScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			MessageBox.Show("w");
		}

		void DragScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && this.IsMouseDirectlyOver)
			{
				startDragPos = e.GetPosition(this);
				scrollStartOffset.X = this.HorizontalOffset;
				scrollStartOffset.Y = this.VerticalOffset;

				this.Cursor = this.canScroll() ? Cursors.ScrollAll : Cursors.Arrow;

				this.isScrolling = true;
				this.CaptureMouse();
				base.OnPreviewMouseDown(e);
			}
		}

		void DragScrollViewer_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (this.isScrolling)
			{
				Point currentPos = e.GetPosition(this);

				Vector delta = getScrollDelta(this.startDragPos, currentPos);

				this.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
				this.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
			}
			base.OnPreviewMouseMove(e); 
		}
		
		void DragScrollViewer_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released)
			{
				if (this.isScrolling)
				{
					this.Cursor = Cursors.Arrow;
					this.isScrolling = false;
					this.ReleaseMouseCapture();
				}
				base.OnPreviewMouseUp(e);
			}
		}

		private Vector getScrollDelta(Point startPos, Point currentPos)
		{
			double dx = startPos.X - currentPos.X;
			double dy = startPos.Y - currentPos.Y;
			//return new Vector(Math.Sign(dx)*dx*dx*0.01, Math.Sign(dy)*dy*dy*0.01);	// quadratic speedup
			return new Vector(dx, dy);
		}
		
		private bool canScroll()
		{
			return (this.ExtentWidth > this.ViewportWidth) || (this.ExtentHeight > this.ViewportHeight);
		}
	}
}
