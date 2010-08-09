using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	public class MatrixControl<TValue> : FrameworkElement // TODO: Virtualization
	{
		Dictionary<string, ImageSource> imgs = new Dictionary<string, ImageSource>();
		Point currentPoint = new Point(-1, -1);
		string font;
		
		// will be loaded from Matrix
		int cellsVertically = 25;
		int cellsHorizontally = 25;
		
		public Matrix<TValue> Matrix { get; set; }
		
		public int CellHeight { get; set; }

		public int CellWidth { get; set; }
		
		public MatrixControl()
		{
			CellHeight = CellWidth = 36;
			font = "Verdana";
		}
		
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			
			var point = e.GetPosition(this);
			if (point.X < cellsHorizontally * CellWidth && point.Y < cellsVertically * CellHeight)
				currentPoint = point;
			else
				currentPoint = new Point(-1, -1);
			
			InvalidateVisual();
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			
			// background
			var background = new Rect(0, 0, cellsHorizontally * CellWidth, cellsVertically * CellHeight);
			var backgroundColor = new SolidColorBrush(Colors.Yellow);
			drawingContext.DrawRectangle(backgroundColor, null, background);

			// hover cell
			if (currentPoint.X > 0 || currentPoint.Y > 0) {
				var rect = new Rect(currentPoint.X - currentPoint.X % CellWidth, currentPoint.Y - currentPoint.Y % CellHeight, CellWidth, CellHeight);
				var brush = new SolidColorBrush(Colors.Red);
				drawingContext.DrawRectangle(brush, null, rect);
			}
			
			// grid
			var pen = new Pen(Brushes.Black, 1);

			for (int i = 0; i <= cellsHorizontally; i++) {
				drawingContext.DrawLine(pen,
				                        new Point(i * CellWidth, 0),
				                        new Point(i * CellWidth,
				                                  cellsHorizontally * CellWidth));
			}

			for (int i = 0; i <= cellsVertically; i++) {
				drawingContext.DrawLine(pen,
				                        new Point(0, i * CellHeight),
				                        new Point(cellsVertically * CellHeight,
				                                  i * CellHeight));
			}
			
			// text
			for (int i = 0; i < cellsHorizontally; i++) {
				for (int j = 0; j < cellsVertically; j++) {
					drawingContext.DrawImage(CreateText(i * j), new Rect(i * CellWidth, j * CellHeight, CellWidth, CellHeight));
				}
			}
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
			
			float spanWidth = CellWidth / 2 - size.Width / 2;
			float spanHeight = CellHeight / 2 - size.Height / 2;
			
			g.DrawString(text, fontOjb, System.Drawing.Brushes.Black, new System.Drawing.PointF(spanWidth, spanHeight));
			g.Dispose();
			
			var img = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), 
			                                                                       IntPtr.Zero, 
			                                                                       System.Windows.Int32Rect.Empty, 
			                                                                       BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
			imgs.Add(text, img);
			
			return img;
		}
	}
}
