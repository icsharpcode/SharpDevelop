// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
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
		TabPage comTabPage = new TabPage();
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
		
		public IProject ConfigureProject {
			get { return configureProject; }
		}
		
		public ArrayList ReferenceInformations {
			get {
				ArrayList referenceInformations = new ArrayList();
				foreach (ListViewItem item in referencesListView.Items) {
					System.Diagnostics.Debug.Assert(item.Tag != null);
					referenceInformations.Add(item.Tag);
				}
				return referenceInformations;
			}
		}
		
		public SelectReferenceDialog(IProject configureProject)
		{
			this.configureProject = configureProject;
			
			InitializeComponent();
			
			gacTabPage.Controls.Add(new GacReferencePanel(this));
			projectTabPage.Controls.Add(new ProjectReferencePanel(this));
			browserTabPage.Controls.Add(new AssemblyReferencePanel(this));
			
			comTabPage.Controls.Add(new COMReferencePanel(this));
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
			this.referencesListView = new System.Windows.Forms.ListView();
			this.selectButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			this.gacTabPage = new System.Windows.Forms.TabPage();
			this.projectTabPage = new System.Windows.Forms.TabPage();
			this.browserTabPage = new System.Windows.Forms.TabPage();
			this.referencesLabel = new System.Windows.Forms.Label();
			this.referenceHeader = new System.Windows.Forms.ColumnHeader();
			this.typeHeader = new System.Windows.Forms.ColumnHeader();
			this.locationHeader = new System.Windows.Forms.ColumnHeader();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.helpButton = new System.Windows.Forms.Button();
			this.referenceTabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// referenceTabControl
			// 
			this.referenceTabControl.Controls.AddRange(new System.Windows.Forms.Control[] {
			                                           	this.gacTabPage,
			                                           	this.projectTabPage,
			                                           	this.browserTabPage,
			                                           	this.comTabPage
			                                           });
			this.referenceTabControl.Location = new System.Drawing.Point(8, 8);
			this.referenceTabControl.SelectedIndex = 0;
			this.referenceTabControl.Size = new System.Drawing.Size(472, 224);
			this.referenceTabControl.TabIndex = 0;
			// 
			// referencesListView
			// 
			this.referencesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			                                         	this.referenceHeader,
			                                         	this.typeHeader,
			                                         	this.locationHeader});
			this.referencesListView.Location = new System.Drawing.Point(8, 256);
			this.referencesListView.Size = new System.Drawing.Size(472, 97);
			this.referencesListView.TabIndex = 3;
			this.referencesListView.View = System.Windows.Forms.View.Details;
			this.referencesListView.FullRowSelect = true;
			
			
			// 
			// selectButton
			// 
			this.selectButton.Location = new System.Drawing.Point(488, 32);
			this.selectButton.TabIndex = 1;
			this.selectButton.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.SelectButton");
			this.selectButton.Click += new EventHandler(SelectReference);
			this.selectButton.FlatStyle = FlatStyle.System;
			
			// 
			// removeButton
			// 
			this.removeButton.Location = new System.Drawing.Point(488, 256);
			this.removeButton.TabIndex = 4;
			this.removeButton.Text = ResourceService.GetString("Global.RemoveButtonText");
			this.removeButton.Click += new EventHandler(RemoveReference);
			this.removeButton.FlatStyle = FlatStyle.System;
			
			// 
			// gacTabPage
			// 
			this.gacTabPage.Location = new System.Drawing.Point(4, 22);
			this.gacTabPage.Size = new System.Drawing.Size(464, 198);
			this.gacTabPage.TabIndex = 0;
			this.gacTabPage.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.GacTabPage");
			this.gacTabPage.UseVisualStyleBackColor = true;
			// 
			// projectTabPage
			// 
			this.projectTabPage.Location = new System.Drawing.Point(4, 22);
			this.projectTabPage.Size = new System.Drawing.Size(464, 198);
			this.projectTabPage.TabIndex = 1;
			this.projectTabPage.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.ProjectTabPage");
			this.projectTabPage.UseVisualStyleBackColor = true;
			// 
			// browserTabPage
			// 
			this.browserTabPage.Location = new System.Drawing.Point(4, 22);
			this.browserTabPage.Size = new System.Drawing.Size(464, 198);
			this.browserTabPage.TabIndex = 2;
			this.browserTabPage.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.BrowserTabPage");
			this.browserTabPage.UseVisualStyleBackColor = true;
			
			this.comTabPage.Location = new System.Drawing.Point(4, 22);
			this.comTabPage.Size = new System.Drawing.Size(464, 198);
			this.comTabPage.TabIndex = 2;
			this.comTabPage.Text = "COM";
			this.comTabPage.UseVisualStyleBackColor = true;
			
			//
			// referencesLabel
			// 
			this.referencesLabel.Location = new System.Drawing.Point(8, 240);
			this.referencesLabel.Size = new System.Drawing.Size(472, 16);
			this.referencesLabel.TabIndex = 2;
			this.referencesLabel.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.ReferencesLabel");
			// 
			// referenceHeader
			// 
			this.referenceHeader.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.ReferenceHeader");
			this.referenceHeader.Width = 183;
			// 
			// typeHeader
			// 
			this.typeHeader.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.TypeHeader");
			this.typeHeader.Width = 57;
			// 
			// locationHeader
			// 
			this.locationHeader.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.LocationHeader");
			this.locationHeader.Width = 228;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(312, 368);
			this.okButton.TabIndex = 5;
			this.okButton.Text = ResourceService.GetString("Global.OKButtonText");
			this.okButton.Click += OkButtonClick;
			
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(400, 368);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
			this.cancelButton.FlatStyle = FlatStyle.System;
			
			//
			// helpButton
			// 
			this.helpButton.Location = new System.Drawing.Point(488, 368);
			this.helpButton.TabIndex = 7;
			this.helpButton.Text = ResourceService.GetString("Global.HelpButtonText");
			this.helpButton.FlatStyle = FlatStyle.System;
			
			//
			// SelectReferenceDialog
			// 
			this.AcceptButton = this.okButton;
//			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(570, 399);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
			                       	this.helpButton,
			                       	this.cancelButton,
			                       	this.okButton,
			                       	this.referencesLabel,
			                       	this.removeButton,
			                       	this.selectButton,
			                       	this.referencesListView,
			                       	this.referenceTabControl});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowInTaskbar = false;
			this.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.DialogName");
			this.referenceTabControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
	}
}
