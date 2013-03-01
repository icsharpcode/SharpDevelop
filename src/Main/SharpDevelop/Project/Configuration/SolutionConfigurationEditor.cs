// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Project
{
	internal partial class SolutionConfigurationEditor
	{
		readonly ISolution solution;
		
		bool inUpdate;
		int configurationComboBoxEditIndex;
		int platformComboBoxEditIndex;
		ConfigurationAndPlatform solutionConfig;
		
		public SolutionConfigurationEditor(ISolution solution)
		{
			if (solution == null)
				throw new ArgumentNullException("solution");
			this.solution = solution;
			
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
			SetItems(configurationComboBox.Items, solution.ConfigurationNames);
			SetItems(platformComboBox.Items, solution.PlatformNames);
			SelectElement(configurationComboBox, solution.ActiveConfiguration.Configuration);
			SelectElement(platformComboBox, solution.ActiveConfiguration.Platform);
			
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
			
			solutionConfig = new ConfigurationAndPlatform(configurationComboBox.Text, platformComboBox.Text);
			
			foreach (DataGridViewRow row in grid.Rows) {
				IProject p = (IProject)row.Tag;
				
				var projectConfig = p.ConfigurationMapping.GetProjectConfiguration(solutionConfig);
				
				DataGridViewComboBoxCell c1 = (DataGridViewComboBoxCell)row.Cells[1];
				SetItems(c1.Items, p.ConfigurationNames);
				SelectElement(c1, projectConfig.Configuration);
				c1.Items.Add(EditTag.Instance);
				
				DataGridViewComboBoxCell c2 = (DataGridViewComboBoxCell)row.Cells[2];
				SetItems(c2.Items, p.PlatformNames);
				SelectElement(c2, projectConfig.Platform);
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
				IProject project = (IProject)row.Tag;
				
				var newConfig = new ConfigurationAndPlatform(
					row.Cells[configurationColumn.Index].ToString(),
					row.Cells[platformColumn.Index].ToString());
				
				project.ConfigurationMapping.SetProjectConfiguration(solutionConfig, newConfig);
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
				IProject project = (IProject)cell.OwningRow.Tag;
				
				inUpdate = true;
				using (Form dlg = new EditAvailableConfigurationsDialog(project,
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
