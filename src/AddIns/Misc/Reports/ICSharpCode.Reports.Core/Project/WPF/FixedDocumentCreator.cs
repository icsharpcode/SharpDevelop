/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 10.05.2011
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
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
			Console.WriteLine("\tCreatepage with {0} items ",exporterPage.Items.Count);
			FixedPage page = new FixedPage();
			//http://www.bing.com/search?q=Convert+Windows+Forms+to+WPF&FORM=QSRE
			
			CreatePageInternal (page,exporterPage.Items);
			
			
			Console.WriteLine("\tPage created with with {0} items ",page.Children.Count);
			return page;
		}
		
		
		void CreatePageInternal(FixedPage page, ExporterCollection items)
		{
			foreach (var element in items)
			{
				var cont = element as ExportContainer;
				var item = ItemFactory(element);
				if (item != null) {
					page.Children.Add(item);
				}
			}
		}
		
		
		UIElement ItemFactory (BaseExportColumn column)
		{
			Console.WriteLine(" Create {0}",column.ToString());
			UIElement element = null;
			
			var container = column as ExportContainer;
			
			if (container != null) {
				element = CreateContainer(column);
			}
			
			var text = column as ExportText;
			
			if (text != null) {
				element = CreateTextColumn(text);
			}
			return element;
		}
		

		private UIElement CreateContainer(BaseExportColumn column)
		{
			ExportContainer container = column as ExportContainer;
			var canvas = new Canvas();
			canvas.Width = column.StyleDecorator.DisplayRectangle.Width;
			canvas.Height = column.StyleDecorator.DisplayRectangle.Height;
			
			FixedPage.SetLeft(canvas,column.StyleDecorator.Location.X);
			FixedPage.SetTop(canvas,column.StyleDecorator.Location.Y);
			
			var conv = new BrushConverter();
//			SolidColorBrush b = conv.ConvertFromString(System.Drawing.Color.LightGray.Name) as SolidColorBrush;
			SolidColorBrush b = conv.ConvertFromString(column.StyleDecorator.BackColor.Name) as SolidColorBrush;
			canvas.Background = b;
			foreach (var element in container.Items) {
				var text = element as ExportText;
				if (text != null)
				{
					var textBlock = CreateTextBlock (text);
					
//					textBlock.ClipToBounds = true;
					textBlock.TextWrapping = TextWrapping.Wrap;
					
					Canvas.SetLeft(textBlock,element.StyleDecorator.Location.X - column.StyleDecorator.Location.X);
					Canvas.SetTop(textBlock,element.StyleDecorator.Location.Y - column.StyleDecorator.Location.Y);
					
//					var m  = new System.Windows.Thickness(text.StyleDecorator.Location.X - column.StyleDecorator.Location.X,
//					                                      text.StyleDecorator.Location.Y - column.StyleDecorator.Location.Y,0,0);
//					t.Margin = m;
					Console.WriteLine("{0} - {1} - {2} ",textBlock.Text,textBlock.Width,textBlock.MaxWidth);
					                  
					                  
					                  
					canvas.Children.Add(textBlock);
				}
			}
			canvas.Measure(PageSize);
			canvas.Arrange(new Rect(new System.Windows.Point(), PageSize));
			canvas.UpdateLayout();
			return canvas;
		}

		
		UIElement CreateTextColumn(ExportText et)
		{
			TextBlock tb = CreateTextBlock(et);
			
			FixedPage.SetLeft(tb,et.StyleDecorator.Location.X );
			FixedPage.SetTop(tb,et.StyleDecorator.Location.Y);
			return tb;
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
		
		//points = pixels * 96 / 72 
		
		void SetFont(TextBlock tb, TextStyleDecorator styleDecorator)
		{
//
//			http://msdn.microsoft.com/en-us/library/system.windows.textdecorations.aspx
//			http://stackoverflow.com/questions/637636/using-a-system-drawing-font-with-a-wpf-label
			
//			var s = NewTypeFaceFromFont (styleDecorator.Font);
			
//			 FontFamilyConverter conv = new FontFamilyConverter();
//			   FontFamily mfont1 = conv.ConvertFromString(styleDecorator.Font.Name) as FontFamily;
//			   tb.FontFamily = mfont1;
			tb.FontFamily = new FontFamily(styleDecorator.Font.FontFamily.Name);
//			var s = styleDecorator.Font.GetHeight();
//			var a = s /97*72;
			var b = styleDecorator.Font.Size;
			//tb.FontSize = (int)styleDecorator.Font.GetHeight();
			tb.FontSize = b * 96/72;
			if (styleDecorator.Font.Bold) {
				tb.FontWeight = FontWeights.Bold;
			}
			
			Console.WriteLine ("{0} - {1}",styleDecorator.Font.FontFamily.Name,styleDecorator.Font.GetHeight());
		}
	
/*
	using System.Drawing;
using Media = System.Windows.Media;

 Font font = new Font(new System.Drawing.FontFamily("Comic Sans MS"), 10);
            //option 1
            Media.FontFamily mfont = new Media.FontFamily(font.Name);
            //option 2 does the same thing
            Media.FontFamilyConverter conv = new Media.FontFamilyConverter();
            Media.FontFamily mfont1 = conv.ConvertFromString(font.Name) as Media.FontFamily;
            //option 3
            Media.FontFamily mfont2 = Media.Fonts.SystemFontFamilies.Where(x => x.Source == font.Name).FirstOrDefault();
	
		
		
*/		
		public void ArrangePage(Size pageSize, FixedPage page)
		{
			Console.WriteLine("Arrange page with Size {0}",pageSize);
			page.Measure(pageSize);
			page.Arrange(new Rect(new System.Windows.Point(), pageSize));
			page.UpdateLayout();
		}
		
		
		public System.Windows.Size PageSize {get;set;}
	}
}
