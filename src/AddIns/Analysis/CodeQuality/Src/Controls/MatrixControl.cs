// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	public class MatrixControl<TValue> : FrameworkElement, IScrollInfo
	{
		private Dictionary<string, ImageSource> imgs = new Dictionary<string, ImageSource>();
		private Coords currentCell = new Coords(-1, -1);
		private string font;

		private bool canHorizontalScroll = true;
		private bool canVerticalScroll = true;
		private Size extent = new Size(0, 0);
		private Size viewport = new Size(0, 0);
		private Point offset;
		
		// will be loaded from Matrix
		private int matrixWidth = 0;
		private int matrixHeight = 0;

		private Matrix<TValue> matrix;
		
		public Matrix<TValue> Matrix
		{
			get { return matrix; }
			set
			{
				matrix = value;

				matrixHeight = Matrix.HeaderRows.Count;
				matrixWidth = Matrix.HeaderColumns.Count;
			}
		}
		
		public int CellHeight { get; set; }

		public int CellWidth { get; set; }
		
		public MatrixControl()
		{
			CellHeight = CellWidth = 36;
			matrixWidth = 20;
			matrixHeight = 20;
			font = "Verdana";
		}
		
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			
			var point = e.GetPosition(this);
			if (point.X < matrixWidth * CellWidth && point.Y < matrixHeight * CellHeight)
				currentCell = new Coords(
					(int)((point.X + offset.X) / CellWidth),
					(int)((point.Y + offset.Y) / CellHeight));
			else
				currentCell = new Coords(-1, -1);
			
			InvalidateVisual();
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			// how much space we got for cells
			var maxWidth = ((int) viewport.Width / CellWidth) + 1;
			var maxHeight = ((int) viewport.Height / CellHeight) + 1;

			// how many cells we will draw
			var cellsHorizontally = maxWidth > matrixWidth ? matrixWidth : maxWidth;
			var cellsVertically = maxHeight > matrixHeight ? matrixHeight : maxHeight;
			
			// number of cell which will be drawn
			var scaledOffsetX = (int)offset.X / CellWidth;
			var scaledOffsetY = (int)offset.Y / CellHeight;

			// sets how much of cell on border should be drawn
			var offsetDiffX = offset.X - scaledOffsetX * CellWidth;
			var offsetDiffY = offset.Y - scaledOffsetY * CellHeight;
			
			// background
			var background = new Rect(0, 0, cellsHorizontally * CellWidth, cellsVertically * CellHeight);
			var backgroundColor = new SolidColorBrush(Colors.Yellow);
			drawingContext.DrawRectangle(backgroundColor, null, background);
			
			// hovering
			if (currentCell.X >= 0 || currentCell.Y >= 0) {
				
				// hover x line
				var rect = new Rect(0,
				                (currentCell.Y - scaledOffsetY) * CellHeight - offsetDiffY,
				                CellWidth * cellsVertically,
				                CellHeight);
				
				var brush = new SolidColorBrush(Colors.GreenYellow);
				drawingContext.DrawRectangle(brush, null, rect);
				
				// hover y line
				rect = new Rect((currentCell.X - scaledOffsetX) * CellWidth - offsetDiffX,
				                0,
				                CellWidth,
				                CellHeight * cellsHorizontally);
				
				brush = new SolidColorBrush(Colors.GreenYellow);
				drawingContext.DrawRectangle(brush, null, rect);
				
				// hover cell
				rect = new Rect(
					(currentCell.X - scaledOffsetX) * CellWidth - offsetDiffX,
					(currentCell.Y - scaledOffsetY) * CellHeight - offsetDiffY,
					CellWidth,
					CellHeight);

				brush = new SolidColorBrush(Colors.Red);
				drawingContext.DrawRectangle(brush, null, rect);
			}
			
			// grid
			var pen = new Pen(Brushes.Black, 1);

			// grid x
			for (int i = 0; i <= cellsHorizontally; i++)
				drawingContext.DrawLine(pen,
				                        new Point(i * CellWidth - offsetDiffX, 0),
				                        new Point(i * CellWidth - offsetDiffX,
				                                  cellsVertically * CellWidth));
			// grid y
			for (int i = 0; i <= cellsVertically; i++)
				drawingContext.DrawLine(pen,
				                        new Point(0, i * CellHeight - offsetDiffY),
				                        new Point(cellsHorizontally * CellHeight,
				                                  i * CellHeight - offsetDiffY));

			// sometimes happens when half of cell is hidden in scroll so text isnt drawn
			// so lets drawn one more cell
			cellsHorizontally = maxWidth > matrixWidth ? matrixWidth : maxWidth + 1;
			cellsVertically = maxHeight > matrixHeight ? matrixHeight : maxHeight + 1;

			// text
			for (int i = 0; i < cellsHorizontally; i++)
				for (int j = 0; j < cellsVertically; j++)
					drawingContext.DrawImage(
						CreateText((i + scaledOffsetX) * (j + scaledOffsetY)),
						new Rect(i * CellWidth - offsetDiffX, j * CellHeight - offsetDiffY, CellWidth, CellHeight));
		}
		
		public ImageSource CreateText(int number)
		{
			return CreateText(number.ToString());
		}
		
		public ImageSource CreateText(string text)
		{
			if (imgs.ContainsKey(text))
				return imgs[text];
			
			var bmp = new System.Drawing.Bitmap(CellWidth, CellHeight);
			var g = System.Drawing.Graphics.FromImage(bmp);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			
			var fontOjb = new System.Drawing.Font(font, 12);
			
			var size = g.MeasureString(text, fontOjb);
			
			var spanWidth = (CellWidth - size.Width) / 2;
			var spanHeight = (CellHeight - size.Height) / 2;
			
			g.DrawString(text, fontOjb, System.Drawing.Brushes.Black, new System.Drawing.PointF(spanWidth, spanHeight));
			g.Dispose();
			
			var img = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
			                                                                       IntPtr.Zero,
			                                                                       Int32Rect.Empty,
			                                                                       BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
			imgs.Add(text, img);
			
			return img;
		}
		
		public bool CanVerticallyScroll
		{
			get { return canVerticalScroll; }
			set { canVerticalScroll = value; }
		}
		
		public bool CanHorizontallyScroll
		{
			get { return canHorizontalScroll; }
			set { canHorizontalScroll = value; }
		}
		
		/// <summary>
		/// Width of entire component
		/// </summary>
		public double ExtentWidth
		{
			get { return extent.Width; }
		}
		
		/// <summary>
		/// Height of entire component
		/// </summary>
		public double ExtentHeight
		{
			get { return extent.Height; }
		}
		
		/// <summary>
		/// Width of available area
		/// </summary>
		public double ViewportWidth
		{
			
			get { return viewport.Width; }
		}
		
		/// <summary>
		/// Height of available area
		/// </summary>
		public double ViewportHeight
		{
			get { return viewport.Height; }
		}
		
		/// <summary>
		/// Where is scrollbar located on X
		/// </summary>
		public double HorizontalOffset
		{
			get { return offset.X; }
		}
		
		/// <summary>
		/// Where is scrollbar located on Y
		/// </summary>
		public double VerticalOffset
		{
			get { return offset.Y; }
		}

		public ScrollViewer ScrollOwner { get; set; }

		public void LineUp()
		{
			SetVerticalOffset(VerticalOffset - 1);
		}
		
		public void LineDown()
		{
			SetVerticalOffset(VerticalOffset + 1);
		}
		
		public void LineLeft()
		{
			SetHorizontalOffset(HorizontalOffset - 1);
		}
		
		public void LineRight()
		{
			SetHorizontalOffset(HorizontalOffset + 1);
		}
		
		public void PageUp()
		{
			SetVerticalOffset(VerticalOffset - CellHeight);
		}
		
		public void PageDown()
		{
			SetVerticalOffset(VerticalOffset + CellHeight);
		}
		
		public void PageLeft()
		{
			SetHorizontalOffset(HorizontalOffset - CellWidth);
		}
		
		public void PageRight()
		{
			SetHorizontalOffset(HorizontalOffset + CellWidth);
		}
		
		public void MouseWheelUp()
		{
			SetVerticalOffset(VerticalOffset - 4);
		}
		
		public void MouseWheelDown()
		{
			SetVerticalOffset(VerticalOffset + 4);
		}
		
		public void MouseWheelLeft()
		{
			SetVerticalOffset(HorizontalOffset - 4);
		}
		
		public void MouseWheelRight()
		{
			SetVerticalOffset(HorizontalOffset + 4);
		}
		
		public void SetHorizontalOffset(double offset)
		{
			if (offset == this.offset.X) return;
			this.offset.X = offset;
			InvalidateVisual();
		}
		
		public void SetVerticalOffset(double offset)
		{
			if (offset == this.offset.Y) return;
			this.offset.Y = offset;
			InvalidateVisual();
		}
		
		public Rect MakeVisible(Visual visual, Rect rectangle)
		{
			throw new NotImplementedException();
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			VerifyScrollData(availableSize, new Size(matrixWidth * CellWidth, matrixHeight * CellHeight));
			return viewport;
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			VerifyScrollData(finalSize, new Size(matrixWidth * CellWidth, matrixHeight * CellHeight));
			return finalSize;
		}
		
		protected void VerifyScrollData(Size viewport, Size extent)
		{
			if (double.IsInfinity(viewport.Width))
				viewport.Width = extent.Width;

			if (double.IsInfinity(viewport.Height))
				viewport.Height = extent.Height;

			// if viewport changes so recalculates offsets
			offset.X = Math.Max(0, Math.Min(offset.X, ExtentWidth - ViewportWidth));
			offset.Y = Math.Max(0, Math.Min(offset.Y, ExtentHeight - ViewportHeight));

			ScrollOwner.InvalidateScrollInfo();

			this.extent = extent;
			this.viewport = viewport;

			if (ScrollOwner != null)
				ScrollOwner.InvalidateScrollInfo();
		}

		private struct Coords
		{
			public int Y { get; private set; }
			public int X { get; private set; }

			public Coords(int x, int y) : this()
			{
				X = x;
				Y = y;
			}
		}
	}
}
