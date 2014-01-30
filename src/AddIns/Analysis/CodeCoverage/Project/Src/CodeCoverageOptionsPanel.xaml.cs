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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Interaction logic for CodeCoverageOptionsPanelXaml.xaml
	/// </summary>
	public partial class CodeCoverageOptionsPanel : OptionPanel
	{
		
		public CodeCoverageOptionsPanel()
		{
			InitializeComponent();
			DataContext = this;
			DisplayItems = new ObservableCollection<CodeCoverageDisplayItem>();
			DisplayItems.Add(new CodeCoverageDisplayItem(StringParser.Parse("${res:ICSharpCode.CodeCoverage.CodeCovered}"), CodeCoverageOptions.VisitedColorProperty, CodeCoverageOptions.VisitedColor, CodeCoverageOptions.VisitedForeColorProperty, CodeCoverageOptions.VisitedForeColor));
			DisplayItems.Add(new CodeCoverageDisplayItem(StringParser.Parse("${res:ICSharpCode.CodeCoverage.CodePartCovered}"), CodeCoverageOptions.PartVisitedColorProperty, CodeCoverageOptions.PartVisitedColor, CodeCoverageOptions.PartVisitedForeColorProperty, CodeCoverageOptions.PartVisitedForeColor));
			DisplayItems.Add(new CodeCoverageDisplayItem(StringParser.Parse("${res:ICSharpCode.CodeCoverage.CodeNotCovered}"), CodeCoverageOptions.NotVisitedColorProperty, CodeCoverageOptions.NotVisitedColor, CodeCoverageOptions.NotVisitedForeColorProperty, CodeCoverageOptions.NotVisitedForeColor));
			DisplayItem = DisplayItems[0];
		}
		
		CodeCoverageDisplayItem displayItem;
		
		public CodeCoverageDisplayItem DisplayItem {
			get { return displayItem; }
			set { displayItem = value;
				base.RaisePropertyChanged(() => DisplayItem);
				ShowColors();
			}
		}
		
		
		private System.Windows.Media.Color foreGroundColor;
		
		public Color ForeGroundColor {
			get { return foreGroundColor; }
			set {
				foreGroundColor = value;
				RaisePropertyChanged(() => ForeGroundColor);
				DisplayItem.ForeColor = SetColor(ForeGroundColor);
			}
		}
		
		
		private System.Windows.Media.Color backGroundColor;
		
		public Color BackGroundColor {
			get { return backGroundColor; }
			set { backGroundColor = value;
				RaisePropertyChanged(() => BackGroundColor);
				DisplayItem.BackColor = SetColor(BackGroundColor);
			}
		}
		
		
		public override bool SaveOptions()
		{
			bool codeCoverageColorsChanged = false;
			
			foreach (CodeCoverageDisplayItem item in displayItemsListBox.Items) {
				if (item.HasChanged) {
					CodeCoverageOptions.Properties.Set<System.Drawing.Color>(item.ForeColorPropertyName, item.ForeColor);
					CodeCoverageOptions.Properties.Set<System.Drawing.Color>(item.BackColorPropertyName, item.BackColor);
					codeCoverageColorsChanged = true;
				}
			}

			if (codeCoverageColorsChanged) {
				CodeCoverageService.RefreshCodeCoverageHighlights();
			}
			return base.SaveOptions();
		}
		
		private void ShowColors()
		{
			System.Drawing.Color clr = DisplayItem.ForeColor;
			ForeGroundColor = System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
			clr = DisplayItem.BackColor;
			BackGroundColor = System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
		}
		
		
		private System.Drawing.Color SetColor (System.Windows.Media.Color color)
		{
			return System.Drawing.Color.FromArgb(color.A,color.R,color.G,color.B);
		}
			
		public ObservableCollection<CodeCoverageDisplayItem> DisplayItems {get; private set;}
	}
}
