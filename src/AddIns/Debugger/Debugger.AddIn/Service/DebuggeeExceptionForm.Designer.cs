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
using System.Windows.Forms;


namespace ICSharpCode.SharpDevelop.Services
{
	partial class DebuggeeExceptionForm : Form
	{		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.lblExceptionText = new System.Windows.Forms.Label();
			this.exceptionView = new System.Windows.Forms.RichTextBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnBreak = new System.Windows.Forms.Button();
			this.btnContinue = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(4, 12);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(48, 52);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			// 
			// lblExceptionText
			// 
			this.lblExceptionText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblExceptionText.Location = new System.Drawing.Point(58, 12);
			this.lblExceptionText.Name = "lblExceptionText";
			this.lblExceptionText.Size = new System.Drawing.Size(564, 52);
			this.lblExceptionText.TabIndex = 1;
			this.lblExceptionText.Text = "Exception Message Text";
			// 
			// exceptionView
			// 
			this.exceptionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.exceptionView.BackColor = System.Drawing.SystemColors.Control;
			this.exceptionView.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.exceptionView.Location = new System.Drawing.Point(4, 70);
			this.exceptionView.Name = "exceptionView";
			this.exceptionView.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.exceptionView.Size = new System.Drawing.Size(635, 290);
			this.exceptionView.TabIndex = 2;
			this.exceptionView.Text = "";
			// 
			// btnStop
			// 
			this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnStop.Location = new System.Drawing.Point(385, 366);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(115, 30);
			this.btnStop.TabIndex = 3;
			this.btnStop.Text = "Stop";
			this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.BtnStopClick);
			// 
			// btnBreak
			// 
			this.btnBreak.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnBreak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnBreak.Location = new System.Drawing.Point(143, 366);
			this.btnBreak.Name = "btnBreak";
			this.btnBreak.Size = new System.Drawing.Size(115, 30);
			this.btnBreak.TabIndex = 4;
			this.btnBreak.Text = "Break";
			this.btnBreak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnBreak.UseVisualStyleBackColor = true;
			this.btnBreak.Click += new System.EventHandler(this.BtnBreakClick);
			// 
			// btnContinue
			// 
			this.btnContinue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnContinue.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnContinue.Location = new System.Drawing.Point(264, 366);
			this.btnContinue.Name = "btnContinue";
			this.btnContinue.Size = new System.Drawing.Size(115, 30);
			this.btnContinue.TabIndex = 5;
			this.btnContinue.Text = "Continue";
			this.btnContinue.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnContinue.UseVisualStyleBackColor = true;
			this.btnContinue.Click += new System.EventHandler(this.BtnContinueClick);
			// 
			// DebuggeeExceptionForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.ClientSize = new System.Drawing.Size(642, 399);
			this.Controls.Add(this.btnContinue);
			this.Controls.Add(this.btnBreak);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.exceptionView);
			this.Controls.Add(this.lblExceptionText);
			this.Controls.Add(this.pictureBox);
			this.Name = "DebuggeeExceptionForm";
			this.ShowInTaskbar = false;
			this.Resize += new System.EventHandler(this.FormResize);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button btnContinue;
		private System.Windows.Forms.Button btnBreak;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.RichTextBox exceptionView;
		private System.Windows.Forms.Label lblExceptionText;
		private System.Windows.Forms.PictureBox pictureBox;
		#endregion
			
	}
}
