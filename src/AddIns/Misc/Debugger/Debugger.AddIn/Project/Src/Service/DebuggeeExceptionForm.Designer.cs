// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
#region License
//  
//  Copyright (c) 2007, ic#code
//  
//  All rights reserved.
//  
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//  
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//  
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//  
#endregion

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
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(12, 12);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(48, 52);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// lblExceptionText
			// 
			this.lblExceptionText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblExceptionText.Location = new System.Drawing.Point(66, 12);
			this.lblExceptionText.Name = "lblExceptionText";
			this.lblExceptionText.Size = new System.Drawing.Size(560, 52);
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
			this.exceptionView.Location = new System.Drawing.Point(12, 70);
			this.exceptionView.Name = "exceptionView";
			this.exceptionView.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.exceptionView.Size = new System.Drawing.Size(614, 317);
			this.exceptionView.TabIndex = 2;
			this.exceptionView.Text = "";
			// 
			// DebuggeeExceptionForm
			// 
			this.ClientSize = new System.Drawing.Size(638, 399);
			this.Controls.Add(this.exceptionView);
			this.Controls.Add(this.lblExceptionText);
			this.Controls.Add(this.pictureBox);
			this.Name = "DebuggeeExceptionForm";
			this.Resize += new System.EventHandler(this.debugeeExceptionFormResize);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.RichTextBox exceptionView;
		private System.Windows.Forms.Label lblExceptionText;
		private System.Windows.Forms.PictureBox pictureBox;
		#endregion
			
	}
}
