// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Drawing;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

namespace ICSharpCode.Reporting.Pdf
{
	/// <summary>
	/// Description of PdfHelper.
	/// </summary>
	public static class PdfHelper
	{
		
		public static void WriteText(XTextFormatter textFormatter,Point columnLocation, ExportText exportColumn)
		{
			XFont font = PdfHelper.CreatePdfFont(exportColumn);
			var rect = new Rectangle(columnLocation,exportColumn.DesiredSize).ToXRect();
			textFormatter.DrawString(exportColumn.Text,
			                         font,
			                         new XSolidBrush(ToXColor(exportColumn.ForeColor)),
			                         rect, XStringFormats.TopLeft);
		}
		
		
		static XFont CreatePdfFont(IExportColumn exportColumn)
		{
			var textColumn = (ExportText)exportColumn;
			return new XFont(textColumn.Font.FontFamily.Name, textColumn.Font.Size);
		}
		
		
		static XColor ToXColor (Color color){
			return XColor.FromArgb(color.R,color.G,color.B);
		}
		
		
		public static XRect CreateDisplayRectangle(IExportColumn column) {
			return column.DisplayRectangle.ToXRect();
		}
		
		
		public static void DrawRectangle (IExportColumn column, XGraphics graphics) {
			FillRectangle(column.DisplayRectangle,column.FrameColor,graphics);
		}
		
		
		public static void FillRectangle(Rectangle rect,Color color,XGraphics graphics) {
			var r = rect.ToXRect();
			graphics.DrawRectangle(new XSolidBrush(ToXColor(color)),r);
		}
		
		
		public static XPen PdfPen(IExportGraphics column) {
			return new XPen(ToXColor(column.ForeColor),column.Thickness);
		}
		
		
		public static Point LocationRelToParent (ExportColumn column) {
			return new Point(column.Parent.Location.X + column.Location.X,
			                 column.Parent.Location.Y + column.Location.Y);
		}
	}
}
