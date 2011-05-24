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
		public FixedDocumentCreator()
		{
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
			
			var container = column as ExportContainer;
			
			if (container != null) {
				element = CreateContainer(container);
			}
			
			var text = column as ExportText;
			
			if (text != null) {
				element = CreateTextColumn(text);
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
			
			var conv = new BrushConverter();
			
			SolidColorBrush backgroundBrush = conv.ConvertFromString(container.StyleDecorator.BackColor.Name) as SolidColorBrush;
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
			if (styleDecorator.Font.Bold) {
				tb.FontWeight = FontWeights.Bold;
			}
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
