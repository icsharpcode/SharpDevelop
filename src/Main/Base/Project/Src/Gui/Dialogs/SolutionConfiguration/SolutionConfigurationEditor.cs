// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class SolutionConfigurationEditor
	{
		Solution solution;
		
		bool inUpdate;
		int configurationComboBoxEditIndex;
		int platformComboBoxEditIndex;
		
		public SolutionConfigurationEditor()
		{
			this.solution = ProjectService.OpenSolution;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.Text = StringParser.Parse(this.Text);
			label1.Text = StringParser.Parse(label1.Text);
			label2.Text = StringParser.Parse(label2.Text);
			okButton.Text = StringParser.Parse(okButton.Text);
			//cancelButton.Text = StringParser.Parse(cancelButton.Text);
			
			inUpdate = true;
			
			SetItems(configurationComboBox.Items, solution.GetConfigurationNames());
			SetItems(platformComboBox.Items, solution.GetPlatformNames());
			SelectElement(configurationComboBox, solution.Preferences.ActiveConfiguration);
			SelectElement(platformComboBox, solution.Preferences.ActivePlatform);
			
			configurationComboBoxEditIndex = configurationComboBox.Items.Add("<Edit>");
			platformComboBoxEditIndex      = platformComboBox.Items.Add("<Edit>");
			
			foreach (IProject p in solution.Projects) {
				DataGridViewRow row = grid.Rows[grid.Rows.Add()];
				row.Tag = p;
				row.Cells[0].Value = p.Name;
			}
			
			UpdateGrid();
		}
		
		void SetItems(IList items, IEnumerable<string> elements)
		{
			items.Clear();
			foreach (string e in elements) items.Add(e);
		}
		
		void SelectElement(ComboBox box, string itemName)
		{
			box.SelectedIndex = box.Items.IndexOf(itemName);
		}
		void SelectElement(DataGridViewComboBoxCell box, string itemName)
		{
			if (box.Items.IndexOf(itemName) == -1) {
				if (itemName == "Any CPU" && box.Items.IndexOf("AnyCPU") >= 0) {
					box.Value = "AnyCPU";
				} else {
					box.Value = box.Items[0];
				}
			} else {
				box.Value = itemName;
			}
		}
		
		void UpdateGrid()
		{
			inUpdate = true;
			
			Dictionary<IProject, Solution.ProjectConfigurationPlatformMatching> matchingDict =
				new Dictionary<IProject, Solution.ProjectConfigurationPlatformMatching>();
			foreach (Solution.ProjectConfigurationPlatformMatching matching in
			         solution.GetActiveConfigurationsAndPlatformsForProjects(configurationComboBox.Text,
			                                                                 platformComboBox.Text))
			{
				matchingDict[matching.Project] = matching;
			}
			
			foreach (DataGridViewRow row in grid.Rows) {
				IProject p = (IProject)row.Tag;
				
				Solution.ProjectConfigurationPlatformMatching matching;
				if (!matchingDict.TryGetValue(p, out matching)) {
					matching = new Solution.ProjectConfigurationPlatformMatching(p, p.Configuration, p.Platform, null);
				}
				DataGridViewComboBoxCell c1 = (DataGridViewComboBoxCell)row.Cells[1];
				c1.Tag = matching;
				SetItems(c1.Items, p.GetConfigurationNames());
				SelectElement(c1, matching.Configuration);
				
				DataGridViewComboBoxCell c2 = (DataGridViewComboBoxCell)row.Cells[2];
				c2.Tag = matching;
				SetItems(c2.Items, p.GetPlatformNames());
				SelectElement(c2, matching.Platform);
			}
			inUpdate = false;
		}
		
		void ConfigurationComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!inUpdate) {
				inUpdate = true;
				if (configurationComboBox.SelectedIndex == configurationComboBoxEditIndex) {
					MessageBox.Show("Edit configurations: Feature not implemented yet.");
					SelectElement(configurationComboBox, solution.Preferences.ActiveConfiguration);
				}
				UpdateGrid();
			}
		}
		
		void PlatformComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!inUpdate) {
				inUpdate = true;
				if (platformComboBox.SelectedIndex == platformComboBoxEditIndex) {
					MessageBox.Show("Edit platforms: Feature not implemented yet.");
					SelectElement(platformComboBox, solution.Preferences.ActivePlatform);
				}
				UpdateGrid();
			}
		}
		
		void GridDataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			e.ThrowException = true;
		}
		
		void GridCellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (!inUpdate && e.RowIndex >= 0) {
				DataGridViewRow row = grid.Rows[e.RowIndex];
				DataGridViewCell cell = row.Cells[e.ColumnIndex];
				Solution.ProjectConfigurationPlatformMatching matching = cell.Tag as Solution.ProjectConfigurationPlatformMatching;
				if (matching != null) {
					if (e.ColumnIndex == configurationColumn.Index) {
						matching.Configuration = cell.Value.ToString();
					} else {
						matching.Platform = cell.Value.ToString();
					}
					
					if (matching.SolutionItem == null) {
						matching.SolutionItem = solution.CreateMatchingItem(configurationComboBox.Text,
						                                                    platformComboBox.Text,
						                                                    matching.Project);
					}
					matching.SolutionItem.Location = matching.Configuration + "|" + matching.Platform;
				}
			}
		}
	}
}
