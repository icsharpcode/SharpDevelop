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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.WixBinding
{
	public class WixPackageFilesControl : System.Windows.Forms.UserControl, IWixPackageFilesView, IWixPackageFilesControl
	{
		bool errorMessageTextBoxVisible;
		bool diffVisible;
		WixPackageFilesEditor editor;
		bool dirty;
		WixProject project;
		WixXmlAttributeTypeDescriptor typeDescriptor = new WixXmlAttributeTypeDescriptor();
		WixXmlAttributeCollection wixXmlAttributes = new WixXmlAttributeCollection();
		
		public WixPackageFilesControl()
		{
			InitializeComponent();
			diffControl.ContextMenuStrip = MenuService.CreateContextMenu(this, "/AddIns/WixBinding/WixPackageFilesDiffControl/ContextMenu");
		}
		
		public TreeNode SelectedNode {
			get { return packageFilesTreeView.SelectedNode; }
		}
		
		/// <summary>
		/// Raised when the files are changed and require saving.
		/// </summary>
		public event EventHandler DirtyChanged;
		
		public bool ContextMenuEnabled {
			get { return packageFilesTreeView.ContextMenuStrip.Enabled; }
			set { packageFilesTreeView.ContextMenuStrip.Enabled = value; }
		}
				
		/// <summary>
		/// Gets or sets the error message that will be displayed instead of the
		/// property grid.
		/// </summary>
		public string ErrorMessage {
			get { return errorMessageTextBox.Text; }
			set { errorMessageTextBox.Text = value; }
		}
		
		/// <summary>
		/// Gets or sets whether the error message is visible. When visible the
		/// error message text box replaces the property grid.
		/// </summary>
		public bool IsErrorMessageTextBoxVisible {
			get { return errorMessageTextBoxVisible; }
			set {
				errorMessageTextBoxVisible = value;
				if (value) {
					errorMessageTextBox.BringToFront();
				} else {
					errorMessageTextBox.SendToBack();
				}
			}
		}
		
		/// <summary>
		/// Shows or hides the diff panel.
		/// </summary>
		public bool IsDiffVisible {
			get { return diffVisible; }
			set {
				if (diffVisible != value) {
					diffVisible = value;
					if (diffVisible) {
						ShowDiffPanel();
					} else {
						HideDiffPanel();
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the project that is currently being displayed.
		/// </summary>
		public WixProject Project {
			get { return project; }
		}
		
		/// <summary>
		/// Gets the document associated currently being displayed.
		/// </summary>
		public WixDocument Document {
			get {
				if (editor != null) {
					return editor.Document;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Adds a new element with the specified name to the tree.
		/// </summary>
		public void AddElement(string name)
		{
			editor.AddElement(name);
		}
		
		public bool IsDirty {
			get { return dirty; }
			set {
				bool oldValue = dirty;
				dirty = value;
				if (oldValue != value) {
					OnDirtyChanged();
				}
			}
		}
		
		/// <summary>
		/// Adds directories to the tree view.
		/// </summary>
		public void AddDirectories(WixDirectoryElement[] directories)
		{
			packageFilesTreeView.AddDirectories(directories);
		}
		
		/// <summary>
		/// Clears all the directories being displayed.
		/// </summary>
		public void ClearDirectories()
		{
			packageFilesTreeView.Clear();
		}
		
		public XmlElement SelectedElement {
			get { return packageFilesTreeView.SelectedElement; }
			set {
				packageFilesTreeView.SelectedElement = value;
				if (value == null) {
					ClearProperties();
				} 
			}
		}
		
		/// <summary>
		/// Gets the attributes for the selected xml element.
		/// </summary>
		public WixXmlAttributeCollection Attributes {
			get { return wixXmlAttributes; }
		}
		
		/// <summary>
		/// Redisplays the attributes after they have been changed.
		/// </summary>
		public void AttributesChanged()
		{
			typeDescriptor = new WixXmlAttributeTypeDescriptor(wixXmlAttributes);
			propertyGrid.SelectedObject = typeDescriptor;
		}
		
		public StringCollection AllowedChildElements {
			get { return packageFilesTreeView.AllowedChildElements; }
		}
		
		public void ShowNoRootDirectoryFoundMessage()
		{
			ShowErrorMessage(StringParser.Parse("${res:ICSharpCode.WixBinding.PackageFilesView.NoRootDirectoryFoundMessage}"));
		}
		
		public void ShowNoSourceFileFoundMessage(string projectName)
		{
			ShowErrorMessage(String.Format(StringParser.Parse("${res:ICSharpCode.WixBinding.PackageFilesView.NoWixFileFoundInProjectMessage}"), projectName));
		}
		
		public void ShowSourceFilesContainErrorsMessage()
		{
			ShowErrorMessage(StringParser.Parse("${res:ICSharpCode.WixBinding.PackageFilesView.AllWixFilesContainErrorsMessage}"));
		}
				
		/// <summary>
		/// Removes the selected element from the view only. The document is not
		/// changed by this method.
		/// </summary>
		public void RemoveElement(XmlElement element)
		{
			packageFilesTreeView.RemoveElement(element);
		}
		
		/// <summary>
		/// Removes the selected element from the tree view. This removes the
		/// element from the Wix document.
		/// </summary>
		public void RemoveSelectedElement()
		{
			editor.RemoveElement();
		}
		
		/// <summary>
		/// Adds a new element to the tree.
		/// </summary>
		/// <remarks>If no node is currently selected this element is added as a 
		/// root node.</remarks>
		public void AddElement(XmlElement element)
		{
			packageFilesTreeView.AddElement(element);
		}
		
		/// <summary>
		/// Shows the open file dialog allowing the user to browse for files to 
		/// add to the selected component element.
		/// </summary>
		public void AddFiles()
		{
			using (OpenFileDialog dialog = CreateOpenFileDialog()) {
				if (dialog.ShowDialog() == DialogResult.OK) {
					editor.AddFiles(dialog.FileNames);
				}
			}
		}
		
		/// <summary>
		/// Loads the Wix document containing the directory information and displays it.
		/// </summary>
		public void ShowFiles(WixProject project, ITextFileReader fileReader, IWixDocumentWriter documentWriter)
		{
			this.project = project;
			editor = new WixPackageFilesEditor(this, fileReader, documentWriter);
			editor.ExcludedItems.AddRange(GetExcludedItems());
			editor.ShowFiles(project);
		}
		
		/// <summary>
		/// Allows the user to browse for a directory and add it to the setup package.
		/// </summary>
		public void AddDirectory()
		{
			// Save selected node since displaying a dialog will change it
			// if no node is selected.
			TreeNode selectedNode = packageFilesTreeView.SelectedNode;
			
			// Allow the user to select a directory.
			string directory = SD.FileService.BrowseForFolder("${res:ICSharpCode.WixBinding.PackageFilesView.AddDirectoryDialog.Title}");
			if (directory != null) {
				packageFilesTreeView.SelectedNode = selectedNode;
				editor.AddDirectory(directory);
			}
		}
		
		/// <summary>
		/// Saves the changes made to the Wix document.
		/// </summary>
		public void Save()
		{
			editor.Save();
		}
		
		public void ShowNoDifferenceFoundMessage()
		{
			IsDiffVisible = true;
			diffControl.ShowNoDiffMessage();
		}
		
		/// <summary>
		/// Shows the specified diff results.
		/// </summary>
		public void ShowDiffResults(WixPackageFilesDiffResult[] diffResults)
		{
			IsDiffVisible = true;
			diffControl.ShowDiffResults(diffResults);
		}
		
		/// <summary>
		/// Determines any differences between the files defined in the
		/// Wix document and the files on the file system and displays
		/// the results.
		/// </summary>
		public void CalculateDiff()
		{
			editor.CalculateDiff();
		}
					
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
		
		#region Forms Designer generated code

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
			this.components = new System.ComponentModel.Container();
			ICSharpCode.SharpDevelop.Gui.ExtTreeViewComparer extTreeViewComparer1 = new ICSharpCode.SharpDevelop.Gui.ExtTreeViewComparer();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.packageFilesTreeView = new ICSharpCode.WixBinding.WixPackageFilesTreeView();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.errorMessageTextBox = new System.Windows.Forms.RichTextBox();
			this.diffSplitContainer = new System.Windows.Forms.SplitContainer();
			this.diffControl = new ICSharpCode.WixBinding.WixPackageFilesDiffControl();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.diffSplitContainer.Panel2.SuspendLayout();
			this.diffSplitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.packageFilesTreeView);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.propertyGrid);
			this.splitContainer.Panel2.Controls.Add(this.errorMessageTextBox);
			this.splitContainer.Panel2.Controls.Add(this.diffSplitContainer);
			this.splitContainer.Size = new System.Drawing.Size(561, 328);
			this.splitContainer.SplitterDistance = 186;
			this.splitContainer.TabIndex = 0;
			// 
			// packageFilesTreeView
			// 
			this.packageFilesTreeView.AllowDrop = true;
			this.packageFilesTreeView.CanClearSelection = true;
			this.packageFilesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.packageFilesTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.packageFilesTreeView.HideSelection = false;
			this.packageFilesTreeView.ImageIndex = 0;
			this.packageFilesTreeView.IsSorted = true;
			this.packageFilesTreeView.Location = new System.Drawing.Point(0, 0);
			this.packageFilesTreeView.Name = "packageFilesTreeView";
			this.packageFilesTreeView.NodeSorter = extTreeViewComparer1;
			this.packageFilesTreeView.SelectedElement = null;
			this.packageFilesTreeView.SelectedImageIndex = 0;
			this.packageFilesTreeView.Size = new System.Drawing.Size(186, 328);
			this.packageFilesTreeView.TabIndex = 0;
			this.packageFilesTreeView.SelectedElementChanged += new System.EventHandler(this.PackageFilesTreeViewSelectedElementChanged);
			// 
			// propertyGrid
			// 
			this.propertyGrid.CommandsVisibleIfAvailable = false;
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.propertyGrid.Size = new System.Drawing.Size(371, 328);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.ToolbarVisible = false;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyGridPropertyValueChanged);
			// 
			// errorMessageTextBox
			// 
			this.errorMessageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.errorMessageTextBox.Location = new System.Drawing.Point(0, 0);
			this.errorMessageTextBox.Name = "errorMessageTextBox";
			this.errorMessageTextBox.ReadOnly = true;
			this.errorMessageTextBox.Size = new System.Drawing.Size(371, 328);
			this.errorMessageTextBox.TabIndex = 2;
			this.errorMessageTextBox.Text = "";
			// 
			// diffSplitContainer
			// 
			this.diffSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.diffSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.diffSplitContainer.Name = "diffSplitContainer";
			this.diffSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// diffSplitContainer.Panel2
			// 
			this.diffSplitContainer.Panel2.Controls.Add(this.diffControl);
			this.diffSplitContainer.Size = new System.Drawing.Size(371, 328);
			this.diffSplitContainer.SplitterDistance = 159;
			this.diffSplitContainer.TabIndex = 3;
			this.diffSplitContainer.TabStop = false;
			// 
			// diffControl
			// 
			this.diffControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.diffControl.Location = new System.Drawing.Point(0, 0);
			this.diffControl.Name = "diffControl";
			this.diffControl.Size = new System.Drawing.Size(371, 165);
			this.diffControl.TabIndex = 0;
			// 
			// WixPackageFilesControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer);
			this.Name = "WixPackageFilesControl";
			this.Size = new System.Drawing.Size(561, 328);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.diffSplitContainer.Panel2.ResumeLayout(false);
			this.diffSplitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private ICSharpCode.WixBinding.WixPackageFilesDiffControl diffControl;
		private System.Windows.Forms.SplitContainer diffSplitContainer;
		private System.Windows.Forms.RichTextBox errorMessageTextBox;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private ICSharpCode.WixBinding.WixPackageFilesTreeView packageFilesTreeView;
		private System.Windows.Forms.SplitContainer splitContainer;
		
		#endregion
		
		void ShowErrorMessage(string message)
		{
			ErrorMessage = message;
			IsErrorMessageTextBoxVisible = true;
		}
		
		/// <summary>
		/// Clears the package files and any properties being displayed.
		/// </summary>
		void Clear()
		{
			ClearProperties();
			packageFilesTreeView.Clear();
		}
		
		void ClearProperties()
		{
			propertyGrid.SelectedObject = null;
			propertyGrid.SelectedObjects = null;
		}
		
		void PackageFilesTreeViewSelectedElementChanged(object sender, EventArgs e)
		{
			editor.SelectedElementChanged();
		}
		
		void OnDirtyChanged()
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, new EventArgs());
			}
		}
		
		void PropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			editor.AttributeChanged();
			packageFilesTreeView.RefreshSelectedElement();
		}
		
		OpenFileDialog CreateOpenFileDialog()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Multiselect = true;
			dialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			dialog.Title = StringParser.Parse("${res:ICSharpCode.WixBinding.PackageFilesView.AddFilesDialog.Title}");
			return dialog;
		}
		
		string[] GetExcludedItems()
		{
			List<string> extensions = AddInTree.BuildItems<string>("/AddIns/WixBinding/ExcludedItems", this);
			return extensions.ToArray();
		}
		
		void ShowDiffPanel()
		{
			// Add property grid to split container.
			splitContainer.Panel2.Controls.Remove(propertyGrid);
			diffSplitContainer.Panel1.Controls.Add(propertyGrid);			
			
			// Fix the tab order.
			diffSplitContainer.TabStop = true;

			// Bring the split container to the front.
			diffSplitContainer.BringToFront();
		}
		
		void HideDiffPanel()
		{
			// Remove the property grid from the split container back to 
			// this control.
			diffSplitContainer.Panel1.Controls.Remove(propertyGrid);			
			splitContainer.Panel2.Controls.Add(propertyGrid);

			// Fix the tab order.
			diffSplitContainer.TabStop = false;
			
			// Send the diff split container to the back.
			diffSplitContainer.SendToBack();
			
			// Bring the property grid back to the front.
			propertyGrid.BringToFront();
		}
	}
}
