// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
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
			if (solution == null)
				throw new Exception("A solution must be opened");
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.Text = StringParser.Parse(this.Text);
			label1.Text = StringParser.Parse(label1.Text);
			label2.Text = StringParser.Parse(label2.Text);
			closeButton.Text = StringParser.Parse(closeButton.Text);
			projectNameColumn.HeaderText = StringParser.Parse(projectNameColumn.HeaderText);
			configurationColumn.HeaderText = StringParser.Parse(configurationColumn.HeaderText);
			platformColumn.HeaderText = StringParser.Parse(platformColumn.HeaderText);
			
			inUpdate = true;
			UpdateAvailableSolutionConfigurationPlatforms();
			
			foreach (IProject p in solution.Projects) {
				DataGridViewRow row = grid.Rows[grid.Rows.Add()];
				row.Tag = p;
				row.Cells[0].Value = p.Name;
			}
			
			UpdateGrid();
		}
		
		void UpdateAvailableSolutionConfigurationPlatforms()
		{
			SetItems(configurationComboBox.Items, solution.GetConfigurationNames());
			SetItems(platformComboBox.Items, solution.GetPlatformNames());
			SelectElement(configurationComboBox, solution.Preferences.ActiveConfiguration);
			SelectElement(platformComboBox, solution.Preferences.ActivePlatform);
			
			string editItemText = EditTag.Instance.ToString();
			configurationComboBoxEditIndex = configurationComboBox.Items.Add(editItemText);
			platformComboBoxEditIndex      = platformComboBox.Items.Add(editItemText);
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
		
		sealed class EditTag
		{
			public static readonly EditTag Instance = new EditTag();
			
			public override string ToString()
			{
				return StringParser.Parse("${res:Dialog.Options.CombineOptions.Configurations.ConfigurationEditor.EditItem}");
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
					matching = new Solution.ProjectConfigurationPlatformMatching(p, p.ActiveConfiguration, p.ActivePlatform, null);
				}
				DataGridViewComboBoxCell c1 = (DataGridViewComboBoxCell)row.Cells[1];
				c1.Tag = matching;
				SetItems(c1.Items, p.ConfigurationNames);
				SelectElement(c1, matching.Configuration);
				c1.Items.Add(EditTag.Instance);
				
				DataGridViewComboBoxCell c2 = (DataGridViewComboBoxCell)row.Cells[2];
				c2.Tag = matching;
				SetItems(c2.Items, p.PlatformNames);
				SelectElement(c2, matching.Platform);
				c2.Items.Add(EditTag.Instance);
			}
			inUpdate = false;
		}
		
		void ConfigurationComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!inUpdate) {
				inUpdate = true;
				if (configurationComboBox.SelectedIndex == configurationComboBoxEditIndex) {
					using (Form dlg = new EditAvailableConfigurationsDialog(solution, false)) {
						dlg.ShowDialog(this);
					}
					UpdateAvailableSolutionConfigurationPlatforms();
				}
				UpdateGrid();
			}
		}
		
		void PlatformComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!inUpdate) {
				inUpdate = true;
				if (platformComboBox.SelectedIndex == platformComboBoxEditIndex) {
					using (Form dlg = new EditAvailableConfigurationsDialog(solution, true)) {
						dlg.ShowDialog(this);
					}
					UpdateAvailableSolutionConfigurationPlatforms();
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
					if (matching.Platform == "AnyCPU") {
						matching.Platform = "Any CPU";
					}
					
					if (matching.SolutionItem == null) {
						matching.SolutionItem = solution.CreateMatchingItem(configurationComboBox.Text,
						                                                    platformComboBox.Text,
						                                                    matching.Project, "");
					}
					matching.SetProjectConfigurationPlatform(solution.GetProjectConfigurationsSection(),
					                                         matching.Configuration, matching.Platform);
				}
			}
		}
		
		ComboBox gridEditingControl;
		
		public ComboBox GridEditingControl {
			get { return gridEditingControl; }
			set {
				if (gridEditingControl == value) return;
				if (gridEditingControl != null) {
					gridEditingControl.SelectedIndexChanged -= GridEditingControlSelectedIndexChanged;
				}
				gridEditingControl = value;
				if (gridEditingControl != null) {
					gridEditingControl.SelectedIndexChanged += GridEditingControlSelectedIndexChanged;
				}
			}
		}
		
		void GridEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			GridEditingControl = e.Control as ComboBox;
		}
		
		void GridEditingControlSelectedIndexChanged(object sender, EventArgs e)
		{
			if (gridEditingControl.SelectedItem == EditTag.Instance) {
				DataGridViewComboBoxCell cell = grid.CurrentCell as DataGridViewComboBoxCell;
				if (cell == null) return;
				Solution.ProjectConfigurationPlatformMatching matching = cell.Tag as Solution.ProjectConfigurationPlatformMatching;
				if (matching != null) {
					inUpdate = true;
					using (Form dlg = new EditAvailableConfigurationsDialog(matching.Project,
					                                                        cell.ColumnIndex != configurationColumn.Index))
					{
						dlg.ShowDialog(this);
					}
					
					grid.EndEdit();
					
					inUpdate = true;
					
					// end edit to allow updating the grid
					grid.EndEdit();
					
					// we need to change the current cell because otherwise UpdateGrid cannot change the
					// list of combobox items in this cell
					grid.CurrentCell = grid.Rows[cell.RowIndex].Cells[0];
					
					// remove cell.Value because otherwise the grid view crashes in UpdateGrid
					cell.Value = null;
					
					UpdateAvailableSolutionConfigurationPlatforms();
					UpdateGrid();
				}
			}
		}
	}
}
