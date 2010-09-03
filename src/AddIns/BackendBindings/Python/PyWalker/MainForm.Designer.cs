// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace PyWalker
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
			this.components = new System.ComponentModel.Container();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.codeTextBox = new System.Windows.Forms.RichTextBox();
			this.runCSharpNRefactoryVisitor = new System.Windows.Forms.Button();
			this.runNRefactoryCSharpCodeDomVisitor = new System.Windows.Forms.Button();
			this.runCSharpToPythonButton = new System.Windows.Forms.Button();
			this.runRoundTripButton = new System.Windows.Forms.Button();
			this.clearButton = new System.Windows.Forms.Button();
			this.runAstWalkerButton = new System.Windows.Forms.Button();
			this.walkerOutputTextBox = new System.Windows.Forms.RichTextBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.codeTextBox);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.runCSharpNRefactoryVisitor);
			this.splitContainer.Panel2.Controls.Add(this.runNRefactoryCSharpCodeDomVisitor);
			this.splitContainer.Panel2.Controls.Add(this.runCSharpToPythonButton);
			this.splitContainer.Panel2.Controls.Add(this.runRoundTripButton);
			this.splitContainer.Panel2.Controls.Add(this.clearButton);
			this.splitContainer.Panel2.Controls.Add(this.runAstWalkerButton);
			this.splitContainer.Panel2.Controls.Add(this.walkerOutputTextBox);
			this.splitContainer.Size = new System.Drawing.Size(515, 386);
			this.splitContainer.SplitterDistance = 138;
			this.splitContainer.TabIndex = 0;
			// 
			// codeTextBox
			// 
			this.codeTextBox.AcceptsTab = true;
			this.codeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeTextBox.Location = new System.Drawing.Point(0, 0);
			this.codeTextBox.Name = "codeTextBox";
			this.codeTextBox.Size = new System.Drawing.Size(515, 138);
			this.codeTextBox.TabIndex = 0;
			this.codeTextBox.Text = "";
			this.codeTextBox.WordWrap = false;
			// 
			// runCSharpNRefactoryVisitor
			// 
			this.runCSharpNRefactoryVisitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.runCSharpNRefactoryVisitor.Location = new System.Drawing.Point(261, 218);
			this.runCSharpNRefactoryVisitor.Name = "runCSharpNRefactoryVisitor";
			this.runCSharpNRefactoryVisitor.Size = new System.Drawing.Size(117, 23);
			this.runCSharpNRefactoryVisitor.TabIndex = 8;
			this.runCSharpNRefactoryVisitor.Text = "Visit C# AST";
			this.toolTip.SetToolTip(this.runCSharpNRefactoryVisitor, "Walks the NRefactory AST generated from the C# code.");
			this.runCSharpNRefactoryVisitor.UseVisualStyleBackColor = true;
			this.runCSharpNRefactoryVisitor.Click += new System.EventHandler(this.RunCSharpNRefactoryVisitorClick);
			// 
			// runNRefactoryCSharpCodeDomVisitor
			// 
			this.runNRefactoryCSharpCodeDomVisitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.runNRefactoryCSharpCodeDomVisitor.Location = new System.Drawing.Point(384, 218);
			this.runNRefactoryCSharpCodeDomVisitor.Name = "runNRefactoryCSharpCodeDomVisitor";
			this.runNRefactoryCSharpCodeDomVisitor.Size = new System.Drawing.Size(127, 23);
			this.runNRefactoryCSharpCodeDomVisitor.TabIndex = 7;
			this.runNRefactoryCSharpCodeDomVisitor.Text = "Visit C# Code DOM";
			this.toolTip.SetToolTip(this.runNRefactoryCSharpCodeDomVisitor, "Visits the code dom generated from the C# code by the NRefactory code dom visitor" +
						".");
			this.runNRefactoryCSharpCodeDomVisitor.UseVisualStyleBackColor = true;
			this.runNRefactoryCSharpCodeDomVisitor.Click += new System.EventHandler(this.RunNRefactoryCSharpCodeDomVisitorClick);
			// 
			// runCSharpToPythonButton
			// 
			this.runCSharpToPythonButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.runCSharpToPythonButton.Location = new System.Drawing.Point(261, 192);
			this.runCSharpToPythonButton.Name = "runCSharpToPythonButton";
			this.runCSharpToPythonButton.Size = new System.Drawing.Size(117, 23);
			this.runCSharpToPythonButton.TabIndex = 6;
			this.runCSharpToPythonButton.Text = "C# to Python";
			this.toolTip.SetToolTip(this.runCSharpToPythonButton, "Takes the code dom generated from the NRefactory parser and converts it to python" +
						" using the python generator.");
			this.runCSharpToPythonButton.UseVisualStyleBackColor = true;
			this.runCSharpToPythonButton.Click += new System.EventHandler(this.RunCSharpToPythonClick);
			// 
			// runRoundTripButton
			// 
			this.runRoundTripButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.runRoundTripButton.Location = new System.Drawing.Point(138, 191);
			this.runRoundTripButton.Name = "runRoundTripButton";
			this.runRoundTripButton.Size = new System.Drawing.Size(117, 23);
			this.runRoundTripButton.TabIndex = 4;
			this.runRoundTripButton.Text = "Round Trip";
			this.toolTip.SetToolTip(this.runRoundTripButton, "Generates a code dom from the python code and then generates python code from the" +
						" code dom.");
			this.runRoundTripButton.UseVisualStyleBackColor = true;
			this.runRoundTripButton.Click += new System.EventHandler(this.RunRoundTripButtonClick);
			// 
			// clearButton
			// 
			this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.clearButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.clearButton.Location = new System.Drawing.Point(138, 218);
			this.clearButton.Name = "clearButton";
			this.clearButton.Size = new System.Drawing.Size(117, 23);
			this.clearButton.TabIndex = 2;
			this.clearButton.Text = "Clear";
			this.clearButton.UseVisualStyleBackColor = true;
			this.clearButton.Click += new System.EventHandler(this.ClearButtonClick);
			// 
			// runAstWalkerButton
			// 
			this.runAstWalkerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.runAstWalkerButton.Location = new System.Drawing.Point(384, 191);
			this.runAstWalkerButton.Name = "runAstWalkerButton";
			this.runAstWalkerButton.Size = new System.Drawing.Size(127, 23);
			this.runAstWalkerButton.TabIndex = 1;
			this.runAstWalkerButton.Text = "Visit AST";
			this.toolTip.SetToolTip(this.runAstWalkerButton, "Walks the python AST generated from the python code.");
			this.runAstWalkerButton.UseVisualStyleBackColor = true;
			this.runAstWalkerButton.Click += new System.EventHandler(this.RunAstWalkerButtonClick);
			// 
			// walkerOutputTextBox
			// 
			this.walkerOutputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.walkerOutputTextBox.Location = new System.Drawing.Point(0, 2);
			this.walkerOutputTextBox.Name = "walkerOutputTextBox";
			this.walkerOutputTextBox.Size = new System.Drawing.Size(515, 184);
			this.walkerOutputTextBox.TabIndex = 0;
			this.walkerOutputTextBox.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(515, 386);
			this.Controls.Add(this.splitContainer);
			this.Name = "MainForm";
			this.Text = "PyWalker";
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button runCSharpNRefactoryVisitor;
		private System.Windows.Forms.Button runNRefactoryCSharpCodeDomVisitor;
		private System.Windows.Forms.Button runCSharpToPythonButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button runRoundTripButton;
		private System.Windows.Forms.Button clearButton;
		private System.Windows.Forms.Button runAstWalkerButton;
		private System.Windows.Forms.RichTextBox walkerOutputTextBox;
		private System.Windows.Forms.RichTextBox codeTextBox;
		private System.Windows.Forms.SplitContainer splitContainer;
	}
}
