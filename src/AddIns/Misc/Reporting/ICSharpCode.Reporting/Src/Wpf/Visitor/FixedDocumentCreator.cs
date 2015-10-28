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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using Pen = System.Windows.Media.Pen;
using Size = System.Windows.Size;

namespace ICSharpCode.Reporting.WpfReportViewer.Visitor
{
	/// <summary>
	/// Description of FixedDocumentCreator.
	/// </summary>
	static class FixedDocumentCreator
	{
		
		public static FixedPage CreateFixedPage(ExportPage exportPage) {
			var fixedPage = new FixedPage();
			fixedPage.Width = exportPage.Size.ToWpf().Width;
			fixedPage.Height = exportPage.Size.ToWpf().Height;
			fixedPage.Background = new SolidColorBrush(System.Drawing.Color.White.ToWpf());
			return fixedPage;
		}
		

		public static Canvas CreateContainer(ExportContainer container)	{
			var canvas = CreateCanvas(container);
			var size = container.DesiredSize.ToWpf();
			CanvasHelper.SetPosition(canvas,new Point(container.Location.X,container.Location.Y));
			canvas.Measure(size);
			canvas.Arrange(new Rect(new Point(),size ));
			canvas.UpdateLayout();
			return canvas;
		}
		
		
		public static FormattedText CreateFormattedText(ExportText exportText)
		{
			var culture = CultureInfo.CurrentCulture;
			var flowDirection = culture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
			var emSize = ExtensionMethodes.ToPoints((int)exportText.Font.SizeInPoints +1);
			
			var formattedText = new FormattedText(exportText.Text,
				CultureInfo.CurrentCulture,
				flowDirection,
				new Typeface(exportText.Font.FontFamily.Name),
				emSize,
				new SolidColorBrush(exportText.ForeColor.ToWpf()), null, TextFormattingMode.Display);
			
			formattedText.MaxTextWidth = exportText.DesiredSize.Width ;
			formattedText.TextAlignment = exportText.TextAlignment;
		
			if (!exportText.CanGrow) {
				formattedText.MaxTextHeight = exportText.Size.Height;
			} else {
				formattedText.MaxTextHeight = ExtensionMethodes.ToPoints(exportText.DesiredSize.Height );
			}
			
			ApplyPrintStyles(formattedText,exportText);
		
			return formattedText;
		}


		static void ApplyPrintStyles (FormattedText formattedText,ExportText exportText) {
			var font = exportText.Font;
			var textDecorations = new TextDecorationCollection();
			FontStyle fontStyle;
			FontWeight fontWeight;
			
			if ((font.Style & System.Drawing.FontStyle.Italic) != 0) {
				fontStyle = FontStyles.Italic;
			} else {
				fontStyle = FontStyles.Normal;
			}
			
			formattedText.SetFontStyle(fontStyle);
			
			if ((font.Style & System.Drawing.FontStyle.Bold) != 0) {
				fontWeight = FontWeights.Bold;
			} else {
				fontWeight = FontWeights.Normal;
			}
			formattedText.SetFontWeight(fontWeight);
			
			if ((font.Style & System.Drawing.FontStyle.Underline) != 0) {
				textDecorations.Add(TextDecorations.Underline);
			}
			
			if ((font.Style & System.Drawing.FontStyle.Strikeout) != 0) {
				textDecorations.Add(TextDecorations.Strikethrough);
			}
			
			formattedText.SetTextDecorations(textDecorations);
		}
			
		
		static Canvas CreateCanvas(ExportContainer container){
			var canvas = new Canvas();
			SetPositionAndSize(canvas,container);

			canvas.Name = container.Name;
			canvas.Background = ConvertBrush(container.BackColor);
			return canvas;
		}
		
		
		static void SetPositionAndSize(FrameworkElement element,ExportColumn column) {
			if (column == null)
				throw new ArgumentNullException("column");
			SetPosition(element,column);
			SetDimension(element,column);	
		}
		
		
		static void SetDimension (FrameworkElement element,IExportColumn exportColumn){
			element.Width = exportColumn.DesiredSize.Width;
			element.Height = exportColumn.DesiredSize.Height;
		}
		
		
		static void SetPosition (UIElement element,IExportColumn exportColumn) {
			FixedPage.SetLeft(element,exportColumn.Location.X );
			FixedPage.SetTop(element,exportColumn.Location.Y);
		}
		
		
		/*
		public static Point CalculateAlignmentOffset (FormattedText formattedText, ExportText exportText) {
			var offset = new Point(0,0);
			double y = 0;
			double x = 0;
			var textLenDif = exportText.Size.Width - formattedText.Width;
			var textHeightDif = exportText.Size.Height - formattedText.Height;
			
			switch (exportText.ContentAlignment) {
				// Top	
				case System.Drawing.ContentAlignment.TopLeft:
					break;
					
				case System.Drawing.ContentAlignment.TopCenter:
					x = textLenDif / 2;
					break;
					
				case System.Drawing.ContentAlignment.TopRight:
					x = textLenDif;
					break;
					
					// Middle
				case System.Drawing.ContentAlignment.MiddleLeft:
					y = textHeightDif / 2;
					break;
					
				case System.Drawing.ContentAlignment.MiddleCenter:
					y = textHeightDif / 2;
					x = textLenDif / 2;
					break;
					
				case System.Drawing.ContentAlignment.MiddleRight:
					x = textLenDif;;
					y = textHeightDif / 2;
					break;
					
					//Bottom
				case System.Drawing.ContentAlignment.BottomLeft:
					x = 0;
					y = textHeightDif;
					break;
					
				case System.Drawing.ContentAlignment.BottomCenter:
					x = textLenDif / 2;
					y = textHeightDif;
					break;
					
				case System.Drawing.ContentAlignment.BottomRight:
					x = textLenDif;
					y  = textHeightDif;
					break;
			}
			return new Point(x,y);
		}
		*/
		
