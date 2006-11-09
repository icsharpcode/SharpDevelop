// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class EditAvailableConfigurationsDialog
	{
		Solution solution;
		IProject project;
		bool editPlatforms;
		
		public EditAvailableConfigurationsDialog(Solution solution, bool editPlatforms)
			: this()
		{
			this.solution = solution;
			this.editPlatforms = editPlatforms;
			InitList();
		}
		
		public EditAvailableConfigurationsDialog(IProject project, bool editPlatforms)
			: this()
		{
			this.project = project;
			this.solution = project.ParentSolution;
			this.editPlatforms = editPlatforms;
			InitList();
		}
		
		void InitList()
		{
			if (project != null) {
				if (editPlatforms) {
					ShowEntries(project.GetPlatformNames(), project.Platform);
				} else {
					ShowEntries(project.GetConfigurationNames(), project.Configuration);
				}
			} else {
				if (editPlatforms) {
					ShowEntries(solution.GetPlatformNames(), solution.Preferences.ActivePlatform);
				} else {
					ShowEntries(solution.GetConfigurationNames(), solution.Preferences.ActiveConfiguration);
				}
			}
		}
		
		void ShowEntries(IEnumerable<string> list, string activeItem)
		{
			string[] array = Linq.ToArray(list);
			listBox.Items.Clear();
			listBox.Items.AddRange(array);
			if (listBox.Items.Count == 0) {
				throw new Exception("There must be at least one configuration/platform");
			}
			listBox.SelectedIndex = Math.Max(Array.IndexOf(array, activeItem), 0);
		}
		
		private EditAvailableConfigurationsDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			foreach (Control ctl in this.Controls) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
		}
		
		void RemoveButtonClick(object sender, EventArgs e)
		{
			if (listBox.Items.Count == 1) {
				MessageService.ShowMessage("You cannot delete all configurations/platforms.");
			}
			string name = listBox.SelectedItem.ToString();
			if (MessageService.AskQuestionFormatted("Do you really want to remove '{0}'?",
			                                        new string[] { name }))
			{
				if (project != null) {
					Remove(project, name, editPlatforms);
				} else {
					Remove(solution, name, editPlatforms);
				}
			}
		}
		
		static void Remove(IProject project, string name, bool isPlatform)
		{
			throw new NotImplementedException();
		}
		
		static void Remove(Solution solution, string name, bool isPlatform)
		{
			throw new NotImplementedException();
		}
		
		void RenameButtonClick(object sender, EventArgs e)
		{
			string oldName = listBox.SelectedItem.ToString();
			string newName = MessageService.ShowInputBox("Rename", "Enter the new name:", oldName);
			if (string.IsNullOrEmpty(newName) || newName == oldName)
				return;
			if (!EnsureCorrectName(ref newName))
				return;
			if (project != null) {
				Rename(project, oldName, newName);
			} else {
				if (editPlatforms) {
					solution.RenameSolutionPlatform(oldName, newName);
				} else {
					solution.RenameSolutionConfiguration(oldName, newName);
				}
				// Solution platform name => project platform name
				foreach (IProject p in solution.Projects) {
					Rename(p, oldName, newName);
				}
			}
			InitList();
		}
		
		void Rename(IProject project, string oldName, string newName)
		{
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
		
		bool EnsureCorrectName(ref string newName)
		{
			newName = newName.Trim();
			if (editPlatforms && string.Equals(newName, "AnyCPU", StringComparison.InvariantCultureIgnoreCase))
				newName = "Any CPU";
			if (listBox.Items.Contains(newName)) {
				MessageService.ShowMessage("Duplicate name.");
				return false;
			}
			if (!FileUtility.IsValidDirectoryName(newName)) {
				MessageService.ShowMessage("The name was invalid.");
				return false;
			}
			return true;
		}
	}
}
