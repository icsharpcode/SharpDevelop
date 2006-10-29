/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 10/28/2006
 * Time: 5:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.SettingsEditor
{
	/// <summary>
	/// Description of SettingsView.
	/// </summary>
	public partial class SettingsView
	{
		public SettingsView()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void GridCellContentClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
		{
			
		}
		
		public void ShowEntries(List<SettingsEntry> list)
		{
			grid.AutoGenerateColumns = false;
			grid.DataSource = list;
			if (grid.Columns.Count == 0) {
				grid.Columns.Add("Name", "Name");
				grid.Columns[0].DataPropertyName = "Name";
				grid.Columns.Add("Value", "Value");
				grid.Columns[1].DataPropertyName = "Value";
				grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				grid.Columns[1].MinimumWidth = 75;
			}
			grid.AllowUserToAddRows = true;
			/*foreach (SettingsEntry e in list) {
				grid.Rows.Add(e.Name,
				              e.SerializedSettingType,
				              e.Scope,
				              e.Value);
			}*/
		}
		
		void GridSelectionChanged(object sender, EventArgs e)
		{
			if (SelectionChanged != null)
				SelectionChanged(this, e);
		}
		
		public List<SettingsEntry> GetSelectedEntries()
		{
			List<SettingsEntry> l = new List<SettingsEntry>();
			if (grid.SelectedRows.Count > 0) {
				foreach (DataGridViewRow row in grid.SelectedRows) {
					l.Add((SettingsEntry)row.DataBoundItem);
				}
			} else {
				bool[] rowAdded = new bool[grid.Rows.Count];
				foreach (DataGridViewCell cell in grid.SelectedCells) {
					if (rowAdded[cell.RowIndex] == false) {
						rowAdded[cell.RowIndex] = true;
						l.Add((SettingsEntry)cell.OwningRow.DataBoundItem);
					}
				}
			}
			return l;
		}
		
		public event EventHandler SelectionChanged;
	}
}
