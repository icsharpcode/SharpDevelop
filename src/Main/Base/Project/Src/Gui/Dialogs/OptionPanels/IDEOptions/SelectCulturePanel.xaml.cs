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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class SelectCulturePanel : OptionPanel
	{
		public SelectCulturePanel()
		{
			InitializeComponent();
			listView.ItemsSource = UILanguageService.Languages;
		}
		
		static readonly string langPropName = "CoreProperties.UILanguage";
		
		public static UILanguage CurrentLanguage {
			get { return GetCulture(PropertyService.Get(langPropName, "en")); }
			set { PropertyService.Set(langPropName, value.Code); }
		}
		
		static UILanguage GetCulture(string languageCode)
		{
			return UILanguageService.Languages.FirstOrDefault(x => x.Code.StartsWith(languageCode))
				?? UILanguageService.Languages.First(x => x.Code.StartsWith("en"));
		}
	}
	
	public class ImageView : ViewBase
	{
		protected override object DefaultStyleKey
		{
			get { return new ComponentResourceKey(GetType(), "ImageView"); }
		}
		
		protected override object ItemContainerDefaultStyleKey
		{
			get { return new ComponentResourceKey(GetType(), "ImageViewItem"); }
		}
	}
}
