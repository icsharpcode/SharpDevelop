/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 10.05.2011
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using Image = System.Windows.Controls.Image;
using Pen = System.Windows.Media.Pen;
using Size = System.Windows.Size;

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
			
			var graphicContainer = column as ExportGraphicContainer;
			if ( graphicContainer != null) {
				element = CreateGraphicsContainer(graphicContainer);
				return element;
			}
			
			var container = column as ExportContainer;
			if (container != null) {
				element = CreateContainer(container);
			}
			
			var exportGraphic = column as ExportGraphic;
			if (exportGraphic != null) {
				element = CreateGraphicsElement(exportGraphic);
			}
			
			var text = column as ExportText;
			if (text != null) {
				var t = CreateTextBlock(text);
				
				if (column.StyleDecorator.DrawBorder) {
					border = CreateBorder(column.StyleDecorator as BaseStyleDecorator,
					                      GlobalValues.DefaultBorderThickness,
					                      GlobalValues.DefaultCornerRadius	);			                      					                      
					                     
					border.Child = t;
					element = border;
				}
				else
				{
					element = t;
				}
			}
			

			var image = column as ExportImage;
			
			if (image != null)
			{
				element = CreateImageColumn(image);
			}
			
			return element;
		}
		
		
		#region GraphicsElement (Line etc)
		
		System.Windows.Controls.Border CreateBorder( BaseStyleDecorator column,double thickness,double cornerRadius)
		{
			var border = new System.Windows.Controls.Border();
			border.Padding = new Thickness(1);
			border.BorderThickness = new Thickness(thickness);
			border.CornerRadius = new CornerRadius(cornerRadius);
			border.BorderBrush = ConvertBrush(column.ForeColor);
			return border;
		}
		
		
		UIElement CreateGraphicsElement(ExportGraphic column)
		{
			var  line = new System.Windows.Shapes.Line();
			line.Stroke = ConvertBrush(column.StyleDecorator.ForeColor);
			line.StrokeThickness = column.StyleDecorator.Thickness;
			
			var ld = column.StyleDecorator as LineDecorator;
			
			if (ld != null) {
				line.X1 = ld.From.X;
				line.Y1 = ld.From.Y;
				line.X2 = ld.To.X;
				line.Y2 = ld.To.Y;
			} else {
				line.X1 = column.StyleDecorator.Location.X;
				line.Y1 = column.StyleDecorator.Location.Y;
				line.X2 = column.StyleDecorator.DisplayRectangle.Width;
				line.Y2 = column.StyleDecorator.Location.Y;
			}
			return line;
		}
		
		#endregion
		
		#region Container
		UIElement CreateGraphicsContainer(ExportGraphicContainer container)
		{
			IGraphicStyleDecorator decorator = container.StyleDecorator as IGraphicStyleDecorator;
			
			UIElement shape = null;
			var ss = decorator.Shape as EllipseShape;
			if (ss != null) {
				
				var circle  = new System.Windows.Shapes.Ellipse();
				SetDimension(circle,decorator);
				circle.Fill = ConvertBrush(decorator.BackColor);
				circle.StrokeThickness = decorator.Thickness;
				circle.Stroke = ConvertBrush(decorator.ForeColor);
				shape = circle;
			}
			else
			{
				RectangleShape rs = decorator.Shape as RectangleShape;
				var border = CreateBorder(decorator as BaseStyleDecorator,decorator.Thickness,rs.CornerRadius);
					
				border.BorderBrush = ConvertBrush(decorator.ForeColor);
				border.Background  = ConvertBrush(decorator.BackColor);
					
				shape = border;
				
				var canvas = CreateCanvas(container);
				canvas.Width = decorator.DisplayRectangle.Width -1;
				canvas.Height = decorator.DisplayRectangle.Height -1 ;
				canvas.Background = border.Background;
				AddItemsToCanvas(ref canvas, container);
				border.Child = canvas;
			}
			
			return shape;
		}
		
	
		
		private UIElement CreateContainer(ExportContainer container)
		{
			var canvas = CreateCanvas(container);
			AddItemsToCanvas(ref canvas, container);
			
			canvas.Measure(PageSize);
			canvas.Arrange(new Rect(new System.Windows.Point(), PageSize));
			canvas.UpdateLayout();
			return canvas;
		}

		
		void AddItemsToCanvas(ref Canvas canvas, ExportContainer container)
		{
			foreach (var exportElement in container.Items) {
				var uiElement = ItemFactory(exportElement);
				Canvas.SetLeft(uiElement, exportElement.StyleDecorator.Location.X - container.StyleDecorator.Location.X);
				Canvas.SetTop(uiElement, exportElement.StyleDecorator.Location.Y - container.StyleDecorator.Location.Y);
				canvas.Children.Add(uiElement);
			}
		}

		Canvas CreateCanvas(ExportContainer container)
		{
			var canvas = new Canvas();
			SetDimension(canvas, container.StyleDecorator);
			canvas.Background = ConvertBrush(container.StyleDecorator.BackColor);
			return canvas;
		}
		
		#endregion
		
		
		UIElement CreateTextColumn(ExportText et)
		{
			TextBlock tb = CreateTextBlock(et);
			return tb;
		}

		
		#region Image
		
		UIElement CreateImageColumn(ExportImage exportImage)
		{
			System.Windows.Media.Imaging.BitmapImage bitmap = BitmapFromImage(exportImage);
			Image image = new Image();
			image.Source = bitmap;
			SetDimension(image,exportImage.StyleDecorator);
			image.Stretch = System.Windows.Media.Stretch.Fill;
			return image;
		}

		
		static System.Windows.Media.Imaging.BitmapImage BitmapFromImage(ExportImage image)
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
		
		#endregion
		
		#region TextBlock
		
		TextBlock CreateTextBlock(ExportText exportText)
		{
			TextBlock textBlock = new TextBlock();

			SetFont(textBlock, exportText.StyleDecorator);
			textBlock.TextWrapping = TextWrapping.Wrap;
			
			string [] inlines = exportText.Text.Split(System.Environment.NewLine.ToCharArray());

			for (int i = 0; i < inlines.Length; i++) {
				if (inlines[i].Length > 0) {
					textBlock.Inlines.Add(new Run(inlines[i]));
					textBlock.Inlines.Add(new LineBreak());
				}
			}
			var li = textBlock.Inlines.LastInline;
			textBlock.Inlines.Remove(li);
			SetDimension(textBlock,exportText.StyleDecorator);
		    textBlock.Background = ConvertBrush(exportText.StyleDecorator.BackColor);
		    SetContendAlignment(textBlock,exportText.StyleDecorator);
			return textBlock;
		}


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
		
		
		void SetFont(TextBlock textBlock, TextStyleDecorator styleDecorator)
		{
			textBlock.FontFamily = new FontFamily(styleDecorator.Font.FontFamily.Name);
			var b = styleDecorator.Font.Size;
			textBlock.FontSize = b * 96/72;
			if (styleDecorator.Font.Bold) {
				textBlock.FontWeight = FontWeights.Bold;
			}
			if (styleDecorator.Font.Underline) {
				CreateUnderline(textBlock,styleDecorator);
			}
			
			if (styleDecorator.Font.Italic) {
				textBlock.FontStyle = System.Windows.FontStyles.Italic ;
			}
			if (styleDecorator.Font.Strikeout) {
				CreateStrikeout(textBlock,styleDecorator);
			}
		}
		
		
		Brush ConvertBrush(System.Drawing.Color color)
		{
			if (brushConverter.IsValid(color.Name))
			{
				return brushConverter.ConvertFromString(color.Name) as SolidColorBrush;
			} else
			{
				return brushConverter.ConvertFromString("Black") as SolidColorBrush;
			}
		}
		
		
		void CreateStrikeout (TextBlock textBlock, TextStyleDecorator styleDecorator)
		{
			TextDecoration strikeOut = new TextDecoration();
			strikeOut.Location = TextDecorationLocation.Strikethrough;

			Pen p = CreateWpfPen(styleDecorator);
			strikeOut.Pen = p ;
			strikeOut.PenThicknessUnit = TextDecorationUnit.FontRecommended;
			textBlock.TextDecorations.Add(strikeOut);
		}
		
		
		void CreateUnderline(TextBlock textBlock,TextStyleDecorator styleDecorator)
		{
			TextDecoration underLine = new TextDecoration();
			Pen p = CreateWpfPen(styleDecorator);
			underLine.Pen = p ;
			underLine.PenThicknessUnit = TextDecorationUnit.FontRecommended;
			textBlock.TextDecorations.Add(underLine);
		}

		#endregion

		Pen CreateWpfPen(TextStyleDecorator styleDecorator)
		{
			Pen myPen = new Pen();
			myPen.Brush = ConvertBrush(styleDecorator.ForeColor);
			myPen.Thickness = 1.5;
			return myPen;
		}
		
		
		static void SetDimension (FrameworkElement element,IBaseStyleDecorator decorator)
		{
			element.Width = decorator.DisplayRectangle.Width;
			element.Height = decorator.DisplayRectangle.Height;
		}
		
		
		public static void ArrangePage(Size pageSize, FixedPage page)
		{
			page.Measure(pageSize);
			page.Arrange(new Rect(new System.Windows.Point(), pageSize));
			page.UpdateLayout();
		}
		
		
		public System.Windows.Size PageSize {get;set;}
	}
}
