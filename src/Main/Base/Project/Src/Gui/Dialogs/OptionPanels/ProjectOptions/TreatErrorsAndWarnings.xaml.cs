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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for ErrorsAndWarnings.xaml
	/// </summary>
	public partial class TreatErrorsAndWarnings : UserControl, ProjectOptionPanel.ILoadSaveCallback
	{
		private ProjectOptionPanel projectOptions;
		
		public TreatErrorsAndWarnings()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		public ProjectOptionPanel.ProjectProperty<bool> TreatWarningsAsErrors {
			get {return projectOptions.GetProperty("TreatWarningsAsErrors", false); }
		}
		
		public ProjectOptionPanel.ProjectProperty<string> WarningsAsErrors {
			get {return projectOptions.GetProperty("WarningsAsErrors","",TextBoxEditMode.EditRawProperty ); }
		}
	
		public void Initialize (ProjectOptionPanel projectOptions)
		{
			if (projectOptions == null) {
				throw new ArgumentNullException("projectOptions");
			}
			this.projectOptions = projectOptions;
			projectOptions.RegisterLoadSaveCallback(this);
		}
		
		public void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			if (this.TreatWarningsAsErrors.Value) {
				this.allRadioButton.IsChecked = true;
			} else {
				if (WarningsAsErrors.Value.Length > 0) {
					this.specificWarningsRadioButton.IsChecked = true;
				} else {
					this.noneRadioButton.IsChecked = true;
				}
			}
		}
		
		public bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			if ((bool)this.noneRadioButton.IsChecked){
				this.specificWarningsTextBox.Text = string.Empty;
			}
			
			if ((bool)this.allRadioButton.IsChecked) {
				this.TreatWarningsAsErrors.Value = true;
			}	else {
				this.TreatWarningsAsErrors.Value = false;
			}
			return true;
		}
		
		void ErrorButton_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			projectOptions.IsDirty = true;
		}
	}	
}
