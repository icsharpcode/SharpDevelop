// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.FormsDesigner.Gui
{
	partial class ConfigureSideBarDialog
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
			this.panel2 = new System.Windows.Forms.Panel();
			this.addComponentsButton = new System.Windows.Forms.Button();
			this.removeComponentsButton = new System.Windows.Forms.Button();
			this.renameCategoryButton = new System.Windows.Forms.Button();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.splitter = new System.Windows.Forms.Splitter();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.categoryListView = new System.Windows.Forms.ListView();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.cancelButton = new System.Windows.Forms.Button();
			this.columnHeader = new System.Windows.Forms.ColumnHeader();
			this.okButton = new System.Windows.Forms.Button();
			this.removeCategoryButton = new System.Windows.Forms.Button();
			this.componentListView = new System.Windows.Forms.ListView();
			this.newCategoryButton = new System.Windows.Forms.Button();
			this.panel = new System.Windows.Forms.Panel();
			this.panel2.SuspendLayout();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.addComponentsButton);
			this.panel2.Controls.Add(this.removeComponentsButton);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(166, 315);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(420, 40);
			this.panel2.TabIndex = 3;
			// 
			// addComponentsButton
			// 
			this.addComponentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.addComponentsButton.Location = new System.Drawing.Point(70, 8);
			this.addComponentsButton.Name = "addComponentsButton";
			this.addComponentsButton.Size = new System.Drawing.Size(166, 23);
			this.addComponentsButton.TabIndex = 3;
			this.addComponentsButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.AddCompone" +
			"ntsButton}";
			this.addComponentsButton.Click += new System.EventHandler(this.AddComponentsButtonClick);
			// 
			// removeComponentsButton
			// 
			this.removeComponentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.removeComponentsButton.Location = new System.Drawing.Point(246, 8);
			this.removeComponentsButton.Name = "removeComponentsButton";
			this.removeComponentsButton.Size = new System.Drawing.Size(166, 23);
			this.removeComponentsButton.TabIndex = 4;
			this.removeComponentsButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.RemoveComp" +
			"onentsButton}";
			this.removeComponentsButton.Click += new System.EventHandler(this.RemoveComponentsButtonClick);
			// 
			// renameCategoryButton
			// 
			this.renameCategoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.renameCategoryButton.Location = new System.Drawing.Point(200, 371);
			this.renameCategoryButton.Name = "renameCategoryButton";
			this.renameCategoryButton.Size = new System.Drawing.Size(90, 23);
			this.renameCategoryButton.TabIndex = 9;
			this.renameCategoryButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.RenameButt" +
			"on}";
			this.renameCategoryButton.Click += new System.EventHandler(this.RenameCategoryButtonClick);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Name = "columnHeader3";
			this.columnHeader3.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.Assembly}";
			this.columnHeader3.Width = 148;
			// 
			// splitter
			// 
			this.splitter.Location = new System.Drawing.Point(160, 0);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(6, 355);
			this.splitter.TabIndex = 1;
			this.splitter.TabStop = false;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Name = "columnHeader2";
			this.columnHeader2.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.Namespace}" +
			"";
			this.columnHeader2.Width = 145;
			// 
			// categoryListView
			// 
			this.categoryListView.CheckBoxes = true;
			this.categoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.columnHeader5});
			this.categoryListView.Dock = System.Windows.Forms.DockStyle.Left;
			this.categoryListView.FullRowSelect = true;
			this.categoryListView.HideSelection = false;
			this.categoryListView.Location = new System.Drawing.Point(0, 0);
			this.categoryListView.MultiSelect = false;
			this.categoryListView.Name = "categoryListView";
			this.categoryListView.Size = new System.Drawing.Size(160, 355);
			this.categoryListView.TabIndex = 0;
			this.categoryListView.UseCompatibleStateImageBehavior = false;
			this.categoryListView.View = System.Windows.Forms.View.Details;
			this.categoryListView.SelectedIndexChanged += new System.EventHandler(this.CategoryListViewSelectedIndexChanged);
			// 
			// columnHeader5
			// 
			this.columnHeader5.Name = "columnHeader5";
			this.columnHeader5.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.Categories" +
			"}";
			this.columnHeader5.Width = 156;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(519, 371);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 11;
			this.cancelButton.Text = "${res:Global.CancelButtonText}";
			// 
			// columnHeader
			// 
			this.columnHeader.Name = "columnHeader";
			this.columnHeader.Text = "${res:Global.Name}";
			this.columnHeader.Width = 123;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(439, 371);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 10;
			this.okButton.Text = "${res:Global.OKButtonText}";
			this.okButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// removeCategoryButton
			// 
			this.removeCategoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.removeCategoryButton.Location = new System.Drawing.Point(104, 371);
			this.removeCategoryButton.Name = "removeCategoryButton";
			this.removeCategoryButton.Size = new System.Drawing.Size(90, 23);
			this.removeCategoryButton.TabIndex = 8;
			this.removeCategoryButton.Text = "${res:Global.RemoveButtonText}";
			this.removeCategoryButton.Click += new System.EventHandler(this.RemoveCategoryButtonClick);
			// 
			// componentListView
			// 
			this.componentListView.CheckBoxes = true;
			this.componentListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.columnHeader,
									this.columnHeader2,
									this.columnHeader3});
			this.componentListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.componentListView.FullRowSelect = true;
			this.componentListView.GridLines = true;
			this.componentListView.HideSelection = false;
			this.componentListView.Location = new System.Drawing.Point(166, 0);
			this.componentListView.Name = "componentListView";
			this.componentListView.Size = new System.Drawing.Size(420, 315);
			this.componentListView.TabIndex = 2;
			this.componentListView.UseCompatibleStateImageBehavior = false;
			this.componentListView.View = System.Windows.Forms.View.Details;
			this.componentListView.SelectedIndexChanged += new System.EventHandler(this.ComponentListViewSelectedIndexChanged);
			// 
			// newCategoryButton
			// 
			this.newCategoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.newCategoryButton.Location = new System.Drawing.Point(8, 371);
			this.newCategoryButton.Name = "newCategoryButton";
			this.newCategoryButton.Size = new System.Drawing.Size(90, 23);
			this.newCategoryButton.TabIndex = 7;
			this.newCategoryButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.NewButton}" +
			"";
			this.newCategoryButton.Click += new System.EventHandler(this.NewCategoryButtonClick);
			// 
			// panel
			// 
			this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel.Controls.Add(this.componentListView);
			this.panel.Controls.Add(this.panel2);
			this.panel.Controls.Add(this.splitter);
			this.panel.Controls.Add(this.categoryListView);
			this.panel.Location = new System.Drawing.Point(8, 8);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(586, 355);
			this.panel.TabIndex = 6;
			// 
			// ConfigureSideBarDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(602, 402);
			this.Controls.Add(this.renameCategoryButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.removeCategoryButton);
			this.Controls.Add(this.newCategoryButton);
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigureSideBarDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ConfigureSidebarDialog.DialogName" +
			"}";
			this.panel2.ResumeLayout(false);
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Button newCategoryButton;
		private System.Windows.Forms.ListView componentListView;
		private System.Windows.Forms.Button removeCategoryButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ColumnHeader columnHeader;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ListView categoryListView;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Splitter splitter;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button renameCategoryButton;
		private System.Windows.Forms.Button removeComponentsButton;
		private System.Windows.Forms.Button addComponentsButton;
		private System.Windows.Forms.Panel panel2;
	}
}
