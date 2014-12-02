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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
		
		sealed class EditTag
		{
			public static readonly EditTag Instance = new EditTag();
			
			public override string ToString()
			{
				return StringParser.Parse("${res:Dialog.Options.CombineOptions.Configurations.ConfigurationEditor.EditItem}");
			}
		}
		
		sealed class MissingItem
		{
			internal readonly string value;
			
			public MissingItem(string value)
			{
				this.value = value;
			}
			
			public override string ToString()
			{
				return "(" + value + ")";
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
				SetItemsAndSelect(c1, p.ConfigurationNames, projectConfig.Configuration);
				c1.Items.Add(EditTag.Instance);
				
				DataGridViewComboBoxCell c2 = (DataGridViewComboBoxCell)row.Cells[2];
				SetItemsAndSelect(c2, p.PlatformNames, projectConfig.Platform);
				c2.Items.Add(EditTag.Instance);
			}
			inUpdate = false;
		}
		
		void SetItemsAndSelect(DataGridViewComboBoxCell c, IEnumerable<string> items, string item)
		{
			SetItems(c.Items, items);
			if (items.Contains(item)) {
				c.Value = item;
			} else {
				var missingItem = new MissingItem(item);
				c.Items.Insert(0, missingItem);
				c.Value = missingItem;
			}
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
					GetValue(row.Cells[configurationColumn.Index]),
					GetValue(row.Cells[platformColumn.Index]));
				
				project.ConfigurationMapping.SetProjectConfiguration(solutionConfig, newConfig);
			}
		}
		
		string GetValue(DataGridViewCell dataGridViewCell)
		{
			var missingItem = dataGridViewCell.Value as MissingItem;
			if (missingItem != null)
				return missingItem.value;
			return dataGridViewCell.Value.ToString();
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

		protected override void OnClosing(CancelEventArgs e)
		{
			// Update solution active configuration when the dialog is closing.
			solution.ActiveConfiguration = solutionConfig;

			base.OnClosing(e);
		}
	}
}
