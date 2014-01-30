// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
