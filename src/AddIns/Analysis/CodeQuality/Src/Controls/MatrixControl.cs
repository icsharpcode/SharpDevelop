// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.CodeQualityAnalysis.Utility;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using PointF = System.Drawing.PointF;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	public class MatrixControl<TMatrix, TItem, TValue> : FrameworkElement, IScrollInfo
		where TValue : IValue
		where TMatrix : Matrix<TItem, TValue>
	{
		public event EventHandler<HoveredCellEventArgs<TValue>> HoveredCellChanged;
		
		private Dictionary<string, ImageSource> imgs = new Dictionary<string, ImageSource>();
		private Coords currentCell = new Coords(0, 0);
		private string font;

		private bool canHorizontalScroll = true;
		private bool canVerticalScroll = true;
		private Size extent = new Size(0, 0);
		private Size viewport = new Size(0, 0);
		private Point offset;
		
		// will be loaded from Matrix
		private int matrixWidth = 0;
		private int matrixHeight = 0;
		
		private int fontSize = 0;
		private int penSize = 0;
		
		
		protected int PageSizeWidth { get; set; }
		protected int PageSizeHeight { get; set; }
		
		public TMatrix Matrix { get; set; }
		
		public int CellHeight { get; set; }

		public int CellWidth { get; set; }
		
		public HoveredCell<TValue> HoveredCell { get; set; }
		
		public bool RenderZeroes { get; set; }
		
		public IColorizer<TValue> Colorizer { get; set; }
		
		public MatrixControl()
		{
			CellHeight = CellWidth = 18;
			matrixWidth = 0;
			matrixHeight = 0;
			fontSize = CellHeight / 3;
			penSize = 1;
			font = "Verdana";
			
			HoveredCell = new HoveredCell<TValue>();
		}
		
		public void SetVisibleItems(HeaderType type, ICollection<TItem> visibleItems)
		{
			Matrix.SetVisibleItems(type, visibleItems);
			
			matrixHeight = Matrix.HeaderRows.Count;
			matrixWidth = Matrix.HeaderColumns.Count;
			
			bool changedCoords = false;
			if (currentCell.X > matrixHeight) {
				currentCell = new Coords(matrixHeight - 1, currentCell.Y);
				changedCoords = true;
			}
			
			if (currentCell.Y > matrixWidth) {
				currentCell = new Coords(currentCell.X, matrixWidth - 1);
				changedCoords = true;
			}
			
			if (changedCoords) {
				SetHoveredCell();
			}
			
			if (matrixHeight >= 0 && matrixWidth >= 0)
				InvalidateVisual();
		}
		
		public void HighlightLine(HeaderType type, INode node)
		{
			var items = type == HeaderType.Columns ? Matrix.HeaderColumns : Matrix.HeaderRows;
			for (int i = 0; i < items.Count; i++) {
				if (items[i].Value.Equals(node)) {
					if (currentCell.X == i && type == HeaderType.Rows)
						return;
					if (currentCell.Y == i && type == HeaderType.Columns)
						return;
							
					currentCell = type == HeaderType.Columns ?
									new Coords(i, currentCell.Y) :
									new Coords(currentCell.X, i);
					
					SetHoveredCell();
					InvalidateVisual();
					
					return;
				}
			}
		}
		
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			
			var point = e.GetPosition(this);
			if (point.X < matrixWidth * CellWidth
			    && point.Y < matrixHeight * CellHeight)
				currentCell = new Coords(
					(int)((point.X + offset.X) / CellWidth),
					(int)((point.Y + offset.Y) / CellHeight));
			// else // if we are out of matrix just use last cell
			//	currentCell = new Coords(-1, -1);
			
			if (currentCell.X != HoveredCell.RowIndex ||
			    currentCell.Y != HoveredCell.ColumnIndex)
			{
				InvalidateVisual();
				SetHoveredCell();
			}
		}
		
		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			Relationship relationship = HoveredCell.Value as Relationship;
			Console.WriteLine("To: " + relationship.To.Name);
			Console.WriteLine("From:" + relationship.From.Name);
		}
		
		protected void SetHoveredCell()
		{
			HoveredCell.RowIndex = currentCell.Y;
			HoveredCell.ColumnIndex = currentCell.X;
			HoveredCell.Value = Matrix[HoveredCell.RowIndex, HoveredCell.ColumnIndex];
			if (HoveredCellChanged != null)
				HoveredCellChanged(this, new HoveredCellEventArgs<TValue>(HoveredCell));
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			// how much space we got for cells
			var maxWidth = ((int) viewport.Width / CellWidth) + 1;
			var maxHeight = ((int) viewport.Height / CellHeight) + 1;

			// how many cells we will draw
			// sometimes happens when half of cell is hidden in scroll so text isnt drawn
			// so lets drawn one more cell
			var cellsHorizontally = maxWidth >= matrixWidth ? matrixWidth : maxWidth + 1;
			var cellsVertically = maxHeight >= matrixHeight ? matrixHeight : maxHeight + 1;
			
			PageSizeWidth = cellsHorizontally;
			PageSizeHeight = cellsVertically;
			
			// number of cell which arent visible
			var scaledOffsetX = (int)offset.X / CellWidth;
			var scaledOffsetY = (int)offset.Y / CellHeight;

			// sets how much of cell on border should be drawn
			var offsetDiffX = offset.X - scaledOffsetX * CellWidth;
			var offsetDiffY = offset.Y - scaledOffsetY * CellHeight;
			
			// background
			var background = new Rect(0, 0, cellsHorizontally * CellWidth, cellsVertically * CellHeight);
			var backgroundColor = new SolidColorBrush(Colors.Yellow);
			backgroundColor.Freeze();
			drawingContext.DrawRectangle(backgroundColor, null, background);
			
			var currentXLine = (currentCell.X - scaledOffsetX) * CellWidth - offsetDiffX;
			var currentYLine = (currentCell.Y - scaledOffsetY) * CellHeight - offsetDiffY;

			// hovering
			if (currentCell.X >= 0 || currentCell.Y >= 0) {
				
				// hover y line
				var rect = new Rect(0,
				                    currentYLine,
				                    CellWidth * cellsHorizontally,
				                    CellHeight);
				
				var brush = new SolidColorBrush(Colors.GreenYellow);
				brush.Freeze();
				drawingContext.DrawRectangle(brush, null, rect);
				
				// hover x line
				rect = new Rect(currentXLine,
				                0,
				                CellWidth,
				                CellHeight * cellsVertically);
				
				brush = new SolidColorBrush(Colors.GreenYellow);
				brush.Freeze();
				drawingContext.DrawRectangle(brush, null, rect);
				
				// hover cell
				rect = new Rect(
					(currentCell.X - scaledOffsetX) * CellWidth - offsetDiffX,
					(currentCell.Y - scaledOffsetY) * CellHeight - offsetDiffY,
					CellWidth,
					CellHeight);

				brush = new SolidColorBrush(Colors.Red);
				brush.Freeze();
				drawingContext.DrawRectangle(brush, null, rect);
			}
			
			// text
			for (int i = 0; i < cellsHorizontally; i++) {
				for (int j = 0; j < cellsVertically; j++) { // dont draw text in unavailables places
					int rowIndex = j + scaledOffsetY;
					int columnIndex = i + scaledOffsetX;
					
					// adjust scales
					rowIndex = rowIndex >= Matrix.HeaderRows.Count ? rowIndex - 1 : rowIndex;
					columnIndex = columnIndex >= Matrix.HeaderColumns.Count ? columnIndex - 1 : columnIndex;
					
					var value = Matrix[rowIndex, columnIndex];
					
					if (Colorizer != null) {
						var rect = new Rect(
							i * CellWidth - offsetDiffX,
							j * CellHeight - offsetDiffY,
							CellWidth,
							CellHeight);

						SolidColorBrush brush = null;
						if ((i * CellWidth - offsetDiffX) == currentXLine ||
						    ((j * CellHeight - offsetDiffY) == currentYLine)) {
							var color = Colors.GreenYellow;
							if (currentCell.X == i && currentCell.Y == j) {
								color = color.MixedWith(Colors.Red);
							}
							brush = Colorizer.GetColorBrushMixedWith(color, value);
							
							
						}
						else
							brush = Colorizer.GetColorBrush(value);
						
						drawingContext.DrawRectangle(brush, null, rect);
					}
					
					if (!RenderZeroes && value != null && value.Text != "0") // rendering zeroes would be distracting
						drawingContext.DrawImage(
							CreateText(value.Text),
							new Rect(i * CellWidth - offsetDiffX, j * CellHeight - offsetDiffY, CellWidth, CellHeight));
				}
			}
			
			// grid
			var pen = new Pen(Brushes.Black, penSize);
			pen.Freeze();

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
		}
		
		public ImageSource CreateText(string text)
		{
			if (imgs.ContainsKey(text))
				return imgs[text];
			
			var bmp = new System.Drawing.Bitmap(CellWidth, CellHeight);
			var g = System.Drawing.Graphics.FromImage(bmp);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			
			var fontOjb = new System.Drawing.Font(font, fontSize);
			
			var size = g.MeasureString(text, fontOjb);
			
			var spanWidth = (CellWidth - size.Width) / 2;
			var spanHeight = (CellHeight - size.Height) / 2;
			
			g.DrawString(text, fontOjb, System.Drawing.Brushes.Black, new PointF(spanWidth, spanHeight));
			g.Dispose();
			
			var bitmap = bmp.GetHbitmap();
			var img = Imaging.CreateBitmapSourceFromHBitmap(bitmap,
			                                                IntPtr.Zero,
			                                                Int32Rect.Empty,
			                                                BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
			img.Freeze();
			imgs.Add(text, img);
			
			Helper.DeleteObject(bitmap);
			
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
			SetVerticalOffset(VerticalOffset - CellHeight);
		}
		
		public void LineDown()
		{
			SetVerticalOffset(VerticalOffset + CellHeight);
		}
		
		public void LineLeft()
		{
			SetHorizontalOffset(HorizontalOffset - CellWidth);
		}
		
		public void LineRight()
		{
			SetHorizontalOffset(HorizontalOffset + CellWidth);
		}
		
		public void PageUp()
		{
			SetVerticalOffset(VerticalOffset - CellHeight * PageSizeHeight);
		}
		
		public void PageDown()
		{
			SetVerticalOffset(VerticalOffset + CellHeight * PageSizeHeight);
		}
		
		public void PageLeft()
		{
			SetHorizontalOffset(HorizontalOffset - CellWidth * PageSizeWidth);
		}
		
		public void PageRight()
		{
			SetHorizontalOffset(HorizontalOffset + CellWidth * PageSizeWidth);
		}
		
		public void MouseWheelUp()
		{
			SetVerticalOffset(VerticalOffset - CellHeight);
		}
		
		public void MouseWheelDown()
		{
			SetVerticalOffset(VerticalOffset + CellHeight);
		}
		
		public void MouseWheelLeft()
		{
			SetVerticalOffset(HorizontalOffset - CellWidth);
		}
		
		public void MouseWheelRight()
		{
			SetVerticalOffset(HorizontalOffset + CellWidth);
		}
		
		public void SetHorizontalOffset(double offset)
		{
			if (offset == this.offset.X) return;
			this.offset.X = Math.Round(offset / CellWidth) * CellWidth;
			InvalidateVisual();
		}
		
		public void SetVerticalOffset(double offset)
		{
			if (offset == this.offset.Y) return;
			this.offset.Y = Math.Round(offset / CellHeight) * CellHeight;
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
	
	public class HoveredCell<TValue>
	{
		public int RowIndex { get; set; }
		public int ColumnIndex { get; set; }
		public TValue Value { get; set; }
	}
	
	public class HoveredCellEventArgs<TValue> : EventArgs
	{
		public HoveredCell<TValue> HoveredCell { get; set; }
		
		public HoveredCellEventArgs(HoveredCell<TValue> cell)
		{
			HoveredCell = cell;
		}
	}
}
