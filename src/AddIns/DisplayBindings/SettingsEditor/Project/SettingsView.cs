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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SettingsEditor
{
	public partial class SettingsView : UserControl
	{
		public event EventHandler SelectionChanged;
		public event EventHandler SettingsChanged;
		
		public SettingsView()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			ScopeColumn.DataSource = Enum.GetValues(typeof(SettingScope));
		}
		
		public void ShowEntries(IList<SettingsEntry> list)
		{
			foreach (SettingsEntry entry in list) {
				bindingSource.Add(entry);
			}
			bindingSource.ListChanged += delegate(object sender, ListChangedEventArgs e) {
				if (e.NewIndex >= 0 && e.NewIndex < bindingSource.Count) {
					if (((SettingsEntry)bindingSource[e.NewIndex]).Name != null) {
						if (SettingsChanged != null) {
							SettingsChanged(this, e);
						}
					}
				}
			};
		}
		
		void GridSelectionChanged(object sender, EventArgs e)
		{
			if (SelectionChanged != null)
				SelectionChanged(this, e);
		}
		
		public IEnumerable<SettingsEntry> GetAllEntries()
		{
			List<SettingsEntry> l = new List<SettingsEntry>();
			foreach (SettingsEntry entry in bindingSource) {
				if (!string.IsNullOrEmpty(entry.Name)) {
					l.Add(entry);
				}
			}
			l.Sort(delegate(SettingsEntry a, SettingsEntry b) {
			       	return a.Name.CompareTo(b.Name);
			       });
			return l;
		}
		
		public List<SettingsEntryPropertyGridWrapper> GetSelectedEntriesForPropertyGrid()
		{
			List<SettingsEntryPropertyGridWrapper> l
				= new List<SettingsEntryPropertyGridWrapper>();
			if (grid.SelectedRows.Count > 0) {
				foreach (DataGridViewRow row in grid.SelectedRows) {
					if (row.DataBoundItem != null) {
						l.Add(new SettingsEntryPropertyGridWrapper((SettingsEntry)row.DataBoundItem));
					}
				}
			} else {
				bool[] rowAdded = new bool[grid.Rows.Count];
				foreach (DataGridViewCell cell in grid.SelectedCells) {
					if (rowAdded[cell.RowIndex] == false) {
						rowAdded[cell.RowIndex] = true;
						if (cell.OwningRow.DataBoundItem != null) {
							l.Add(new SettingsEntryPropertyGridWrapper((SettingsEntry)cell.OwningRow.DataBoundItem));
						}
					}
				}
			}
			return l;
		}
		
		void BindingSourceAddingNew(object sender, AddingNewEventArgs e)
		{
			SettingsEntry entry = new SettingsEntry();
			entry.Type = typeof(string);
			e.NewObject = entry;
		}
	}
}
