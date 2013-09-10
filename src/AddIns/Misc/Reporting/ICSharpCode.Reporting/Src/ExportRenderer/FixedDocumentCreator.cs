/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.05.2013
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using Pen = System.Windows.Media.Pen;
using Size = System.Windows.Size;
using ICSharpCode.Reporting.BaseClasses;

namespace ICSharpCode.Reporting.ExportRenderer
{
	/// <summary>
	/// Description of FixedDocumentCreator.
	/// </summary>
	class FixedDocumentCreator
	{
		BrushConverter brushConverter ;
		
		public FixedDocumentCreator()
		{
			brushConverter = new BrushConverter();
		}
		
		
		public static UIElement CreateFixedPage(ExportPage exportPage) {
			var fixedPage = new FixedPage();
			fixedPage.Width = exportPage.Size.ToWpf().Width;
			fixedPage.Height = exportPage.Size.ToWpf().Height;
	
//			fixedPage.Background = ConvertBrush(System.Drawing.Color.Blue);
			fixedPage.Background = new SolidColorBrush(System.Drawing.Color.Blue.ToWpf());
			return fixedPage;
		}
			
		
		public  UIElement CreateContainer(ExportContainer container)
		{
			var canvas = CreateCanvas(container);
			var size = container.DesiredSize.ToWpf();
			canvas.Measure(size);
			
			canvas.Arrange(new Rect(new Point(),size ));

			canvas.UpdateLayout();

			return canvas;
			
		}
		
		
		public TextBlock CreateTextBlock(ExportText exportText)
		{
			var textBlock = new TextBlock();
			textBlock.Foreground = ConvertBrush(exportText.ForeColor);
//			textBlock.Background = ConvertBrush(exportText.BackColor);
			textBlock.Background = ConvertBrush(System.Drawing.Color.LightGray);
		
			SetFont(textBlock,exportText);
			
			textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
			
			string [] inlines = exportText.Text.Split(System.Environment.NewLine.ToCharArray());
	
			for (int i = 0; i < inlines.Length; i++) {
				if (inlines[i].Length > 0) {
					textBlock.Inlines.Add(new Run(inlines[i]));
					textBlock.Inlines.Add(new LineBreak());
				}
			}
			var li = textBlock.Inlines.LastInline;
			textBlock.Inlines.Remove(li);
		
			var s = MeasureTextInWpf(exportText);
			textBlock.Width = s.Width;
			textBlock.Height = s.Height;
			
//		    textBlock.Background = ConvertBrush(exportText.StyleDecorator.BackColor);
//		    SetContendAlignment(textBlock,exportText.StyleDecorator);
			
			return textBlock;
		}
		
		
		static Size MeasureTextInWpf(ExportText exportText)
		{
			if (exportText.CanGrow) {
				
			
			FormattedText ft = new FormattedText(exportText.Text,
			                                     CultureInfo.CurrentCulture,
			                                     System.Windows.FlowDirection.LeftToRight,
			                                     new Typeface(exportText.Font.FontFamily.Name),
			                                     exportText.Font.Size,
//			                                     System.Windows.Media.Brushes.Black,
			                                     new SolidColorBrush(exportText.ForeColor.ToWpf()),
			                                     
			                                     null,
			                                     TextFormattingMode.Display);
			
			ft.MaxTextWidth = exportText.Size.Width * 96.0 / 72.0;
			ft.MaxTextHeight = Double.MaxValue ;
			
			ft.SetFontSize(exportText.Font.Size  * 96.0 / 72.0);
		
			var ss = new Size {
				Width = ft.WidthIncludingTrailingWhitespace,
				Height = ft.Height};
			return ss;
			}
			return new Size(exportText.Size.Width,exportText.Size.Height);
		}
		
		
		Canvas CreateCanvas(ExportContainer container)
		{
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
		
		static void SetDimension (FrameworkElement element,IExportColumn exportColumn)
		{
			element.Width = exportColumn.DesiredSize.Width;
			element.Height = exportColumn.DesiredSize.Height;
		}
		
		
		static void SetPosition (UIElement element,IExportColumn exportColumn) {
			FixedPage.SetLeft(element,exportColumn.Location.X );
			FixedPage.SetTop(element,exportColumn.Location.Y);
		}
		
		
		void SetFont(TextBlock textBlock,IExportText exportText)
		{
			textBlock.FontFamily = new FontFamily(exportText.Font.FontFamily.Name);

//http://www.codeproject.com/Articles/441009/Drawing-Formatted-Text-in-a-Windows-Forms-Applicat
			
textBlock.FontSize = exportText.Font.Size * 96/72;

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
		
		
		void CreateStrikeout (TextBlock textBlock,IExportText exportColumn )
		{
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
		
		
		void CreateUnderline(TextBlock textBlock,IExportText exportColumn)
		{
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
		
		
		Pen CreateWpfPen(IReportObject exportColumn)
		{
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var myPen = new Pen();
			myPen.Brush = ConvertBrush(exportColumn.ForeColor);
			myPen.Thickness = 1.5;
			return myPen;
		}
		
		
		Brush ConvertBrush(System.Drawing.Color color)
		{
			if (brushConverter.IsValid(color.Name)){
				return brushConverter.ConvertFromString(color.Name) as SolidColorBrush;
			} else{
				return brushConverter.ConvertFromString("Black") as SolidColorBrush;
			}
		}
	}
}
