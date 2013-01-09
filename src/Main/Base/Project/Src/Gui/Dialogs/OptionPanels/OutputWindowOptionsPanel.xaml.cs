/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 27.12.2012
 * Time: 19:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Media;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels 
{
	/// <summary>
	/// Interaction logic for OutputWindowOptionsPanelXaml.xaml
	/// </summary>
	public partial class OutputWindowOptionsPanel :  OptionPanel
	{
		public static readonly string OutputWindowsProperty = "SharpDevelop.UI.OutputWindowOptions";
	
		public static readonly string FontFamilyName = "FontFamily";
		public static readonly string FontSizeName = "FontSize";
		public static readonly string WordWrapName = "WordWrap";
		
		
		public OutputWindowOptionsPanel()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			var properties = PropertyService.NestedProperties(OutputWindowsProperty);
			WordWrap = properties.Get(WordWrapName, true);
			
			var fontDescription =  OutputWindowOptionsPanel.DefaultFontDescription();
			fontSelectionPanel.SelectedFontFamily = new FontFamily(fontDescription.Item1);
			fontSelectionPanel.SelectedFontSize = fontDescription.Item2;
		}
		
		
		public override bool SaveOptions()
		{
			var properties = PropertyService.NestedProperties(OutputWindowsProperty);
			properties.Set(WordWrapName, WordWrap);
			properties.Set(FontFamilyName,fontSelectionPanel.SelectedFontFamily);
			properties.Set(FontSizeName,fontSelectionPanel.SelectedFontSize);
			return base.SaveOptions();
		}
		
		
		public static Tuple<string,int> DefaultFontDescription()
		{
			var properties = PropertyService.NestedProperties(OutputWindowsProperty);
			return new Tuple<string,int>(properties.Get(FontFamilyName,"Consolas"),properties.Get(FontSizeName,13));
		}
		
		
		bool wordWrap;
		
		public bool WordWrap {
			get { return wordWrap; }
			set { wordWrap = value; 
				base.RaisePropertyChanged(() => WordWrap);}
		}
		
	}
}