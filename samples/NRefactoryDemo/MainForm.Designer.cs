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

namespace NRefactoryDemo
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.codeTextBox = new System.Windows.Forms.TextBox();
			this.astPanel = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.transformationComboBox = new System.Windows.Forms.ComboBox();
			this.applyTransformation = new System.Windows.Forms.Button();
			this.editNodeButton = new System.Windows.Forms.Button();
			this.deleteSelectedNode = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.arrowUpPictureBox = new System.Windows.Forms.PictureBox();
			this.arrowDownPictureBox = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.clearSpecialsButton = new System.Windows.Forms.Button();
			this.specialsLabel = new System.Windows.Forms.Label();
			this.generateVBButton = new System.Windows.Forms.Button();
			this.parseVBButton = new System.Windows.Forms.Button();
			this.generateCSharpButton = new System.Windows.Forms.Button();
			this.parseCSharpButton = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.arrowUpPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.arrowDownPictureBox)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.codeTextBox);
			this.splitContainer1.Panel1MinSize = 50;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.astPanel);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel2.Controls.Add(this.panel1);
			this.splitContainer1.Panel2MinSize = 150;
			this.splitContainer1.Size = new System.Drawing.Size(512, 411);
			this.splitContainer1.SplitterDistance = 146;
			this.splitContainer1.TabIndex = 0;
			// 
			// codeTextBox
			// 
			this.codeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeTextBox.Location = new System.Drawing.Point(0, 0);
			this.codeTextBox.Multiline = true;
			this.codeTextBox.Name = "codeTextBox";
			this.codeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.codeTextBox.Size = new System.Drawing.Size(512, 146);
			this.codeTextBox.TabIndex = 0;
			this.codeTextBox.Text = "using System;\r\nclass MainClass\r\n{\r\n  // This is the entry method of the applicati" +
			"on\r\n  public static void Main()\r\n  {\r\n    Console.WriteLine(\"Hello, World!\");\r\n " +
			" }\r\n}";
			// 
			// astPanel
			// 
			this.astPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.astPanel.Location = new System.Drawing.Point(128, 60);
			this.astPanel.Name = "astPanel";
			this.astPanel.Size = new System.Drawing.Size(384, 201);
			this.astPanel.TabIndex = 2;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.transformationComboBox);
			this.groupBox2.Controls.Add(this.applyTransformation);
			this.groupBox2.Controls.Add(this.editNodeButton);
			this.groupBox2.Controls.Add(this.deleteSelectedNode);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBox2.Location = new System.Drawing.Point(0, 60);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(128, 201);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Apply transformations";
			// 
			// transformationComboBox
			// 
			this.transformationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.transformationComboBox.DropDownWidth = 200;
			this.transformationComboBox.FormattingEnabled = true;
			this.transformationComboBox.Items.AddRange(new object[] {
									"CSharpConstructsVisitor",
									"ToVBNetConvertVisitor",
									"VBNetConstructsConvertVisitor",
									"ToCSharpConvertVisitor"});
			this.transformationComboBox.Location = new System.Drawing.Point(9, 78);
			this.transformationComboBox.Name = "transformationComboBox";
			this.transformationComboBox.Size = new System.Drawing.Size(112, 21);
			this.transformationComboBox.TabIndex = 2;
			// 
			// applyTransformation
			// 
			this.applyTransformation.Location = new System.Drawing.Point(13, 105);
			this.applyTransformation.Name = "applyTransformation";
			this.applyTransformation.Size = new System.Drawing.Size(96, 23);
			this.applyTransformation.TabIndex = 3;
			this.applyTransformation.Text = "Run visitor";
			this.applyTransformation.UseVisualStyleBackColor = true;
			this.applyTransformation.Click += new System.EventHandler(this.ApplyTransformationClick);
			// 
			// editNodeButton
			// 
			this.editNodeButton.Location = new System.Drawing.Point(9, 49);
			this.editNodeButton.Name = "editNodeButton";
			this.editNodeButton.Size = new System.Drawing.Size(110, 23);
			this.editNodeButton.TabIndex = 1;
			this.editNodeButton.Text = "Edit node";
			this.editNodeButton.UseVisualStyleBackColor = true;
			this.editNodeButton.Click += new System.EventHandler(this.EditNodeButtonClick);
			// 
			// deleteSelectedNode
			// 
			this.deleteSelectedNode.Location = new System.Drawing.Point(9, 20);
			this.deleteSelectedNode.Name = "deleteSelectedNode";
			this.deleteSelectedNode.Size = new System.Drawing.Size(110, 23);
			this.deleteSelectedNode.TabIndex = 0;
			this.deleteSelectedNode.Text = "Delete node";
			this.deleteSelectedNode.UseVisualStyleBackColor = true;
			this.deleteSelectedNode.Click += new System.EventHandler(this.DeleteSelectedNodeClick);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.arrowUpPictureBox);
			this.panel1.Controls.Add(this.arrowDownPictureBox);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.generateVBButton);
			this.panel1.Controls.Add(this.parseVBButton);
			this.panel1.Controls.Add(this.generateCSharpButton);
			this.panel1.Controls.Add(this.parseCSharpButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(512, 60);
			this.panel1.TabIndex = 0;
			// 
			// arrowUpPictureBox
			// 
			this.arrowUpPictureBox.Location = new System.Drawing.Point(404, 3);
			this.arrowUpPictureBox.Name = "arrowUpPictureBox";
			this.arrowUpPictureBox.Size = new System.Drawing.Size(29, 54);
			this.arrowUpPictureBox.TabIndex = 2;
			this.arrowUpPictureBox.TabStop = false;
			this.arrowUpPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ArrowUpPictureBoxPaint);
			// 
			// arrowDownPictureBox
			// 
			this.arrowDownPictureBox.Location = new System.Drawing.Point(123, 3);
			this.arrowDownPictureBox.Name = "arrowDownPictureBox";
			this.arrowDownPictureBox.Size = new System.Drawing.Size(29, 54);
			this.arrowDownPictureBox.TabIndex = 2;
			this.arrowDownPictureBox.TabStop = false;
			this.arrowDownPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ArrowDownPictureBoxPaint);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.clearSpecialsButton);
			this.groupBox1.Controls.Add(this.specialsLabel);
			this.groupBox1.Location = new System.Drawing.Point(3, -2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(114, 59);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			// 
			// clearSpecialsButton
			// 
			this.clearSpecialsButton.Location = new System.Drawing.Point(31, 30);
			this.clearSpecialsButton.Name = "clearSpecialsButton";
			this.clearSpecialsButton.Size = new System.Drawing.Size(75, 23);
			this.clearSpecialsButton.TabIndex = 1;
			this.clearSpecialsButton.Text = "Clear";
			this.clearSpecialsButton.UseVisualStyleBackColor = true;
			this.clearSpecialsButton.Click += new System.EventHandler(this.ClearSpecialsButtonClick);
			// 
			// specialsLabel
			// 
			this.specialsLabel.Location = new System.Drawing.Point(6, 16);
			this.specialsLabel.Name = "specialsLabel";
			this.specialsLabel.Size = new System.Drawing.Size(100, 23);
			this.specialsLabel.TabIndex = 0;
			this.specialsLabel.Text = "# specials saved";
			// 
			// generateVBButton
			// 
			this.generateVBButton.Location = new System.Drawing.Point(281, 31);
			this.generateVBButton.Name = "generateVBButton";
			this.generateVBButton.Size = new System.Drawing.Size(117, 23);
			this.generateVBButton.TabIndex = 3;
			this.generateVBButton.Text = "Generate VB code";
			this.generateVBButton.UseVisualStyleBackColor = true;
			this.generateVBButton.Click += new System.EventHandler(this.GenerateVBButtonClick);
			// 
			// parseVBButton
			// 
			this.parseVBButton.Location = new System.Drawing.Point(158, 31);
			this.parseVBButton.Name = "parseVBButton";
			this.parseVBButton.Size = new System.Drawing.Size(117, 23);
			this.parseVBButton.TabIndex = 1;
			this.parseVBButton.Text = "Parse VB code";
			this.parseVBButton.UseVisualStyleBackColor = true;
			this.parseVBButton.Click += new System.EventHandler(this.ParseVBButtonClick);
			// 
			// generateCSharpButton
			// 
			this.generateCSharpButton.Location = new System.Drawing.Point(281, 6);
			this.generateCSharpButton.Name = "generateCSharpButton";
			this.generateCSharpButton.Size = new System.Drawing.Size(117, 23);
			this.generateCSharpButton.TabIndex = 2;
			this.generateCSharpButton.Text = "Generate C# code";
			this.generateCSharpButton.UseVisualStyleBackColor = true;
			this.generateCSharpButton.Click += new System.EventHandler(this.GenerateCSharpButtonClick);
			// 
			// parseCSharpButton
			// 
			this.parseCSharpButton.Location = new System.Drawing.Point(158, 6);
			this.parseCSharpButton.Name = "parseCSharpButton";
			this.parseCSharpButton.Size = new System.Drawing.Size(117, 23);
			this.parseCSharpButton.TabIndex = 0;
			this.parseCSharpButton.Text = "Parse C# code";
			this.parseCSharpButton.UseVisualStyleBackColor = true;
			this.parseCSharpButton.Click += new System.EventHandler(this.ParseCSharpButtonClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(512, 411);
			this.Controls.Add(this.splitContainer1);
			this.Name = "MainForm";
			this.Text = "NRefactoryDemo";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.arrowUpPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.arrowDownPictureBox)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button editNodeButton;
		private System.Windows.Forms.ComboBox transformationComboBox;
		private System.Windows.Forms.Button applyTransformation;
		private System.Windows.Forms.Button deleteSelectedNode;
		private System.Windows.Forms.Panel astPanel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button parseCSharpButton;
		private System.Windows.Forms.Button generateCSharpButton;
		private System.Windows.Forms.Button parseVBButton;
		private System.Windows.Forms.Button generateVBButton;
		private System.Windows.Forms.PictureBox arrowUpPictureBox;
		private System.Windows.Forms.PictureBox arrowDownPictureBox;
		private System.Windows.Forms.Label specialsLabel;
		private System.Windows.Forms.Button clearSpecialsButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox codeTextBox;
		private System.Windows.Forms.SplitContainer splitContainer1;
	}
}
