// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.SettingsEditor
{
	public partial class SettingsView : UserControl, ISettingsEntryHost
	{
		public event EventHandler SelectionChanged;
		public event EventHandler SettingsChanged;
		
		static readonly Type[] defaultAvailableTypes = new Type[] {
			typeof(bool),
			typeof(byte),
			typeof(char),
			typeof(decimal),
			typeof(double),
			typeof(float),
			typeof(int),
			typeof(long),
			typeof(sbyte),
			typeof(short),
			typeof(string),
			typeof(System.Collections.Specialized.StringCollection),
			typeof(System.DateTime),
			typeof(System.Drawing.Color),
			typeof(System.Drawing.Font),
			typeof(System.Drawing.Point),
			typeof(System.Drawing.Size),
			typeof(System.Guid),
			typeof(System.TimeSpan),
			typeof(uint),
			typeof(ulong),
			typeof(ushort)
		};
		
		List<string> typeNames = new List<string>();
		List<Type> types = new List<Type>();
		IAmbience ambience;
		
		public SettingsView()
		{
			InitializeComponent();
			
			ambience = AmbienceService.GetCurrentAmbience();
			foreach (Type type in defaultAvailableTypes) {
				types.Add(type);
				typeNames.Add(ambience.GetIntrinsicTypeName(type.FullName));
			}
			foreach (SpecialTypeDescriptor d in SpecialTypeDescriptor.Descriptors) {
				types.Add(d.type);
				typeNames.Add(d.name);
			}
			
			ScopeColumn.DataSource = Enum.GetValues(typeof(SettingScope));
			TypeColumn.DataSource = typeNames;
		}
		
		public void ShowEntries(IList<SettingsEntry> list)
		{
			bindingSource.Clear();
			foreach (SettingsEntry entry in list) {
				bindingSource.Add(entry);
			}
			bindingSource.ListChanged += delegate(object sender, ListChangedEventArgs e) {
				if (e.NewIndex >= 0 && e.NewIndex < bindingSource.Count) {
					if (((SettingsEntry)bindingSource[e.NewIndex]).Name != null) {
						OnSettingsChanged(e);
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
			SettingsEntry entry = new SettingsEntry(this);
			entry.Type = typeof(string);
			e.NewObject = entry;
		}
		
		void GridDataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			LoggingService.Debug("Row " + e.RowIndex + ", column " + e.ColumnIndex + ", error " + e.Exception.ToString());
			if (e.Exception != null) {
				MessageBox.Show("Error in data entry: " + e.Exception.Message);
			} else {
				MessageBox.Show("Error in data entry");
			}
		}
		
		string ISettingsEntryHost.GetDisplayNameForType(Type type)
		{
			foreach (SpecialTypeDescriptor d in SpecialTypeDescriptor.Descriptors) {
				if (type == d.type)
					return d.name;
			}
			return ambience.GetIntrinsicTypeName(type.FullName);
		}
		
		Type ISettingsEntryHost.GetTypeByDisplayName(string displayName)
		{
			for (int i = 0; i < typeNames.Count; i++) {
				if (typeNames[i] == displayName)
					return types[i];
			}
			return null;
		}
		
		void GridUserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			if (e.Row != null && !e.Cancel) {
				OnSettingsChanged(EventArgs.Empty);
			}
		}
		
		protected virtual void OnSettingsChanged(EventArgs e)
		{
			if (SettingsChanged != null) {
				SettingsChanged(this, e);
			}
		}
	}
}
