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
		const bool KeepLocalizationModelDefaultValue = true;
		
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
			// This wrapper is no longer necessary in SD5;
			// if the actual property service isn't available (in unit tests), a dummy property service is used
			return PropertyService.Get<T>(name, defaultValue);
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
