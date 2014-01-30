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

namespace ICSharpCode.FormsDesigner.Gui
{
	partial class ImageResourceEditorDialog
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
			System.Windows.Forms.Button cancelButton;
			System.Windows.Forms.SplitContainer splitContainer;
			System.Windows.Forms.GroupBox resourceSelectionGroup;
			System.Windows.Forms.GroupBox previewGroup;
			this.projectResourcesTreeView = new System.Windows.Forms.TreeView();
			this.projectResourceRadioButton = new System.Windows.Forms.RadioButton();
			this.importLocalResourceButton = new System.Windows.Forms.Button();
			this.noResourceRadioButton = new System.Windows.Forms.RadioButton();
			this.localResourceRadioButton = new System.Windows.Forms.RadioButton();
			this.previewPictureBox = new System.Windows.Forms.PictureBox();
			this.okButton = new System.Windows.Forms.Button();
			this.projectTreeScanningBackgroundWorker = new System.ComponentModel.BackgroundWorker();
			cancelButton = new System.Windows.Forms.Button();
			splitContainer = new System.Windows.Forms.SplitContainer();
			resourceSelectionGroup = new System.Windows.Forms.GroupBox();
			previewGroup = new System.Windows.Forms.GroupBox();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			resourceSelectionGroup.SuspendLayout();
			previewGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.Location = new System.Drawing.Point(509, 317);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(75, 23);
			cancelButton.TabIndex = 1;
			cancelButton.Text = "${res:Global.CancelButtonText}";
			cancelButton.UseVisualStyleBackColor = true;
			// 
			// splitContainer
			// 
			splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			splitContainer.Location = new System.Drawing.Point(12, 12);
			splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			splitContainer.Panel1.Controls.Add(resourceSelectionGroup);
			// 
			// splitContainer.Panel2
			// 
			splitContainer.Panel2.Controls.Add(previewGroup);
			splitContainer.Size = new System.Drawing.Size(572, 299);
			splitContainer.SplitterDistance = 280;
			splitContainer.TabIndex = 2;
			// 
			// resourceSelectionGroup
			// 
			resourceSelectionGroup.Controls.Add(this.projectResourcesTreeView);
			resourceSelectionGroup.Controls.Add(this.projectResourceRadioButton);
			resourceSelectionGroup.Controls.Add(this.importLocalResourceButton);
			resourceSelectionGroup.Controls.Add(this.noResourceRadioButton);
			resourceSelectionGroup.Controls.Add(this.localResourceRadioButton);
			resourceSelectionGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			resourceSelectionGroup.Location = new System.Drawing.Point(0, 0);
			resourceSelectionGroup.Name = "resourceSelectionGroup";
			resourceSelectionGroup.Size = new System.Drawing.Size(280, 299);
			resourceSelectionGroup.TabIndex = 0;
			resourceSelectionGroup.TabStop = false;
			// 
			// projectResourcesTreeView
			// 
			this.projectResourcesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.projectResourcesTreeView.HideSelection = false;
			this.projectResourcesTreeView.Location = new System.Drawing.Point(26, 138);
			this.projectResourcesTreeView.Name = "projectResourcesTreeView";
			this.projectResourcesTreeView.Size = new System.Drawing.Size(248, 155);
			this.projectResourcesTreeView.TabIndex = 4;
			this.projectResourcesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ProjectResourcesTreeViewAfterSelect);
			// 
			// projectResourceRadioButton
			// 
			this.projectResourceRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.projectResourceRadioButton.Location = new System.Drawing.Point(6, 108);
			this.projectResourceRadioButton.Name = "projectResourceRadioButton";
			this.projectResourceRadioButton.Size = new System.Drawing.Size(268, 24);
			this.projectResourceRadioButton.TabIndex = 3;
			this.projectResourceRadioButton.TabStop = true;
			this.projectResourceRadioButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ImageResourceEditor.ProjectResour" +
			"ceButton}";
			this.projectResourceRadioButton.UseVisualStyleBackColor = true;
			this.projectResourceRadioButton.CheckedChanged += new System.EventHandler(this.ProjectResourceRadioButtonCheckedChanged);
			// 
			// importLocalResourceButton
			// 
			this.importLocalResourceButton.Location = new System.Drawing.Point(26, 79);
			this.importLocalResourceButton.Name = "importLocalResourceButton";
			this.importLocalResourceButton.Size = new System.Drawing.Size(126, 23);
			this.importLocalResourceButton.TabIndex = 2;
			this.importLocalResourceButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ImageResourceEditor.ImportLocalRe" +
			"sourceButton}";
			this.importLocalResourceButton.UseVisualStyleBackColor = true;
			this.importLocalResourceButton.Click += new System.EventHandler(this.ImportLocalResourceButtonClick);
			// 
			// noResourceRadioButton
			// 
			this.noResourceRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.noResourceRadioButton.Location = new System.Drawing.Point(6, 19);
			this.noResourceRadioButton.Name = "noResourceRadioButton";
			this.noResourceRadioButton.Size = new System.Drawing.Size(268, 24);
			this.noResourceRadioButton.TabIndex = 0;
			this.noResourceRadioButton.TabStop = true;
			this.noResourceRadioButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ImageResourceEditor.NoResourceBut" +
			"ton}";
			this.noResourceRadioButton.UseVisualStyleBackColor = true;
			this.noResourceRadioButton.CheckedChanged += new System.EventHandler(this.NoResourceRadioButtonCheckedChanged);
			// 
			// localResourceRadioButton
			// 
			this.localResourceRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.localResourceRadioButton.Location = new System.Drawing.Point(6, 49);
			this.localResourceRadioButton.Name = "localResourceRadioButton";
			this.localResourceRadioButton.Size = new System.Drawing.Size(268, 24);
			this.localResourceRadioButton.TabIndex = 1;
			this.localResourceRadioButton.TabStop = true;
			this.localResourceRadioButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ImageResourceEditor.LocalResource" +
			"Button}";
			this.localResourceRadioButton.UseVisualStyleBackColor = true;
			this.localResourceRadioButton.CheckedChanged += new System.EventHandler(this.LocalResourceRadioButtonCheckedChanged);
			// 
			// previewGroup
			// 
			previewGroup.Controls.Add(this.previewPictureBox);
			previewGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			previewGroup.Location = new System.Drawing.Point(0, 0);
			previewGroup.Name = "previewGroup";
			previewGroup.Size = new System.Drawing.Size(288, 299);
			previewGroup.TabIndex = 0;
			previewGroup.TabStop = false;
			previewGroup.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ImageResourceEditor.Preview}";
			// 
			// previewPictureBox
			// 
			this.previewPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.previewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.previewPictureBox.Location = new System.Drawing.Point(6, 19);
			this.previewPictureBox.Name = "previewPictureBox";
			this.previewPictureBox.Size = new System.Drawing.Size(276, 274);
			this.previewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.previewPictureBox.TabIndex = 0;
			this.previewPictureBox.TabStop = false;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(428, 317);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "${res:Global.OKButtonText}";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// projectTreeScanningBackgroundWorker
			// 
			this.projectTreeScanningBackgroundWorker.WorkerSupportsCancellation = true;
			this.projectTreeScanningBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ProjectTreeScanningBackgroundWorkerDoWork);
			this.projectTreeScanningBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ProjectTreeScanningBackgroundWorkerRunWorkerCompleted);
			// 
			// ImageResourceEditorDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = cancelButton;
			this.ClientSize = new System.Drawing.Size(596, 352);
			this.Controls.Add(splitContainer);
			this.Controls.Add(cancelButton);
			this.Controls.Add(this.okButton);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImageResourceEditorDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ImageResourceEditor.Title}";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImageResourceEditorDialogFormClosed);
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			splitContainer.ResumeLayout(false);
			resourceSelectionGroup.ResumeLayout(false);
			previewGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
			this.ResumeLayout(false);
		}
		private System.ComponentModel.BackgroundWorker projectTreeScanningBackgroundWorker;
		private System.Windows.Forms.RadioButton noResourceRadioButton;
		private System.Windows.Forms.Button importLocalResourceButton;
		private System.Windows.Forms.RadioButton projectResourceRadioButton;
		private System.Windows.Forms.TreeView projectResourcesTreeView;
		private System.Windows.Forms.RadioButton localResourceRadioButton;
		private System.Windows.Forms.PictureBox previewPictureBox;
		private System.Windows.Forms.Button okButton;
	}
}
