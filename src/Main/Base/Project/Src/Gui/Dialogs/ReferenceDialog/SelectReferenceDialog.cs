// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
	public class SelectReferenceDialog : System.Windows.Forms.Form, ISelectReferenceDialog
	{
		private System.Windows.Forms.ListView referencesListView;
		private System.Windows.Forms.Button selectButton;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.TabPage gacTabPage;
		private System.Windows.Forms.TabPage projectTabPage;
		private System.Windows.Forms.TabPage browserTabPage;
		private System.Windows.Forms.TabPage comTabPage;
		private System.Windows.Forms.Label referencesLabel;
		private System.Windows.Forms.ColumnHeader referenceHeader;
		private System.Windows.Forms.ColumnHeader typeHeader;
		private System.Windows.Forms.ColumnHeader locationHeader;
		private System.Windows.Forms.TabControl referenceTabControl;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button helpButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		IProject configureProject;

		/// <summary>
		/// Identify the different tabs
		/// </summary>
		[Flags]
		public enum TabTypes
		{
			GAC,
			Project,
			Assembly,
			COM
		};

		/// <summary>
		/// Dictionary to hold alternative panels for each tab (optional)
		/// </summary>
		private Dictionary<TabTypes, Control> alternativePanels = new Dictionary<TabTypes, Control>();

		public IProject ConfigureProject {
			get { return configureProject; }
		}
		
		public ArrayList ReferenceInformations {
			get {
				ArrayList referenceInformations = new ArrayList();
				foreach (ListViewItem item in referencesListView.Items)
				{
					System.Diagnostics.Debug.Assert(item.Tag != null);
					referenceInformations.Add(item.Tag);
				}
				return referenceInformations;
			}
		}

		public SelectReferenceDialog(IProject configureProject)
			: this(configureProject, true)
		{
		}

		/// <summary>
		/// alternative constructor.
		/// set defaultTabs=false if you wish to hide some tabs and\or set alternative panels.
		/// </summary>
		public SelectReferenceDialog(IProject configureProject, bool defaultTabs)
		{
			this.configureProject = configureProject;

			InitializeComponent();

			Translate(this);

			if (defaultTabs)
				InitializeAllTabs();   
		}

		/// <summary>
		/// Set and alternative panel for the specified tab.
		/// </summary>
		public void SetAlternativePanel(TabTypes tab, Control panel)
		{
			alternativePanels[tab] = panel;
		}

		/// <summary>
		/// Initialize all the default tabs.
		/// </summary>
		public void InitializeAllTabs()
		{
			InitializeTabs(TabTypes.GAC | TabTypes.Project | TabTypes.Assembly | TabTypes.COM);
		}

		/// <summary>
		/// Initialize only the specified tabs, and hide the remaining tabs
		/// </summary>
		public void InitializeTabs(TabTypes tabs)
		{
			InitializeTab(tabs, TabTypes.GAC);
			InitializeTab(tabs, TabTypes.Project);
			InitializeTab(tabs, TabTypes.Assembly);
			InitializeTab(tabs, TabTypes.COM);
		}

		private void InitializeTab(TabTypes tabs, TabTypes tab)
		{
			if ((tabs & tab) == tab) {
				GetTabPage(tab).Controls.Add(alternativePanels.ContainsKey(tab) ? alternativePanels[tab] : CreateTabPanel(tab));
			} else {
				referenceTabControl.TabPages.Remove(GetTabPage(tab));
			}
		}

		private TabPage GetTabPage(TabTypes tab)
		{
			switch (tab) {
				case TabTypes.GAC:
					return gacTabPage;
				case TabTypes.Project:
					return projectTabPage;
				case TabTypes.Assembly:
					return browserTabPage;
				case TabTypes.COM:
					return comTabPage;
			}

			throw new ArgumentOutOfRangeException("tab");
		}

		private Control CreateTabPanel(TabTypes tab)
		{
			switch (tab) {
				case TabTypes.GAC:
					return new GacReferencePanel(this);
				case TabTypes.Project:
					return new ProjectReferencePanel(this);
				case TabTypes.Assembly:
					return new AssemblyReferencePanel(this);
				case TabTypes.COM:
					return new COMReferencePanel(this);
			}

			throw new ArgumentOutOfRangeException("tab");
		}

		void Translate(Control ctl)
		{
			ctl.Text = StringParser.Parse(ctl.Text);
			foreach (Control c in ctl.Controls)
				Translate(c);
			if (ctl is ListView) {
				foreach (ColumnHeader h in ((ListView)ctl).Columns) {
					h.Text = StringParser.Parse(h.Text);
				}
			}
		}
		
		public void AddReference(string referenceName, string referenceType, string referenceLocation, ReferenceProjectItem projectItem)
		{
			if (projectItem == null)
				throw new ArgumentNullException("projectItem");
			
			foreach (ListViewItem item in referencesListView.Items) {
				if (referenceLocation == item.SubItems[2].Text && referenceName == item.Text ) {
					return;
				}
			}
			
			ListViewItem newItem = new ListViewItem(new string[] {referenceName, referenceType, referenceLocation});
			newItem.Tag = projectItem;
			referencesListView.Items.Add(newItem);
		}
		
		void SelectReference(object sender, EventArgs e)
		{
			IReferencePanel refPanel = (IReferencePanel)referenceTabControl.SelectedTab.Controls[0];
			refPanel.AddReference();
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			if (referencesListView.Items.Count == 0) {
				SelectReference(sender, e);
			}
		}
		
		void RemoveReference(object sender, EventArgs e)
		{
			ArrayList itemsToDelete = new ArrayList();
			
			foreach (ListViewItem item in referencesListView.SelectedItems) {
				itemsToDelete.Add(item);
			}
			
			foreach (ListViewItem item in itemsToDelete) {
				referencesListView.Items.Remove(item);
			}
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.referenceTabControl = new System.Windows.Forms.TabControl();
			this.gacTabPage = new System.Windows.Forms.TabPage();
			this.projectTabPage = new System.Windows.Forms.TabPage();
			this.browserTabPage = new System.Windows.Forms.TabPage();
			this.comTabPage = new System.Windows.Forms.TabPage();
			this.referencesListView = new System.Windows.Forms.ListView();
			this.referenceHeader = new System.Windows.Forms.ColumnHeader();
			this.typeHeader = new System.Windows.Forms.ColumnHeader();
			this.locationHeader = new System.Windows.Forms.ColumnHeader();
			this.selectButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			this.referencesLabel = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.helpButton = new System.Windows.Forms.Button();
			this.referenceTabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// referenceTabControl
			// 
			this.referenceTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.referenceTabControl.Controls.Add(this.gacTabPage);
			this.referenceTabControl.Controls.Add(this.projectTabPage);
			this.referenceTabControl.Controls.Add(this.browserTabPage);
			this.referenceTabControl.Controls.Add(this.comTabPage);
			this.referenceTabControl.Location = new System.Drawing.Point(8, 8);
			this.referenceTabControl.Name = "referenceTabControl";
			this.referenceTabControl.SelectedIndex = 0;
			this.referenceTabControl.Size = new System.Drawing.Size(472, 224);
			this.referenceTabControl.TabIndex = 0;
			// 
			// gacTabPage
			// 
			this.gacTabPage.Location = new System.Drawing.Point(4, 22);
			this.gacTabPage.Name = "gacTabPage";
			this.gacTabPage.Size = new System.Drawing.Size(464, 198);
			this.gacTabPage.TabIndex = 0;
			this.gacTabPage.Text = "${res:Dialog.SelectReferenceDialog.GacTabPage}";
			this.gacTabPage.UseVisualStyleBackColor = true;
			// 
			// projectTabPage
			// 
			this.projectTabPage.Location = new System.Drawing.Point(4, 22);
			this.projectTabPage.Name = "projectTabPage";
			this.projectTabPage.Size = new System.Drawing.Size(464, 198);
			this.projectTabPage.TabIndex = 1;
			this.projectTabPage.Text = "${res:Dialog.SelectReferenceDialog.ProjectTabPage}";
			this.projectTabPage.UseVisualStyleBackColor = true;
			// 
			// browserTabPage
			// 
			this.browserTabPage.Location = new System.Drawing.Point(4, 22);
			this.browserTabPage.Name = "browserTabPage";
			this.browserTabPage.Size = new System.Drawing.Size(464, 198);
			this.browserTabPage.TabIndex = 2;
			this.browserTabPage.Text = "${res:Dialog.SelectReferenceDialog.BrowserTabPage}";
			this.browserTabPage.UseVisualStyleBackColor = true;
			// 
			// comTabPage
			// 
			this.comTabPage.Location = new System.Drawing.Point(4, 22);
			this.comTabPage.Name = "comTabPage";
			this.comTabPage.Size = new System.Drawing.Size(464, 198);
			this.comTabPage.TabIndex = 2;
			this.comTabPage.Text = "COM";
			this.comTabPage.UseVisualStyleBackColor = true;
			// 
			// referencesListView
			// 
			this.referencesListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.referencesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.referenceHeader,
									this.typeHeader,
									this.locationHeader});
			this.referencesListView.FullRowSelect = true;
			this.referencesListView.Location = new System.Drawing.Point(8, 265);
			this.referencesListView.Name = "referencesListView";
			this.referencesListView.Size = new System.Drawing.Size(472, 97);
			this.referencesListView.TabIndex = 3;
			this.referencesListView.UseCompatibleStateImageBehavior = false;
			this.referencesListView.View = System.Windows.Forms.View.Details;
			// 
			// referenceHeader
			// 
			this.referenceHeader.Text = "${res:Dialog.SelectReferenceDialog.ReferenceHeader}";
			this.referenceHeader.Width = 183;
			// 
			// typeHeader
			// 
			this.typeHeader.Text = "${res:Dialog.SelectReferenceDialog.TypeHeader}";
			this.typeHeader.Width = 57;
			// 
			// locationHeader
			// 
			this.locationHeader.Text = "${res:Dialog.SelectReferenceDialog.LocationHeader}";
			this.locationHeader.Width = 228;
			// 
			// selectButton
			// 
			this.selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.selectButton.Location = new System.Drawing.Point(488, 32);
			this.selectButton.Name = "selectButton";
			this.selectButton.Size = new System.Drawing.Size(75, 23);
			this.selectButton.TabIndex = 1;
			this.selectButton.Text = "${res:Dialog.SelectReferenceDialog.SelectButton}";
			this.selectButton.Click += new System.EventHandler(this.SelectReference);
			// 
			// removeButton
			// 
			this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.removeButton.Location = new System.Drawing.Point(488, 256);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new System.Drawing.Size(75, 23);
			this.removeButton.TabIndex = 4;
			this.removeButton.Text = "${res:Global.RemoveButtonText}";
			this.removeButton.Click += new System.EventHandler(this.RemoveReference);
			// 
			// referencesLabel
			// 
			this.referencesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.referencesLabel.Location = new System.Drawing.Point(8, 240);
			this.referencesLabel.Name = "referencesLabel";
			this.referencesLabel.Size = new System.Drawing.Size(472, 16);
			this.referencesLabel.TabIndex = 2;
			this.referencesLabel.Text = "${res:Dialog.SelectReferenceDialog.ReferencesLabel}";
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(395, 368);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "${res:Global.OKButtonText}";
			this.okButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(483, 368);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "${res:Global.CancelButtonText}";
			// 
			// helpButton
			// 
			this.helpButton.Location = new System.Drawing.Point(140, 368);
			this.helpButton.Name = "helpButton";
			this.helpButton.Size = new System.Drawing.Size(75, 23);
			this.helpButton.TabIndex = 7;
			this.helpButton.Text = "${res:Global.HelpButtonText}";
			this.helpButton.Visible = false;
			// 
			// SelectReferenceDialog
			// 
			this.AcceptButton = this.okButton;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(570, 399);
			this.Controls.Add(this.helpButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.referencesLabel);
			this.Controls.Add(this.removeButton);
			this.Controls.Add(this.selectButton);
			this.Controls.Add(this.referencesListView);
			this.Controls.Add(this.referenceTabControl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(280, 350);
			this.Name = "SelectReferenceDialog";
			this.ShowInTaskbar = false;
			this.Text = "${res:Dialog.SelectReferenceDialog.DialogName}";
			this.referenceTabControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
	}
	
	public interface IReferencePanel
	{
		void AddReference();
	}
	
	public interface ISelectReferenceDialog
	{
		/// <summary>
		/// Project to create references for.
		/// </summary>
		IProject ConfigureProject { get; }
		
		void AddReference(string referenceName, string referenceType, string referenceLocation, ReferenceProjectItem projectItem);
	}
	
	public enum ReferenceType {
		Assembly,
		Typelib,
		Gac,
		
		Project
	}
}
