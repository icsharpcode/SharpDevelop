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
			canvas.Measure(size);
			canvas.Arrange(new Rect(new Point(),size ));
			canvas.UpdateLayout();
			return canvas;
		}

		/*
		public static TextBlock CreateTextBlock(ExportText exportText,bool setBackcolor){
			
			var textBlock = new TextBlock();

			textBlock.Foreground = ConvertBrush(exportText.ForeColor);
			
			if (setBackcolor) {
				textBlock.Background = ConvertBrush(exportText.BackColor);
			}
			 
			SetFont(textBlock,exportText);
			
			textBlock.TextWrapping = TextWrapping.Wrap;
			
			CheckForNewLine (textBlock,exportText);
			SetContentAlignment(textBlock,exportText);
			MeasureTextBlock (textBlock,exportText);
			return textBlock;
		}
		*/
		
		/*
		static void CheckForNewLine(TextBlock textBlock,ExportText exportText) {
			string [] inlines = exportText.Text.Split(Environment.NewLine.ToCharArray());
			for (int i = 0; i < inlines.Length; i++) {
				if (inlines[i].Length > 0) {
					textBlock.Inlines.Add(new Run(inlines[i]));
					textBlock.Inlines.Add(new LineBreak());
				}
			}
			var li = textBlock.Inlines.LastInline;
			textBlock.Inlines.Remove(li);
		}
		
		
		static void MeasureTextBlock(TextBlock textBlock,ExportText exportText)
		{
			var wpfSize = MeasureTextInWpf(exportText);
			textBlock.Width = wpfSize.Width;
			textBlock.Height = wpfSize.Height;
		}	
		
		*/
		
		/*
		static Size MeasureTextInWpf(ExportText exportText){
			
			if (exportText.CanGrow) {
				var formattedText = NewMethod(exportText);
				
				formattedText.MaxTextWidth = exportText.DesiredSize.Width * 96.0 / 72.0;
				
				formattedText.SetFontSize(Math.Floor(exportText.Font.Size  * 96.0 / 72.0));
				
				var size = new Size {
					Width = formattedText.WidthIncludingTrailingWhitespace,
					Height = formattedText.Height + 6};
				return size;
			}
			return new Size(exportText.Size.Width,exportText.Size.Height);
		}

		*/
		
		
		public static FormattedText CreateFormattedText(ExportText exportText)
		{
			var formattedText = new FormattedText(exportText.Text,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(exportText.Font.FontFamily.Name),
				exportText.Font.Size,
				new SolidColorBrush(exportText.ForeColor.ToWpf()), null, TextFormattingMode.Display);
			
			formattedText.MaxTextWidth = exportText.DesiredSize.Width * 96.0 / 72.0;
			formattedText.SetFontSize(Math.Floor(exportText.Font.Size  * 96.0 / 72.0));
			
			var td = new TextDecorationCollection()	;
			CheckUnderline(td,exportText);
			formattedText.SetTextDecorations(td);
			return formattedText;
		}

		static void CheckUnderline(TextDecorationCollection td, ExportText exportText)
		{
			if (exportText.Font.Underline) {
				td.Add(new TextDecoration{Location = TextDecorationLocation.Underline});
			}
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
		static void SetFont(TextBlock textBlock,IExportText exportText){
			textBlock.FontFamily = new FontFamily(exportText.Font.FontFamily.Name);

			//http://www.codeproject.com/Articles/441009/Drawing-Formatted-Text-in-a-Windows-Forms-Applicat
			
			textBlock.FontSize = Math.Floor(exportText.Font.Size * 96/72);

			if (exportText.Font.Bold) {
				textBlock.FontWeight = FontWeights.Bold;
			}
			if (exportText.Font.Underline) {
				CreateUnderline(textBlock,exportText);
			}
			
			if (exportText.Font.Italic) {
				textBlock.FontStyle = FontStyles.Italic ;
			}
			if (exportText.Font.Strikeout) {
				CreateStrikeout(textBlock,exportText);
			}
		}
		*/
		
		static void SetContentAlignment(TextBlock textBlock,ExportText exportText)
		{
	//	http://social.msdn.microsoft.com/Forums/vstudio/en-US/e480abb9-a86c-4f78-8955-dddb866bcfef/vertical-text-alignment-in-textblock?forum=wpf	
	//Vertical alignment not working
	
			switch (exportText.ContentAlignment) {
				case System.Drawing.ContentAlignment.TopLeft:
					textBlock.VerticalAlignment = VerticalAlignment.Top;
					textBlock.TextAlignment = TextAlignment.Left;
					break;
				case System.Drawing.ContentAlignment.TopCenter:
					textBlock.VerticalAlignment = VerticalAlignment.Top;
					textBlock.TextAlignment = TextAlignment.Center;
					break;
				case System.Drawing.ContentAlignment.TopRight:
					textBlock.VerticalAlignment = VerticalAlignment.Top;
					textBlock.TextAlignment = TextAlignment.Right;
					break;
					// Middle
				case System.Drawing.ContentAlignment.MiddleLeft:
					textBlock.VerticalAlignment = VerticalAlignment.Center;
					textBlock.TextAlignment = TextAlignment.Left;
					break;
				case System.Drawing.ContentAlignment.MiddleCenter:
					textBlock.VerticalAlignment = VerticalAlignment.Center;
					textBlock.TextAlignment = TextAlignment.Center;
					break;
				case System.Drawing.ContentAlignment.MiddleRight:
					textBlock.VerticalAlignment = VerticalAlignment.Center;
					textBlock.TextAlignment = TextAlignment.Right;
					break;
					//Bottom
				case System.Drawing.ContentAlignment.BottomLeft:
					textBlock.VerticalAlignment = VerticalAlignment.Bottom;
					textBlock.TextAlignment = TextAlignment.Left;
					break;
				case System.Drawing.ContentAlignment.BottomCenter:
					textBlock.VerticalAlignment = VerticalAlignment.Bottom;
					textBlock.TextAlignment = TextAlignment.Center;
					break;
				case System.Drawing.ContentAlignment.BottomRight:
					textBlock.VerticalAlignment = VerticalAlignment.Bottom;
					textBlock.TextAlignment = TextAlignment.Right;
					break;
			}
		}
		
		
		static void CreateStrikeout (TextBlock textBlock,IExportText exportColumn ){
			if (textBlock == null)
				throw new ArgumentNullException("textBlock");
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var strikeOut = new TextDecoration();
			strikeOut.Location = TextDecorationLocation.Strikethrough;

			Pen p = CreateWpfPen(exportColumn);
			strikeOut.Pen = p ;
			strikeOut.PenThicknessUnit = TextDecorationUnit.FontRecommended;
			textBlock.TextDecorations.Add(strikeOut);
		}
		
		/*
		static void CreateUnderline(TextBlock textBlock,IExportText exportColumn){
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			if (textBlock == null)
				throw new ArgumentNullException("textBlock");
			var underLine = new TextDecoration();
			Pen p = CreateWpfPen(exportColumn);
			underLine.Pen = p ;
			underLine.PenThicknessUnit = TextDecorationUnit.FontRecommended;
			textBlock.TextDecorations.Add(underLine);
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
				pen.DashStyle = FixedDocumentCreator.DashStyle(exportGraphics);
				pen.StartLineCap = FixedDocumentCreator.LineCap(exportGraphics.StartLineCap);
				pen.EndLineCap = FixedDocumentCreator.LineCap(exportGraphics.EndLineCap);
			}
			return pen;
		}
		
		
		public static Brush ConvertBrush(System.Drawing.Color color){
			var b = new BrushConverter();
			if (b.IsValid(color.Name)){
				return b.ConvertFromString(color.Name) as SolidColorBrush;
			} else{
				return b.ConvertFromString("Black") as SolidColorBrush;
			}
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
