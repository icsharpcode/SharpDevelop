// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Dialog that allows the user to add standard compiler extensions. Only
	/// the standard compiler extensions are displayed.
	/// </summary>
	public class AddCompilerExtensionsDialog : System.Windows.Forms.Form
	{
		List<WixCompilerExtensionName> currentExtensions = new List<WixCompilerExtensionName>();
		bool changed;
		
		/// <summary>
		/// Creates a new instance of this dialog.
		/// </summary>
		/// <param name="currentExtensions">The current extensions selected
		/// by the user. This is used to tick the standard WiX extensions that
		/// have already been added to the project.</param>
		/// <param name="standardExtensions">The standard WiX extensions (e.g. NetFx)
		/// that are available for the user.</param>
		public AddCompilerExtensionsDialog(WixCompilerExtensionName[] currentExtensions, WixCompilerExtensionName[] standardExtensions)
		{
			InitializeComponent();
			InitStrings();
			this.currentExtensions.AddRange(currentExtensions);
			AddStandardExtensions(standardExtensions);
		}
		
		/// <summary>
		/// Gets whether one or more extensions have been added or removed
		/// by the user.
		/// </summary>
		public bool IsChanged {
			get {
				return changed;
			}
		}
		
		/// <summary>
		/// Gets the new set of extensions selected. This includes the current
		/// extensions that are not considered standard extensions and were
		/// passed to the constructor of this class.
		/// </summary>
		public WixCompilerExtensionName[] GetExtensions()
		{
			UpdateCurrentExtensions();
			
			List<WixCompilerExtensionName> extensions = new List<WixCompilerExtensionName>();
			foreach (WixCompilerExtensionName extensionName in currentExtensions) {
				extensions.Add(extensionName);
			}
			return extensions.ToArray();
		}
		
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
		
		#region Windows Forms Designer generated code

		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		void InitializeComponent()
		{
			this.extensionsListView = new System.Windows.Forms.ListView();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// extensionsListView
			// 
			this.extensionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.extensionsListView.CheckBoxes = true;
			this.extensionsListView.Location = new System.Drawing.Point(0, 0);
			this.extensionsListView.Name = "extensionsListView";
			this.extensionsListView.Size = new System.Drawing.Size(466, 255);
			this.extensionsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.extensionsListView.TabIndex = 0;
			this.extensionsListView.UseCompatibleStateImageBehavior = false;
			this.extensionsListView.View = System.Windows.Forms.View.List;
			this.extensionsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ExtensionsListViewItemChecked);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(294, 261);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(379, 261);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// AddCompilerExtensionsDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(466, 291);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.extensionsListView);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(200, 100);
			this.Name = "AddCompilerExtensionsDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Extensions";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ListView extensionsListView;
		
		#endregion
	
		void AddStandardExtensions(WixCompilerExtensionName[] extensions)
		{
			foreach (WixCompilerExtensionName extension in extensions) {
				ListViewItem item = extensionsListView.Items.Add(extension.DisplayName);
				item.Tag = extension;
				item.Checked = currentExtensions.Contains(extension);
			}
		}
		
		void ExtensionsListViewItemChecked(object sender, ItemCheckedEventArgs e)
		{
			changed = true;
		}
		
		void CreateCurrentExtensions(string[] extensions)
		{
			foreach (string extension in extensions) {
				currentExtensions.Add(new WixCompilerExtensionName(extension));
			}
		}
		
		/// <summary>
		/// Adds and removes any extensions that have been checked or 
		/// unchecked.
		/// </summary>
		void UpdateCurrentExtensions()
		{
			foreach (ListViewItem item in extensionsListView.Items) {
				WixCompilerExtensionName extension = (WixCompilerExtensionName)item.Tag;
				if (item.Checked) {
					if (!currentExtensions.Contains(extension)) {
						currentExtensions.Add(extension);
					}
				} else {
					int index = currentExtensions.IndexOf(extension);
					if (index >= 0) {
						currentExtensions.RemoveAt(index);
					}
				}
			}
		}
		
		void InitStrings()
		{
			okButton.Text = StringParser.Parse("${res:Global.OKButtonText}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
			Text = StringParser.Parse("${res:ICSharpCode.WixBinding.AddCompilerExtensionsDialog.Title}");
		}
	}
}
