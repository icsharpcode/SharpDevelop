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

namespace ICSharpCode.IconEditor
{
	partial class EditorPanel : System.Windows.Forms.UserControl
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
			this.panel2 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.addFormatButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.colorComboBox = new System.Windows.Forms.ComboBox();
			this.table = new System.Windows.Forms.TableLayoutPanel();
			this.tableLabel = new System.Windows.Forms.Label();
			this.panel2.SuspendLayout();
			this.table.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.addFormatButton);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.colorComboBox);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(422, 50);
			this.panel2.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(3, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(416, 18);
			this.label2.TabIndex = 3;
			this.label2.Text = "Right-click on the individual images to see more options.";
			// 
			// addFormatButton
			// 
			this.addFormatButton.Location = new System.Drawing.Point(171, 3);
			this.addFormatButton.Name = "addFormatButton";
			this.addFormatButton.Size = new System.Drawing.Size(131, 23);
			this.addFormatButton.TabIndex = 2;
			this.addFormatButton.Text = "Add Custom Format...";
			this.addFormatButton.UseVisualStyleBackColor = true;
			this.addFormatButton.Click += new System.EventHandler(this.AddFormatButtonClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "View on color:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// colorComboBox
			// 
			this.colorComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.colorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.colorComboBox.FormattingEnabled = true;
			this.colorComboBox.Location = new System.Drawing.Point(108, 3);
			this.colorComboBox.Name = "colorComboBox";
			this.colorComboBox.Size = new System.Drawing.Size(46, 21);
			this.colorComboBox.TabIndex = 0;
			this.colorComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ColorComboBoxDrawItem);
			this.colorComboBox.SelectedIndexChanged += new System.EventHandler(this.ColorComboBoxSelectedIndexChanged);
			// 
			// table
			// 
			this.table.AutoScroll = true;
			this.table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.table.ColumnCount = 1;
			this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.table.Controls.Add(this.tableLabel, 0, 0);
			this.table.Dock = System.Windows.Forms.DockStyle.Fill;
			this.table.Location = new System.Drawing.Point(0, 50);
			this.table.Name = "table";
			this.table.RowCount = 1;
			this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.table.Size = new System.Drawing.Size(422, 212);
			this.table.TabIndex = 5;
			// 
			// tableLabel
			// 
			this.tableLabel.Location = new System.Drawing.Point(3, 0);
			this.tableLabel.Name = "tableLabel";
			this.tableLabel.Size = new System.Drawing.Size(68, 23);
			this.tableLabel.TabIndex = 0;
			this.tableLabel.Text = "Icon Editor";
			this.tableLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// EditorPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this.table);
			this.Controls.Add(this.panel2);
			this.Name = "EditorPanel";
			this.Size = new System.Drawing.Size(422, 262);
			this.panel2.ResumeLayout(false);
			this.table.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button addFormatButton;
		private System.Windows.Forms.Label tableLabel;
		private System.Windows.Forms.TableLayoutPanel table;
		private System.Windows.Forms.ComboBox colorComboBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel2;
	}
}
