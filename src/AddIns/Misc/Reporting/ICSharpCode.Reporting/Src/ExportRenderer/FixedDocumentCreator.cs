/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.05.2013
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
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
		
		
		public UIElement CreateFixedPage(ExportPage exportPage) {
			var fixedPage = new FixedPage();
			fixedPage.Width = exportPage.Size.Width;
			fixedPage.Height = exportPage.Size.Height;
			fixedPage.Background = ConvertBrush(System.Drawing.Color.Blue);
			return fixedPage;
		}
			
		
		public  UIElement CreateContainer(ExportContainer container)
		{
			var canvas = CreateCanvas(container);
			
			var size = new Size(container.DesiredSize.Width,container.DesiredSize.Height);
			
			canvas.Measure(size);
			
			canvas.Arrange(new Rect(new Point(),size ));

			canvas.UpdateLayout();
			
			return canvas;
			
		}
		
		
		public TextBlock CreateTextBlock(ExportText exportText)
		{
			var textBlock = new TextBlock();
		
//			textBlock.Text = exportText.Text;
		textBlock.Width = exportText.DesiredSize.Width;
		textBlock.Height = exportText.DesiredSize.Height;
		
			textBlock.Foreground = ConvertBrush(exportText.ForeColor);
			SetFont(textBlock,exportText);
			textBlock.Background = ConvertBrush(exportText.BackColor);
			
			textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
//			textBlock.TextWrapping = TextWrapping.NoWrap;
			string [] inlines = exportText.Text.Split(System.Environment.NewLine.ToCharArray());
			//string [] inlines = "jmb,.n,knn-.n.-n.n-.n.n.-";
			for (int i = 0; i < inlines.Length; i++) {
				if (inlines[i].Length > 0) {
					textBlock.Inlines.Add(new Run(inlines[i]));
							textBlock.Inlines.Add(new LineBreak());
				}
			}
			var li = textBlock.Inlines.LastInline;
			textBlock.Inlines.Remove(li);
//			SetDimension(textBlock,exportText.StyleDecorator);
//		    textBlock.Background = ConvertBrush(exportText.StyleDecorator.BackColor);
//		    SetContendAlignment(textBlock,exportText.StyleDecorator);
			
			return textBlock;
		}
		
		
		Canvas CreateCanvas(ExportContainer container)
		{
			var canvas = new Canvas();
			SetPositionAndSize(canvas,container);

			canvas.Name = container.Name;
			canvas.Background = ConvertBrush(container.BackColor);
			return canvas;
		}
		
		void SetPositionAndSize(FrameworkElement element,ExportColumn column) {
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
