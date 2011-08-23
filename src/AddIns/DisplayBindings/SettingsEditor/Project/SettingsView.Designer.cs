// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.SettingsEditor
{
	partial class SettingsView
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.grid = new System.Windows.Forms.DataGridView();
			this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TypeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.ScopeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
			((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.AutoGenerateColumns = false;
			this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
									this.NameColumn,
									this.TypeColumn,
									this.ScopeColumn,
									this.ValueColumn});
			this.grid.DataSource = this.bindingSource;
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(486, 362);
			this.grid.TabIndex = 0;
			this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.GridDataError);
			this.grid.SelectionChanged += new System.EventHandler(this.GridSelectionChanged);
			this.grid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.GridUserDeletingRow);
			// 
			// NameColumn
			// 
			this.NameColumn.DataPropertyName = "Name";
			this.NameColumn.HeaderText = "Name";
			this.NameColumn.MinimumWidth = 50;
			this.NameColumn.Name = "NameColumn";
			// 
			// TypeColumn
			// 
			this.TypeColumn.DataPropertyName = "WrappedSettingType";
			this.TypeColumn.DropDownWidth = 255;
			this.TypeColumn.HeaderText = "Type";
			this.TypeColumn.MinimumWidth = 50;
			this.TypeColumn.Name = "TypeColumn";
			this.TypeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// ScopeColumn
			// 
			this.ScopeColumn.DataPropertyName = "Scope";
			this.ScopeColumn.DropDownWidth = 80;
			this.ScopeColumn.HeaderText = "Scope";
			this.ScopeColumn.MinimumWidth = 30;
			this.ScopeColumn.Name = "ScopeColumn";
			this.ScopeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// ValueColumn
			// 
			this.ValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ValueColumn.DataPropertyName = "SerializedValue";
			this.ValueColumn.HeaderText = "Value";
			this.ValueColumn.MinimumWidth = 50;
			this.ValueColumn.Name = "ValueColumn";
			// 
			// bindingSource
			// 
			this.bindingSource.DataSource = typeof(ICSharpCode.SettingsEditor.SettingsEntry);
			this.bindingSource.AddingNew += new System.ComponentModel.AddingNewEventHandler(this.BindingSourceAddingNew);
			// 
			// SettingsView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grid);
			this.Name = "SettingsView";
			this.Size = new System.Drawing.Size(486, 362);
			((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.DataGridViewComboBoxColumn TypeColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn ScopeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
		private System.Windows.Forms.BindingSource bindingSource;
		private System.Windows.Forms.DataGridView grid;
	}
}
