/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 10.05.2011
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Core.WPF
{
	/// <summary>
	/// Description of FixedDocumentCreator.
	/// </summary>
	public class FixedDocumentCreator
	{
		
		BrushConverter brushConverter ;
		
		public FixedDocumentCreator()
		{
			brushConverter = new BrushConverter();
		}
		
		
		public FixedPage CreatePage(ExporterPage exporterPage)
		{
			FixedPage page = new FixedPage();
			CreatePageInternal (page,exporterPage.Items);
			return page;
		}
		
		
		void CreatePageInternal(FixedPage page, ExporterCollection items)
		{
			foreach (var element in items)
			{
				var item = ItemFactory(element);
				if (item != null) {
					FixedPage.SetLeft(item,element.StyleDecorator.Location.X );
					FixedPage.SetTop(item,element.StyleDecorator.Location.Y);
					page.Children.Add(item);
				}
			}
		}
		
		
		UIElement ItemFactory (BaseExportColumn column)
		{
			UIElement element = null;
			System.Windows.Controls.Border border = null;
			
			
			var container = column as ExportContainer;
			if (container != null) {
				element = CreateContainer(container);
			}
			
			var text = column as ExportText;
			
			if (text != null) {
				
				if (column.StyleDecorator.DrawBorder) {
					border = new System.Windows.Controls.Border();
					border.Padding = new Thickness(1);
					border.BorderThickness = new Thickness(2);
					border.CornerRadius = new CornerRadius(2);
					border.BorderBrush = brushConverter.ConvertFromString(column.StyleDecorator.ForeColor.Name) as SolidColorBrush;
					var t = CreateTextBlock(text);
					border.Child = t;
					element = border;
				} else {
					
					element = CreateTextBlock (text);
				}
			}
			

			var image = column as ExportImage;
			
			if (image != null)
			{
				element = CreateImageColumn(image);
			}
			
			
			return element;
		}
		
		
		private UIElement CreateContainer(ExportContainer container)
		{
			//ExportContainer container = column as ExportContainer;
			var canvas = new Canvas();
			canvas.Width = container.StyleDecorator.DisplayRectangle.Width;
			canvas.Height = container.StyleDecorator.DisplayRectangle.Height;
			
			
			
			SolidColorBrush backgroundBrush = brushConverter.ConvertFromString(container.StyleDecorator.BackColor.Name) as SolidColorBrush;
			canvas.Background = backgroundBrush;
			
			foreach (var exportElement in container.Items) {
				var uiElement = ItemFactory (exportElement);
				Canvas.SetLeft(uiElement,exportElement.StyleDecorator.Location.X - container.StyleDecorator.Location.X);
				Canvas.SetTop(uiElement,exportElement.StyleDecorator.Location.Y - container.StyleDecorator.Location.Y);
				canvas.Children.Add(uiElement);
			}
			
			canvas.Measure(PageSize);
			canvas.Arrange(new Rect(new System.Windows.Point(), PageSize));
			canvas.UpdateLayout();
			return canvas;
		}

		
		UIElement CreateTextColumn(ExportText et)
		{
			TextBlock tb = CreateTextBlock(et);
			return tb;
		}

		
		UIElement CreateImageColumn(ExportImage exportImage)
		{
			System.Windows.Media.Imaging.BitmapImage bitmap = BitmapFromImage(exportImage);
			Image image = new Image();
			image.Source = bitmap;
			image.Width = exportImage.StyleDecorator.DisplayRectangle.Width;
			image.Height = exportImage.StyleDecorator.DisplayRectangle.Height;
			image.Stretch = System.Windows.Media.Stretch.Fill;
			return image;
		}

		
		System.Windows.Media.Imaging.BitmapImage BitmapFromImage(ExportImage image)
		{
			var bitmap = new System.Windows.Media.Imaging.BitmapImage();
			bitmap.BeginInit();
			MemoryStream memoryStream = new MemoryStream();
			image.Image.Save(memoryStream, ImageFormat.Bmp);
			memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
			bitmap.StreamSource = memoryStream;
			bitmap.EndInit();
			return bitmap;
		}
		
		TextBlock CreateTextBlock(ExportText et)
		{
			TextBlock tb = new TextBlock();
			tb.Text = et.Text;
			SetFont(tb, et.StyleDecorator);
			tb.Width = et.StyleDecorator.DisplayRectangle.Width;
			tb.Height = et.StyleDecorator.DisplayRectangle.Height;
			tb.MaxHeight = et.StyleDecorator.DisplayRectangle.Height;
			tb.MaxWidth = et.StyleDecorator.DisplayRectangle.Width;
			return tb;
		}
		
		
		void SetFont(TextBlock tb, TextStyleDecorator styleDecorator)
		{
			tb.FontFamily = new FontFamily(styleDecorator.Font.FontFamily.Name);
			var b = styleDecorator.Font.Size;
			tb.FontSize = b * 96/72;
			tb.Foreground = brushConverter.ConvertFromString(styleDecorator.ForeColor.Name) as SolidColorBrush;
			if (styleDecorator.Font.Bold) {
				tb.FontWeight = FontWeights.Bold;
			}
			if (styleDecorator.Font.Underline) {
				CreateUnderline(tb,styleDecorator);
			}
			
			if (styleDecorator.Font.Italic) {
				tb.FontStyle = System.Windows.FontStyles.Italic ;
			}
			if (styleDecorator.Font.Strikeout) {
				 CreateStrikeout(tb,styleDecorator);
			}
		}
		
		
		void CreateStrikeout (TextBlock tb, TextStyleDecorator styleDecorator)
		{
			TextDecoration strikeOut = new TextDecoration();
			strikeOut.Location = TextDecorationLocation.Strikethrough;

			Pen p = CreateWpfPen(styleDecorator);
			strikeOut.Pen = p ;
			strikeOut.PenThicknessUnit = TextDecorationUnit.FontRecommended;
			tb.TextDecorations.Add(strikeOut);
		}
		
		void CreateUnderline(TextBlock tb,TextStyleDecorator styleDecorator)
		{
			TextDecoration underLine = new TextDecoration();
			Pen p = CreateWpfPen(styleDecorator);
			underLine.Pen = p ;
			underLine.PenThicknessUnit = TextDecorationUnit.FontRecommended;
			tb.TextDecorations.Add(underLine);
		}


	    Pen CreateWpfPen(TextStyleDecorator styleDecorator)
		{
			Pen myPen = new Pen();
			SolidColorBrush underlineBrush = brushConverter.ConvertFromString(styleDecorator.ForeColor.Name) as SolidColorBrush;
			myPen.Brush = underlineBrush;
			myPen.Thickness = 1.5;
			return myPen;
		}
		
		
		public void ArrangePage(Size pageSize, FixedPage page)
		{
			page.Measure(pageSize);
			page.Arrange(new Rect(new System.Windows.Point(), pageSize));
			page.UpdateLayout();
		}
		
		
		public System.Windows.Size PageSize {get;set;}
	}
}
