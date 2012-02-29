/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 28.02.2012
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for LocalizationOptionPanelXAML.xaml
	/// </summary>
	public partial class LocalizationModelOptionsPanel : OptionPanel
	{
		public const string DefaultLocalizationModelPropertyName = "FormsDesigner.DesignerOptions.DefaultLocalizationModel";
		public const string KeepLocalizationModelPropertyName = "FormsDesigner.DesignerOptions.KeepLocalizationModel";
		
		const CodeDomLocalizationModel DefaultLocalizationModelDefaultValue = CodeDomLocalizationModel.PropertyReflection;
		const bool KeepLocalizationModelDefaultValue = false;
		
		public LocalizationModelOptionsPanel()
		{
			InitializeComponent();
			this.reflectionRadioButton.IsChecked = (DefaultLocalizationModel == CodeDomLocalizationModel.PropertyReflection);
			this.assignmentRadioButton.IsChecked = !this.reflectionRadioButton.IsChecked;
			this.keepModelCheckBox.IsChecked = KeepLocalizationModel;
		}
		
		
		
		public static CodeDomLocalizationModel DefaultLocalizationModel {
			get { return GetPropertySafe(DefaultLocalizationModelPropertyName, DefaultLocalizationModelDefaultValue); }
			set { PropertyService.Set(DefaultLocalizationModelPropertyName, value); }
		}
		
		
		public static bool KeepLocalizationModel {
			get { return GetPropertySafe(KeepLocalizationModelPropertyName, KeepLocalizationModelDefaultValue); }
			set { PropertyService.Set(KeepLocalizationModelPropertyName, value); }
		}
		
		
		static T GetPropertySafe<T>(string name, T defaultValue)
		{
			if (PropertyService.Initialized) {
				return PropertyService.Get<T>(name, defaultValue);
			} else {
				return defaultValue;
			}
		}
		
		public override bool SaveOptions()
		{
			if (this.reflectionRadioButton.IsChecked == true) {
				DefaultLocalizationModel = CodeDomLocalizationModel.PropertyReflection;
			} else if (this.assignmentRadioButton.IsChecked == true) {
				DefaultLocalizationModel = CodeDomLocalizationModel.PropertyAssignment;
			} else {
				MessageService.ShowError("One localization model must be selected!");
				return false;
			}
			
			KeepLocalizationModel = (this.keepModelCheckBox.IsChecked == true);
			
			return true;
			
		}
	}
}