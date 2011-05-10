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
			foreach (var element in exporterPage.Items) {
				var item = ItemFactory(element);
				page.Children.Add(item);
			}
			
			Console.WriteLine("\tPage created with with {0} items ",page.Children.Count);
			return page;
		}
		
		
		UIElement ItemFactory (BaseExportColumn column)
		{
			var text = column as ExportText;
			UIElement element = null;
			if (text != null) {
				element = CreateTextColumn(text);
			}
			return element;
		}
		
		
		TextBlock CreateTextColumn(ExportText et)
		{
			TextBlock tb = new TextBlock();
					tb.Text = et.Text;
				
					SetFont(tb,et.StyleDecorator);
					FixedPage.SetLeft(tb,et.StyleDecorator.Location.X );
					FixedPage.SetTop(tb,et.StyleDecorator.Location.Y);
			return tb;
		}
		
		
		void SetFont(TextBlock tb, TextStyleDecorator styleDecorator)
		{
//			
//			http://msdn.microsoft.com/en-us/library/system.windows.textdecorations.aspx
//			http://stackoverflow.com/questions/637636/using-a-system-drawing-font-with-a-wpf-label
			
			var s = NewTypeFaceFromFont (styleDecorator.Font);
	
//			 FontFamilyConverter conv = new FontFamilyConverter(); 
//			   FontFamily mfont1 = conv.ConvertFromString(styleDecorator.Font.Name) as FontFamily;
//			   tb.FontFamily = mfont1;
			tb.FontFamily = new FontFamily(styleDecorator.Font.FontFamily.Name);
			tb.FontSize = styleDecorator.Font.GetHeight();
			if (styleDecorator.Font.Bold) {
				tb.FontWeight = FontWeights.Bold;
			}
			Console.WriteLine ("{0} - {1}",styleDecorator.Font.FontFamily.Name,styleDecorator.Font.GetHeight());
		}
		
		
		private static Typeface NewTypeFaceFromFont(System.Drawing.Font f)
		{     Typeface typeface = null;
			FontFamily ff = new FontFamily(f.Name);
			if (typeface == null)
			{
				typeface = new Typeface(ff, (f.Style == System.Drawing.FontStyle.Italic ? FontStyles.Italic : FontStyles.Normal),
				                        (f.Style == System.Drawing.FontStyle.Bold ? FontWeights.Bold : FontWeights.Normal),
				                        FontStretches.Normal);
			}
			if (typeface == null)
			{
				typeface = new Typeface(new FontFamily("Arial"),
				                        FontStyles.Italic,
				                        FontWeights.Normal,
				                        FontStretches.Normal);
			}
			return typeface;
		}


		public void ArrangePage(Size pageSize, FixedPage page)
		{
			Console.WriteLine("Arrange page with Size {0}",pageSize);
			page.Measure(pageSize);
			page.Arrange(new Rect(new System.Windows.Point(), pageSize));
			page.UpdateLayout();
		}
			
	}
}
