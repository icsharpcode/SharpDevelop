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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Project
{
	internal partial class EditAvailableConfigurationsDialog
	{
		readonly IConfigurable configurable;
		readonly bool editPlatforms;
		readonly IConfigurationOrPlatformNameCollection editedCollection;
		
		public EditAvailableConfigurationsDialog(IConfigurable configurable, bool editPlatforms)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			foreach (Control ctl in this.Controls) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
			
			this.configurable = configurable;
			this.editPlatforms = editPlatforms;
			if (editPlatforms) {
				if (configurable is ISolution)
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.EditSolutionPlatforms}");
				else
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.EditProjectPlatforms}");
				this.editedCollection = configurable.PlatformNames;
			} else {
				if (configurable is ISolution)
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.EditSolutionConfigurations}");
				else
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.EditProjectConfigurations}");
				this.editedCollection = configurable.ConfigurationNames;
			}
			InitList();
		}
		
		void InitList()
		{
			if (editPlatforms) {
				ShowEntries(configurable.PlatformNames, configurable.ActiveConfiguration.Platform);
			} else {
				ShowEntries(configurable.ConfigurationNames, configurable.ActiveConfiguration.Configuration);
			}
		}
		
		void ShowEntries(IEnumerable<string> list, string activeItem)
		{
			string[] array = list.ToArray();
			listBox.Items.Clear();
			listBox.Items.AddRange(array);
			if (listBox.Items.Count == 0) {
				throw new Exception("There must be at least one configuration/platform");
			}
			listBox.SelectedIndex = Math.Max(Array.IndexOf(array, activeItem), 0);
		}
		
		void RemoveButtonClick(object sender, EventArgs e)
		{
			if (listBox.Items.Count == 1) {
				MessageService.ShowMessage("${res:Dialog.EditAvailableConfigurationsDialog.CannotDeleteAllConfigurationsOrPlatforms}");
				return;
			}
			string name = listBox.SelectedItem.ToString();
			if (MessageService.AskQuestion(StringParser.Format(
				"${res:Dialog.EditAvailableConfigurationsDialog.ConfirmRemoveConfigurationOrPlatform}", name)))
			{
				editedCollection.Remove(name);
				InitList();
			}
		}
		
		void RenameButtonClick(object sender, EventArgs e)
		{
			string oldName = listBox.SelectedItem.ToString();
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}",
			                                             "${res:Dialog.EditAvailableConfigurationsDialog.EnterNewName}", oldName);
			if (string.IsNullOrEmpty(newName) || newName == oldName)
				return;
			if (!EnsureCorrectName(ref newName))
				return;
			editedCollection.Rename(oldName, newName);
			ISolution solution = configurable as ISolution;
			if (solution != null) {
				// Solution platform name => project platform name
				foreach (IProject p in solution.Projects) {
					if (editPlatforms) {
						p.PlatformNames.Rename(oldName, newName);
					} else {
						p.ConfigurationNames.Rename(oldName, newName);
					}
				}
			}
			InitList();
		}
		
		bool EnsureCorrectName(ref string newName)
		{
			newName = editedCollection.ValidateName(newName);
			if (newName == null) {
				MessageService.ShowMessage("${res:Dialog.EditAvailableConfigurationsDialog.InvalidName}");
				return false;
			}
			foreach (string item in listBox.Items) {
				if (ConfigurationAndPlatform.ConfigurationNameComparer.Equals(item, newName)) {
					MessageService.ShowMessage("${res:Dialog.EditAvailableConfigurationsDialog.DuplicateName}");
					return false;
				}
			}
			return true;
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{
			using (AddNewConfigurationDialog dlg = new AddNewConfigurationDialog
			       (configurable is ISolution, editPlatforms,
			        editedCollection,
			        delegate (string name) { return EnsureCorrectName(ref name); }
			       ))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					editedCollection.Add(dlg.NewName, dlg.CopyFrom);
					if (dlg.CreateInAllProjects) {
						foreach (var project in ((ISolution)configurable).Projects) {
							if (editPlatforms)
								project.PlatformNames.Add(dlg.NewName, dlg.CopyFrom);
							else
								project.ConfigurationNames.Add(dlg.NewName, dlg.CopyFrom);
						}
					}
					InitList();
				}
			}
		}
	}
}
