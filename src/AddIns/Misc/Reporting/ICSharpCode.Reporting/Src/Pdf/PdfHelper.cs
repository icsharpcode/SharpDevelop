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
			var rect = PdfHelper.CreateDisplayRectangle(columnLocation, exportColumn.DesiredSize);
			textFormatter.DrawString(exportColumn.Text, font, XBrushes.Black, rect, XStringFormats.TopLeft);	
		}
		
		
		static XFont CreatePdfFont(IExportColumn exportColumn)
		{
			var textColumn = (ExportText)exportColumn;
			return new XFont(textColumn.Font.FontFamily.Name, textColumn.Font.Size);
		}
		
		
		public static XRect CreateDisplayRectangle(IExportColumn column) {
			return new XRect(column.DisplayRectangle.Location.ToXPoints(),
			                 column.DisplayRectangle.Size.ToXSize());
		}
		
		
		public static XRect CreateDisplayRectangle(Point location,Size size) {
			return new XRect(location.ToXPoints(),size.ToXSize());
		}
		
		
		public static void DrawRectangle (IExportColumn column, XGraphics graphics) {
			var c = XColor.FromArgb(column.FrameColor.R,column.FrameColor.G,column.FrameColor.B);
			var pen = new XPen(c, 1);
			var r = CreateDisplayRectangle(column);
			graphics.DrawRectangle(pen,r);
		}
		
		
		
		
		public static void DrawRectangle(Rectangle rect,Color color,XGraphics graphics) {
			var c = XColor.FromArgb(color.R,color.G,color.B);
			var pen = new XPen(c, 1);
			var r = CreateDisplayRectangle(rect.Location,rect.Size);
			graphics.DrawRectangle(pen,r);
		}
	}
}
