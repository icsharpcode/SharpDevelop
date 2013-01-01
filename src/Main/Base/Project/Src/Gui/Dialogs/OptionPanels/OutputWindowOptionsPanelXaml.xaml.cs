/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 27.12.2012
 * Time: 19:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels 
{
	/// <summary>
	/// Interaction logic for OutputWindowOptionsPanelXaml.xaml
	/// </summary>
	public partial class OutputWindowOptionsPanelXaml :  OptionPanel
	{
		private static readonly string OutputWindowsProperty = "SharpDevelop.UI.OutputWindowOptions";
		
		public OutputWindowOptionsPanelXaml()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			var properties = PropertyService.NestedProperties(OutputWindowsProperty);
			WordWrap = properties.Get("WordWrap", true);
			var fontStr = properties.Get("DefaultFont", SD.WinForms.DefaultMonospacedFont.ToString()).ToString();
			var font = ParseFont(fontStr);
			fontSelectionPanel.SelectedFontFamily = new System.Windows.Media.FontFamily(font.Name);
			fontSelectionPanel.SelectedFontSize = (int)font.Size;
			font.Dispose();
		}
		
		
		public static Font ParseFont(string font)
		{
			try {
				string[] descr = font.Split(new char[]{',', '='});
				return new Font(descr[1], Single.Parse(descr[3]));
			} catch (Exception ex) {
				LoggingService.Warn(ex);
				return SD.WinForms.DefaultMonospacedFont;
			}
		}
		
		
		public override bool SaveOptions()
		{
			var properties = PropertyService.NestedProperties(OutputWindowsProperty);
			properties.Set("WordWrap", WordWrap);
		
			var font = new Font(new System.Drawing.FontFamily(fontSelectionPanel.SelectedFontName),
			                     (float)fontSelectionPanel.SelectedFontSize);
			Console.WriteLine(font.ToString());
			font.Dispose();
			return base.SaveOptions();
		}
		
		
		bool wordWrap;
		
		public bool WordWrap {
			get { return wordWrap; }
			set { wordWrap = value; 
				base.RaisePropertyChanged(() => WordWrap);}
		}
		
	}
}