		public static Pen CreateWpfPen(IReportObject exportColumn){
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var pen = new Pen();
			pen.Brush = ConvertBrush(exportColumn.ForeColor);
			pen.Thickness = 1;
			
			var exportGraphics = exportColumn as IExportGraphics;
			if (exportGraphics != null) {
				pen.Thickness = exportGraphics.Thickness;
				pen.DashStyle = DashStyle(exportGraphics);
				pen.StartLineCap = LineCap(exportGraphics.StartLineCap);
				pen.EndLineCap = LineCap(exportGraphics.EndLineCap);
			}
			return pen;
		}
		
		
		public static Brush ConvertBrush(System.Drawing.Color color)
		{
			var b = new BrushConverter();
			if (b.IsValid(color.Name)) {
				return b.ConvertFromString(color.Name) as SolidColorBrush;
			}
			return b.ConvertFromString("Black") as SolidColorBrush;
		}
		
		
		public static PenLineCap LineCap (System.Drawing.Drawing2D.LineCap lineCap) {
			var penLineCap = PenLineCap.Flat;
			switch (lineCap) {
				case System.Drawing.Drawing2D.LineCap.Flat:
					penLineCap = PenLineCap.Flat;
					break;
				case System.Drawing.Drawing2D.LineCap.Square:
					penLineCap = PenLineCap.Square;
					break;
				case System.Drawing.Drawing2D.LineCap.Round:
					penLineCap = PenLineCap.Round;
					break;
				case System.Drawing.Drawing2D.LineCap.Triangle:
					penLineCap = PenLineCap.Triangle;
					break;
				case System.Drawing.Drawing2D.LineCap.NoAnchor:
					penLineCap = PenLineCap.Flat;
					break;
				case System.Drawing.Drawing2D.LineCap.SquareAnchor:
					penLineCap = PenLineCap.Flat;
					break;
				case System.Drawing.Drawing2D.LineCap.RoundAnchor:
					penLineCap = PenLineCap.Flat;
					break;
				case System.Drawing.Drawing2D.LineCap.DiamondAnchor:
					penLineCap = PenLineCap.Flat;
					break;
				case System.Drawing.Drawing2D.LineCap.ArrowAnchor:
					penLineCap = PenLineCap.Flat;
					break;
				case System.Drawing.Drawing2D.LineCap.Custom:
					penLineCap = PenLineCap.Flat;
					break;
				case System.Drawing.Drawing2D.LineCap.AnchorMask:
					
					break;
				default:
					throw new Exception("Invalid value for LineCap");
					
			}
			return penLineCap;
		}
		
		
		public static DashStyle DashStyle (IExportGraphics exportGraphics) {
			var dashStyle = DashStyles.Solid;
			
			switch (exportGraphics.DashStyle) {
				case System.Drawing.Drawing2D.DashStyle.Solid:
					dashStyle = DashStyles.Solid;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dash:
					dashStyle = DashStyles.Dash;
					break;
				case System.Drawing.Drawing2D.DashStyle.Dot:
					dashStyle = DashStyles.Dot;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDot:
					dashStyle = DashStyles.DashDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.DashDotDot:
					dashStyle = DashStyles.DashDotDot;
					break;
				case System.Drawing.Drawing2D.DashStyle.Custom:
					dashStyle = DashStyles.Solid;
					break;
				default:
					throw new Exception("Invalid value for DashStyle");
			}
			return dashStyle;
		}
		
	}
}
