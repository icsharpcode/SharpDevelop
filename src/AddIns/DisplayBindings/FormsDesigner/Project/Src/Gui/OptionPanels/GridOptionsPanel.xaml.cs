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
using System.Windows;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for GridOptrionsXAML.xaml
	/// </summary>
	public partial class GridOptionsPanel : OptionPanel
	{
		public GridOptionsPanel()
		{
			InitializeComponent();
			bool snapToGridOn = PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGridMode", false);
			
			this.snapToGridRadioButton.IsChecked = snapToGridOn;
			this.snapLinesRadioButton.IsChecked = !snapToGridOn;
			
			this.widthTextBox.Text = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeWidth", 8).ToString();
			this.heightTextBox.Text = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeHeight", 8).ToString();
			
			
			this.showGridCheckBox.IsChecked =  PropertyService.Get("FormsDesigner.DesignerOptions.ShowGrid", true);
			this.snapToGridCheckBox.IsChecked  = PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGrid", true);
			EnableGridOptions(snapToGridOn);
		}
		
		
		public override bool SaveOptions()
		{
			
			PropertyService.Set("FormsDesigner.DesignerOptions.SnapToGridMode", this.snapToGridCheckBox.IsChecked);
			PropertyService.Set("FormsDesigner.DesignerOptions.UseSnapLines", this.snapLinesRadioButton.IsChecked);
			PropertyService.Set("FormsDesigner.DesignerOptions.ShowGrid", this.showGridCheckBox.IsChecked);
			PropertyService.Set("FormsDesigner.DesignerOptions.SnapToGrid", this.snapToGridRadioButton.IsChecked);
			
			int width;
			
			bool result = Int32.TryParse(this.widthTextBox.Text, out width);
			
			if (result) {
				PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeWidth", width);
			} else {
				MessageService.ShowError("Forms Designer grid width is invalid");
				return false;
			}
			
			int height = 0;
			result = Int32.TryParse(this.heightTextBox.Text, out height);
			if (result) {
				PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeHeight", height);
			}else {
				MessageService.ShowError("Forms Designer grid height is invalid");
				return false;
			}
			return true;
		}
		
		
		void EnableGridOptions (bool enable)
		{
			this.widthTextBox.IsEnabled = enable;
			this.heightTextBox.IsEnabled = enable;
			this.showGridCheckBox.IsEnabled = enable;
			this.snapToGridCheckBox.IsEnabled = enable;
		}
		
		
		void SnapToGridRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			EnableGridOptions(true);
		}
		
		void SnapLinesRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			EnableGridOptions(false);
		}
	}
}
