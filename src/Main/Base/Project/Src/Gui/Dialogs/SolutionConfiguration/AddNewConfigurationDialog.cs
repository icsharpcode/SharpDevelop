// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Dialog for adding a new configuration or platform to a solution or project.
	/// </summary>
	public partial class AddNewConfigurationDialog
	{
		Predicate<string> checkNameValid;
		
		public AddNewConfigurationDialog(bool solution, bool editPlatforms,
		                                 IEnumerable<string> availableSourceItems,
		                                 Predicate<string> checkNameValid)
		{
			this.checkNameValid = checkNameValid;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			foreach (Control ctl in this.Controls) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
			
			createInAllCheckBox.Visible = solution;
			nameTextBox.TextChanged += delegate {
				okButton.Enabled = nameTextBox.TextLength > 0;
			};
			copyFromComboBox.Items.Add(StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.EmptyItem}"));
			copyFromComboBox.Items.AddRange(availableSourceItems.ToArray());
			copyFromComboBox.SelectedIndex = 0;
			
			if (solution) {
				if (editPlatforms)
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.AddSolutionPlatform}");
				else
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.AddSolutionConfiguration}");
			} else {
				if (editPlatforms)
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.AddProjectPlatform}");
				else
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.AddProjectConfiguration}");
			}
		}
		
		public bool CreateInAllProjects {
			get {
				return createInAllCheckBox.Checked;
			}
		}
		
		public string CopyFrom {
			get {
				if (copyFromComboBox.SelectedIndex <= 0)
					return null;
				else
					return copyFromComboBox.SelectedItem.ToString();
			}
		}
		
		public string NewName {
			get {
				return nameTextBox.Text;
			}
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			if (checkNameValid(nameTextBox.Text)) {
				this.DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
