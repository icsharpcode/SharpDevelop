/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 20.09.2012
 * Time: 19:38
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
	
	
	public partial class TreatErrorsAndWarnings : UserControl,IProjectUserControl
	{
		private ProjectOptionPanel projectOptions;
		
		public TreatErrorsAndWarnings()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		#region properties
		
		
		public ProjectOptionPanel.ProjectProperty<bool> TreatWarningsAsErrors {
			get {return projectOptions.GetProperty("TreatWarningsAsErrors", false); }
		}
		
		
		public ProjectOptionPanel.ProjectProperty<string> WarningsAsErrors {
			get {return projectOptions.GetProperty("WarningsAsErrors","",TextBoxEditMode.EditRawProperty ); }
		}
		
		
		#endregion
		
		#region IProjectuserControl
	
		public void SetProjectOptions (ProjectOptionPanel projectOptions)
		{
			if (projectOptions == null) {
				throw new ArgumentNullException("projectOptions");
			}
			this.projectOptions = projectOptions;
			SetTreatWarningAsErrorRadioButtons();
		}
		
		public bool SaveProjectOptions()
		{
			if ((bool)this.noneRadioButton.IsChecked){
				this.specificWarningsTextBox.Text = string.Empty;
			}
			
			if ((bool)this.allRadioButton.IsChecked) {
				this.TreatWarningsAsErrors.Value = true;
			}	else {
				this.TreatWarningsAsErrors.Value = false;
			}
			this.noneRadioButton.Checked -= ErrorButton_Checked;
			this.allRadioButton.Checked -= ErrorButton_Checked;
			this.specificWarningsRadioButton.Checked -= ErrorButton_Checked;
			return true;
		}
		
		#endregion
		
		private void SetTreatWarningAsErrorRadioButtons()
		{
			if (this.TreatWarningsAsErrors.Value) {
				this.allRadioButton.IsChecked  = true;
			} else {
				if (WarningsAsErrors.Value.Length > 0) {
					this.specificWarningsRadioButton.IsChecked = true;
				} else {
					this.noneRadioButton.IsChecked = true;
				}
			}
			this.noneRadioButton.Checked += ErrorButton_Checked;
			this.allRadioButton.Checked += ErrorButton_Checked;
			this.specificWarningsRadioButton.Checked += ErrorButton_Checked;
		}
		

		
		void ErrorButton_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			projectOptions.IsDirty = true;
		}
	}	
}