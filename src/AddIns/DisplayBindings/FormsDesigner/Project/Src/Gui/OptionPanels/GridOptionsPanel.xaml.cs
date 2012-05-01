/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 29.02.2012
 * Time: 20:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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