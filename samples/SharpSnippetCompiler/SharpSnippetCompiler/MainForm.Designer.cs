// SharpDevelop samples
// Copyright (c) 2008, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

namespace ICSharpCode.SharpSnippetCompiler
{
	partial class MainForm
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
			this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fileNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fileOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fileCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fileExitToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoSeparatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllSeparatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.continueSeparatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.continueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stepSeparatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.stepOverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stepIntoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stepOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.referencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.fileTabControl = new System.Windows.Forms.TabControl();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.errorsTabPage = new System.Windows.Forms.TabPage();
			this.outputTabPage = new System.Windows.Forms.TabPage();
			this.mainMenuStrip.SuspendLayout();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenuStrip
			// 
			this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileToolStripMenuItem,
									this.editToolStripMenuItem,
									this.buildtoolStripMenuItem,
									this.debugToolStripMenuItem,
									this.toolsToolStripMenuItem});
			this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.mainMenuStrip.Name = "mainMenuStrip";
			this.mainMenuStrip.Size = new System.Drawing.Size(757, 24);
			this.mainMenuStrip.TabIndex = 0;
			this.mainMenuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileNewToolStripMenuItem,
									this.fileOpenToolStripMenuItem,
									this.fileCloseToolStripMenuItem,
									this.fileExitToolStripSeparator,
									this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// fileNewToolStripMenuItem
			// 
			this.fileNewToolStripMenuItem.Name = "fileNewToolStripMenuItem";
			this.fileNewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.fileNewToolStripMenuItem.Text = "&New...";
			this.fileNewToolStripMenuItem.Click += new System.EventHandler(this.FileNewToolStripMenuItemClick);
			// 
			// fileOpenToolStripMenuItem
			// 
			this.fileOpenToolStripMenuItem.Name = "fileOpenToolStripMenuItem";
			this.fileOpenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.fileOpenToolStripMenuItem.Text = "&Open...";
			this.fileOpenToolStripMenuItem.Click += new System.EventHandler(this.FileOpenToolStripMenuItemClick);
			// 
			// fileCloseToolStripMenuItem
			// 
			this.fileCloseToolStripMenuItem.Name = "fileCloseToolStripMenuItem";
			this.fileCloseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.fileCloseToolStripMenuItem.Text = "&Close";
			this.fileCloseToolStripMenuItem.Click += new System.EventHandler(this.FileCloseToolStripMenuItemClick);
			// 
			// fileExitToolStripSeparator
			// 
			this.fileExitToolStripSeparator.Name = "fileExitToolStripSeparator";
			this.fileExitToolStripSeparator.Size = new System.Drawing.Size(149, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.undoToolStripMenuItem,
									this.redoToolStripMenuItem,
									this.redoSeparatorToolStripMenuItem,
									this.cutToolStripMenuItem,
									this.copyToolStripMenuItem,
									this.pasteToolStripMenuItem,
									this.deleteToolStripMenuItem,
									this.selectAllSeparatorToolStripMenuItem,
									this.selectAllToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.undoToolStripMenuItem.Text = "&Undo";
			this.undoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItemClick);
			// 
			// redoToolStripMenuItem
			// 
			this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.redoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.redoToolStripMenuItem.Text = "&Redo";
			this.redoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItemClick);
			// 
			// redoSeparatorToolStripMenuItem
			// 
			this.redoSeparatorToolStripMenuItem.Name = "redoSeparatorToolStripMenuItem";
			this.redoSeparatorToolStripMenuItem.Size = new System.Drawing.Size(161, 6);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.cutToolStripMenuItem.Text = "Cu&t";
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItemClick);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItemClick);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItemClick);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.deleteToolStripMenuItem.Text = "&Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItemClick);
			// 
			// selectAllSeparatorToolStripMenuItem
			// 
			this.selectAllSeparatorToolStripMenuItem.Name = "selectAllSeparatorToolStripMenuItem";
			this.selectAllSeparatorToolStripMenuItem.Size = new System.Drawing.Size(161, 6);
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.selectAllToolStripMenuItem.Text = "Select &All";
			this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItemClick);
			// 
			// buildtoolStripMenuItem
			// 
			this.buildtoolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.buildCurrentToolStripMenuItem});
			this.buildtoolStripMenuItem.Name = "buildtoolStripMenuItem";
			this.buildtoolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.buildtoolStripMenuItem.Text = "&Build";
			// 
			// buildCurrentToolStripMenuItem
			// 
			this.buildCurrentToolStripMenuItem.Name = "buildCurrentToolStripMenuItem";
			this.buildCurrentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.B)));
			this.buildCurrentToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.buildCurrentToolStripMenuItem.Text = "&Build Current";
			this.buildCurrentToolStripMenuItem.Click += new System.EventHandler(this.BuildCurrentToolStripMenuItemClick);
			// 
			// debugToolStripMenuItem
			// 
			this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.runToolStripMenuItem,
									this.stopToolStripMenuItem,
									this.continueSeparatorToolStripMenuItem,
									this.continueToolStripMenuItem,
									this.stepSeparatorToolStripMenuItem,
									this.stepOverToolStripMenuItem,
									this.stepIntoToolStripMenuItem,
									this.stepOutToolStripMenuItem});
			this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
			this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.debugToolStripMenuItem.Text = "&Debug";
			// 
			// runToolStripMenuItem
			// 
			this.runToolStripMenuItem.Name = "runToolStripMenuItem";
			this.runToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.runToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			this.runToolStripMenuItem.Text = "&Run";
			this.runToolStripMenuItem.Click += new System.EventHandler(this.RunToolStripMenuItemClick);
			// 
			// stopToolStripMenuItem
			// 
			this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			this.stopToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			this.stopToolStripMenuItem.Text = "St&op";
			this.stopToolStripMenuItem.Click += new System.EventHandler(this.StopToolStripMenuItemClick);
			// 
			// continueSeparatorToolStripMenuItem
			// 
			this.continueSeparatorToolStripMenuItem.Name = "continueSeparatorToolStripMenuItem";
			this.continueSeparatorToolStripMenuItem.Size = new System.Drawing.Size(174, 6);
			// 
			// continueToolStripMenuItem
			// 
			this.continueToolStripMenuItem.Name = "continueToolStripMenuItem";
			this.continueToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
			this.continueToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			this.continueToolStripMenuItem.Text = "&Continue";
			this.continueToolStripMenuItem.Click += new System.EventHandler(this.ContinueToolStripMenuItemClick);
			// 
			// stepSeparatorToolStripMenuItem
			// 
			this.stepSeparatorToolStripMenuItem.Name = "stepSeparatorToolStripMenuItem";
			this.stepSeparatorToolStripMenuItem.Size = new System.Drawing.Size(174, 6);
			// 
			// stepOverToolStripMenuItem
			// 
			this.stepOverToolStripMenuItem.Name = "stepOverToolStripMenuItem";
			this.stepOverToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
			this.stepOverToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			this.stepOverToolStripMenuItem.Text = "Step O&ver";
			this.stepOverToolStripMenuItem.Click += new System.EventHandler(this.StepOverToolStripMenuItemClick);
			// 
			// stepIntoToolStripMenuItem
			// 
			this.stepIntoToolStripMenuItem.Name = "stepIntoToolStripMenuItem";
			this.stepIntoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
			this.stepIntoToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			this.stepIntoToolStripMenuItem.Text = "Step &Into";
			this.stepIntoToolStripMenuItem.Click += new System.EventHandler(this.StepIntoToolStripMenuItemClick);
			// 
			// stepOutToolStripMenuItem
			// 
			this.stepOutToolStripMenuItem.Name = "stepOutToolStripMenuItem";
			this.stepOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F11)));
			this.stepOutToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			this.stepOutToolStripMenuItem.Text = "Step O&ut";
			this.stepOutToolStripMenuItem.Click += new System.EventHandler(this.StepOutToolStripMenuItemClick);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.referencesToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.toolsToolStripMenuItem.Text = "&Tools";
			// 
			// referencesToolStripMenuItem
			// 
			this.referencesToolStripMenuItem.Name = "referencesToolStripMenuItem";
			this.referencesToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.referencesToolStripMenuItem.Text = "&References...";
			this.referencesToolStripMenuItem.Click += new System.EventHandler(this.ReferencesToolStripMenuItemClick);
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 24);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.fileTabControl);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.tabControl);
			this.splitContainer.Size = new System.Drawing.Size(757, 349);
			this.splitContainer.SplitterDistance = 225;
			this.splitContainer.TabIndex = 1;
			// 
			// fileTabControl
			// 
			this.fileTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fileTabControl.Location = new System.Drawing.Point(0, 0);
			this.fileTabControl.Name = "fileTabControl";
			this.fileTabControl.SelectedIndex = 0;
			this.fileTabControl.Size = new System.Drawing.Size(757, 225);
			this.fileTabControl.TabIndex = 0;
			this.fileTabControl.SelectedIndexChanged += new System.EventHandler(this.FileTabControlSelectedIndexChanged);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.errorsTabPage);
			this.tabControl.Controls.Add(this.outputTabPage);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(757, 120);
			this.tabControl.TabIndex = 0;
			// 
			// errorsTabPage
			// 
			this.errorsTabPage.Location = new System.Drawing.Point(4, 22);
			this.errorsTabPage.Name = "errorsTabPage";
			this.errorsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.errorsTabPage.Size = new System.Drawing.Size(749, 94);
			this.errorsTabPage.TabIndex = 0;
			this.errorsTabPage.Text = "Errors";
			this.errorsTabPage.UseVisualStyleBackColor = true;
			// 
			// outputTabPage
			// 
			this.outputTabPage.Location = new System.Drawing.Point(4, 22);
			this.outputTabPage.Name = "outputTabPage";
			this.outputTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.outputTabPage.Size = new System.Drawing.Size(749, 94);
			this.outputTabPage.TabIndex = 1;
			this.outputTabPage.Text = "Output";
			this.outputTabPage.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(757, 373);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.mainMenuStrip);
			this.MainMenuStrip = this.mainMenuStrip;
			this.Name = "MainForm";
			this.Text = "SharpSnippetCompiler";
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripSeparator fileExitToolStripSeparator;
		private System.Windows.Forms.ToolStripMenuItem fileCloseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileOpenToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileNewToolStripMenuItem;
		private System.Windows.Forms.TabControl fileTabControl;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator redoSeparatorToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator selectAllSeparatorToolStripMenuItem;
		private System.Windows.Forms.TabPage outputTabPage;
		private System.Windows.Forms.TabPage errorsTabPage;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.ToolStripMenuItem referencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stepOutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stepIntoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stepOverToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator stepSeparatorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem continueToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator continueSeparatorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildCurrentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildtoolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip mainMenuStrip;
	}
}
