/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 10/28/2006
 * Time: 5:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TypeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.ScopeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.grid.AutoGenerateColumns = false;
			this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
									this.NameColumn,
									this.TypeColumn,
									this.ScopeColumn,
									this.ValueColumn});
			this.grid.DataSource = this.bindingSource;
			this.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.grid.Location = new System.Drawing.Point(3, 38);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(480, 321);
			this.grid.TabIndex = 0;
			this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.GridDataError);
			this.grid.SelectionChanged += new System.EventHandler(this.GridSelectionChanged);
			// 
			// bindingSource
			// 
			this.bindingSource.DataSource = typeof(ICSharpCode.SettingsEditor.SettingsEntry);
			this.bindingSource.AddingNew += new System.ComponentModel.AddingNewEventHandler(this.BindingSourceAddingNew);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(292, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "SettingsView prototype. Code generation not implemented!";
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
			// 
			// ScopeColumn
			// 
			this.ScopeColumn.DataPropertyName = "Scope";
			this.ScopeColumn.DropDownWidth = 80;
			this.ScopeColumn.HeaderText = "Scope";
			this.ScopeColumn.MinimumWidth = 30;
			this.ScopeColumn.Name = "ScopeColumn";
			// 
			// ValueColumn
			// 
			this.ValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ValueColumn.DataPropertyName = "SerializedValue";
			this.ValueColumn.HeaderText = "Value";
			this.ValueColumn.MinimumWidth = 50;
			this.ValueColumn.Name = "ValueColumn";
			// 
			// SettingsView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.grid);
			this.Name = "SettingsView";
			this.Size = new System.Drawing.Size(486, 362);
			((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.DataGridViewComboBoxColumn TypeColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn ScopeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
		private System.Windows.Forms.BindingSource bindingSource;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridView grid;
	}
}
