// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
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

namespace SdaUser
{
	partial class MainForm : System.Windows.Forms.Form
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
			this.runButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.makeTransparentButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.visibleCheckBox = new System.Windows.Forms.CheckBox();
			this.unloadHostDomainButton = new System.Windows.Forms.Button();
			this.openFileButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// runButton
			// 
			this.runButton.Location = new System.Drawing.Point(28, 57);
			this.runButton.Name = "runButton";
			this.runButton.Size = new System.Drawing.Size(221, 31);
			this.runButton.TabIndex = 0;
			this.runButton.Text = "Run Integrated SharpDevelop";
			this.runButton.UseVisualStyleBackColor = true;
			this.runButton.Click += new System.EventHandler(this.RunButtonClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.makeTransparentButton);
			this.groupBox1.Controls.Add(this.closeButton);
			this.groupBox1.Controls.Add(this.visibleCheckBox);
			this.groupBox1.Enabled = false;
			this.groupBox1.Location = new System.Drawing.Point(12, 123);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(268, 82);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
			// 
			// makeTransparentButton
			// 
			this.makeTransparentButton.Location = new System.Drawing.Point(135, 50);
			this.makeTransparentButton.Name = "makeTransparentButton";
			this.makeTransparentButton.Size = new System.Drawing.Size(112, 23);
			this.makeTransparentButton.TabIndex = 2;
			this.makeTransparentButton.Text = "Make Transparent";
			this.makeTransparentButton.UseVisualStyleBackColor = true;
			this.makeTransparentButton.Click += new System.EventHandler(this.MakeTransparentButtonClick);
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(6, 50);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(123, 23);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Close Workbench";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
			// 
			// visibleCheckBox
			// 
			this.visibleCheckBox.Checked = true;
			this.visibleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.visibleCheckBox.Location = new System.Drawing.Point(6, 20);
			this.visibleCheckBox.Name = "visibleCheckBox";
			this.visibleCheckBox.Size = new System.Drawing.Size(256, 24);
			this.visibleCheckBox.TabIndex = 0;
			this.visibleCheckBox.Text = "WorkbenchVisible";
			this.visibleCheckBox.UseVisualStyleBackColor = true;
			this.visibleCheckBox.CheckedChanged += new System.EventHandler(this.VisibleCheckBoxCheckedChanged);
			// 
			// unloadHostDomainButton
			// 
			this.unloadHostDomainButton.Enabled = false;
			this.unloadHostDomainButton.Location = new System.Drawing.Point(28, 94);
			this.unloadHostDomainButton.Name = "unloadHostDomainButton";
			this.unloadHostDomainButton.Size = new System.Drawing.Size(221, 23);
			this.unloadHostDomainButton.TabIndex = 2;
			this.unloadHostDomainButton.Text = "Unload SharpDevelop AppDomain";
			this.unloadHostDomainButton.UseVisualStyleBackColor = true;
			this.unloadHostDomainButton.Click += new System.EventHandler(this.UnloadHostDomainButtonClick);
			// 
			// openFileButton
			// 
			this.openFileButton.Location = new System.Drawing.Point(18, 223);
			this.openFileButton.Name = "openFileButton";
			this.openFileButton.Size = new System.Drawing.Size(75, 23);
			this.openFileButton.TabIndex = 3;
			this.openFileButton.Text = "Open File";
			this.openFileButton.UseVisualStyleBackColor = true;
			this.openFileButton.Click += new System.EventHandler(this.OpenFileButtonClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 255);
			this.Controls.Add(this.openFileButton);
			this.Controls.Add(this.unloadHostDomainButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.runButton);
			this.Name = "MainForm";
			this.Text = "SdaUser";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button makeTransparentButton;
		private System.Windows.Forms.Button openFileButton;
		private System.Windows.Forms.Button unloadHostDomainButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.CheckBox visibleCheckBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button runButton;
	}
}
