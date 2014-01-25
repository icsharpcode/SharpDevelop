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

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	partial class LayoutPanelControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
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
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.radioTableLayout = new System.Windows.Forms.RadioButton();
			this.radioListLayout = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this.radioTableLayout);
			this.groupBox1.Controls.Add(this.radioListLayout);
			this.groupBox1.Location = new System.Drawing.Point(17, 18);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(232, 242);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Report Layout";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboBox1);
			this.groupBox2.Location = new System.Drawing.Point(55, 146);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 64);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "groupBox2";
			this.groupBox2.Visible = false;
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(6, 28);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 21);
			this.comboBox1.TabIndex = 6;
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(64, 106);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(104, 24);
			this.checkBox1.TabIndex = 4;
			this.checkBox1.Text = "Grouping";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
			// 
			// radioTableLayout
			// 
			this.radioTableLayout.Location = new System.Drawing.Point(64, 65);
			this.radioTableLayout.Name = "radioTableLayout";
			this.radioTableLayout.Size = new System.Drawing.Size(104, 24);
			this.radioTableLayout.TabIndex = 3;
			this.radioTableLayout.TabStop = true;
			this.radioTableLayout.Text = "Table Layout";
			this.radioTableLayout.UseVisualStyleBackColor = true;
			// 
			// radioListLayout
			// 
			this.radioListLayout.Location = new System.Drawing.Point(64, 19);
			this.radioListLayout.Name = "radioListLayout";
			this.radioListLayout.Size = new System.Drawing.Size(104, 24);
			this.radioListLayout.TabIndex = 2;
			this.radioListLayout.TabStop = true;
			this.radioListLayout.Text = "List Layout";
			this.radioListLayout.UseVisualStyleBackColor = true;
			// 
			// LayoutPanelControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "LayoutPanelControl";
			this.Size = new System.Drawing.Size(331, 284);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.RadioButton radioTableLayout;
		private System.Windows.Forms.RadioButton radioListLayout;
		private System.Windows.Forms.GroupBox groupBox1;
		
	}
}
