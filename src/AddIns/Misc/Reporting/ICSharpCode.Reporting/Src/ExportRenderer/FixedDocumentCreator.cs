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

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using Pen = System.Windows.Media.Pen;
using Size = System.Windows.Size;

namespace ICSharpCode.Reporting.ExportRenderer
{
	/// <summary>
	/// Description of FixedDocumentCreator.
	/// </summary>
	class FixedDocumentCreator
	{
		private readonly BrushConverter brushConverter ;
		
		public FixedDocumentCreator()
		{
			brushConverter = new BrushConverter();
		}
		
		public static FixedPage CreateFixedPage(ExportPage exportPage) {
			var fixedPage = new FixedPage();
			fixedPage.Width = exportPage.Size.ToWpf().Width;
			fixedPage.Height = exportPage.Size.ToWpf().Height;
	
//			fixedPage.Background = ConvertBrush(System.Drawing.Color.Blue);
			fixedPage.Background = new SolidColorBrush(System.Drawing.Color.Blue.ToWpf());
			return fixedPage;
		}
		

		public  Canvas CreateContainer(ExportContainer container)	{
			var canvas = CreateCanvas(container);
			var size = container.DesiredSize.ToWpf();
			canvas.Measure(size);
			canvas.Arrange(new Rect(new Point(),size ));
			canvas.UpdateLayout();
			return canvas;
		}
		
		
		public TextBlock CreateTextBlock(ExportText exportText,bool setBackcolor){
			var textBlock = new TextBlock();
			
			textBlock.Foreground = ConvertBrush(exportText.ForeColor);
			
			if (setBackcolor) {
				textBlock.Background = ConvertBrush(exportText.BackColor);
			}
			
//			textBlock.Background = ConvertBrush(System.Drawing.Color.LightGray);
		
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
		SetContentAlignment(textBlock,exportText);
			var wpfSize = MeasureTextInWpf(exportText);
			textBlock.Width = wpfSize.Width;
			textBlock.Height = wpfSize.Height;
			
//		    textBlock.Background = ConvertBrush(exportText.StyleDecorator.BackColor);
//		    SetContendAlignment(textBlock,exportText.StyleDecorator);
			if (exportText.ContentAlignment != System.Drawing.ContentAlignment.TopLeft) {
				Console.WriteLine("----Aliogn --------{0}",exportText.ContentAlignment.ToString());
			}
//			SetContentAlignment(textBlock,exportText);
			return textBlock;
		}
		
		
		static Size MeasureTextInWpf(ExportText exportText){
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
		
		
		Canvas CreateCanvas(ExportContainer container){
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
		
		
		void SetFont(TextBlock textBlock,IExportText exportText){
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
		
		
		void SetContentAlignment(TextBlock textBlock,ExportText exportText)
		{
	//	http://social.msdn.microsoft.com/Forums/vstudio/en-US/e480abb9-a86c-4f78-8955-dddb866bcfef/vertical-text-alignment-in-textblock?forum=wpf	
	//Vertical alignment not working
	
			Console.WriteLine("align {0}",exportText.ContentAlignment);
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
		/*
		private void SetContendAlignment(TextBlock textBlock,TextStyleDecorator decorator)
		{
			switch (decorator.ContentAlignment)
			{
				case ContentAlignment.TopLeft:
					textBlock.VerticalAlignment = VerticalAlignment.Top;
					textBlock.TextAlignment = TextAlignment.Left;
					break;
				case ContentAlignment.TopCenter:
					textBlock.VerticalAlignment = VerticalAlignment.Top;
					textBlock.TextAlignment = TextAlignment.Center;
					break;
				case ContentAlignment.TopRight:
					textBlock.VerticalAlignment = VerticalAlignment.Top;
					textBlock.TextAlignment = TextAlignment.Right;
					break;
					// Middle
				case ContentAlignment.MiddleLeft:
					textBlock.VerticalAlignment = VerticalAlignment.Center;
					textBlock.TextAlignment = TextAlignment.Left;
					break;
				case ContentAlignment.MiddleCenter:
					textBlock.VerticalAlignment = VerticalAlignment.Center;
					textBlock.TextAlignment = TextAlignment.Center;
					break;
				case ContentAlignment.MiddleRight:
					textBlock.VerticalAlignment = VerticalAlignment.Center;
					textBlock.TextAlignment = TextAlignment.Right;
					break;
					//Bottom
				case ContentAlignment.BottomLeft:
					textBlock.VerticalAlignment = VerticalAlignment.Bottom;
					textBlock.TextAlignment = TextAlignment.Left;
					break;
				case ContentAlignment.BottomCenter:
					textBlock.VerticalAlignment = VerticalAlignment.Bottom;
					textBlock.TextAlignment = TextAlignment.Center;
					break;
				case ContentAlignment.BottomRight:
					textBlock.VerticalAlignment = VerticalAlignment.Bottom;
					textBlock.TextAlignment = TextAlignment.Right;
					break;
			}
		}
		*/
		void CreateStrikeout (TextBlock textBlock,IExportText exportColumn ){
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
		
		
		void CreateUnderline(TextBlock textBlock,IExportText exportColumn){
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
		
		
		Pen CreateWpfPen(IReportObject exportColumn){
			if (exportColumn == null)
				throw new ArgumentNullException("exportColumn");
			var myPen = new Pen();
			myPen.Brush = ConvertBrush(exportColumn.ForeColor);
			myPen.Thickness = 1.5;
			return myPen;
		}
		
		
		Brush ConvertBrush(System.Drawing.Color color){
			if (brushConverter.IsValid(color.Name)){
				return brushConverter.ConvertFromString(color.Name) as SolidColorBrush;
			} else{
				return brushConverter.ConvertFromString("Black") as SolidColorBrush;
			}
		}
	}
}
