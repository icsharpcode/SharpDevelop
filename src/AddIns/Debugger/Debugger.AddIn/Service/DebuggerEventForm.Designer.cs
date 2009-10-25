// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2858 $</version>
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
	partial class DebuggerEventForm : System.Windows.Forms.Form
	{		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.buttonBreak = new System.Windows.Forms.Button();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.buttonTerminate = new System.Windows.Forms.Button();
			this.buttonContinue = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonBreak
			// 
			this.buttonBreak.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonBreak.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonBreak.Location = new System.Drawing.Point(176, 160);
			this.buttonBreak.MaximumSize = new System.Drawing.Size(91, 32);
			this.buttonBreak.MinimumSize = new System.Drawing.Size(91, 32);
			this.buttonBreak.Name = "buttonBreak";
			this.buttonBreak.Size = new System.Drawing.Size(91, 32);
			this.buttonBreak.TabIndex = 0;
			this.buttonBreak.Text = "${res:MainWindow.Windows.Debug.ExceptionForm.Break}";
			this.buttonBreak.Click += new System.EventHandler(this.buttonBreak_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(14, 16);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(56, 64);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox.TabIndex = 3;
			this.pictureBox.TabStop = false;
			// 
			// buttonTerminate
			// 
			this.buttonTerminate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonTerminate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonTerminate.Location = new System.Drawing.Point(372, 160);
			this.buttonTerminate.MaximumSize = new System.Drawing.Size(91, 32);
			this.buttonTerminate.MinimumSize = new System.Drawing.Size(91, 32);
			this.buttonTerminate.Name = "buttonTerminate";
			this.buttonTerminate.Size = new System.Drawing.Size(91, 32);
			this.buttonTerminate.TabIndex = 2;
			this.buttonTerminate.Text = "${res:MainWindow.Windows.Debug.ExceptionForm.Terminate}";
			this.buttonTerminate.Click += new System.EventHandler(this.buttonTerminate_Click);
			// 
			// buttonContinue
			// 
			this.buttonContinue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonContinue.Location = new System.Drawing.Point(274, 160);
			this.buttonContinue.MaximumSize = new System.Drawing.Size(91, 32);
			this.buttonContinue.MinimumSize = new System.Drawing.Size(91, 32);
			this.buttonContinue.Name = "buttonContinue";
			this.buttonContinue.Size = new System.Drawing.Size(91, 32);
			this.buttonContinue.TabIndex = 1;
			this.buttonContinue.Text = "${res:MainWindow.Windows.Debug.ExceptionForm.Continue}";
			this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(76, 16);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(543, 138);
			this.textBox.TabIndex = 4;
			this.textBox.WordWrap = false;
			// 
			// DebuggerEventForm
			// 
			this.CancelButton = this.buttonBreak;
			this.ClientSize = new System.Drawing.Size(638, 203);
			this.Controls.Add(this.buttonTerminate);
			this.Controls.Add(this.buttonContinue);
			this.Controls.Add(this.buttonBreak);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.pictureBox);
			this.MinimizeBox = false;
			this.Name = "DebuggerEventForm";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.Resize += new System.EventHandler(this.debuggerEventFormResize);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		
		protected System.Windows.Forms.PictureBox pictureBox;
		protected System.Windows.Forms.TextBox textBox;
		protected System.Windows.Forms.Button buttonBreak;
		protected System.Windows.Forms.Button buttonContinue;
		protected System.Windows.Forms.Button buttonTerminate;
	}
}
