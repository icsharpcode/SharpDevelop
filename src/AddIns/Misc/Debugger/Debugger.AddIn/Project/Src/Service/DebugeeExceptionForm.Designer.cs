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
	partial class DebugeeExceptionForm : DebuggerEventForm
	{		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.localVarList = new Aga.Controls.Tree.TreeViewAdv();
			this.linkExceptionDetail = new System.Windows.Forms.LinkLabel();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Size = new System.Drawing.Size(542, 139);
			// 
			// buttonBreak
			// 
			this.buttonBreak.Location = new System.Drawing.Point(176, 356);
			// 
			// buttonContinue
			// 
			this.buttonContinue.Location = new System.Drawing.Point(274, 356);
			// 
			// buttonTerminate
			// 
			this.buttonTerminate.Location = new System.Drawing.Point(372, 356);
			// 
			// localVarList
			// 
			this.localVarList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.localVarList.AutoRowHeight = true;
			this.localVarList.BackColor = System.Drawing.SystemColors.Window;
			this.localVarList.DefaultToolTipProvider = null;
			this.localVarList.DragDropMarkColor = System.Drawing.Color.Black;
			this.localVarList.LineColor = System.Drawing.SystemColors.ControlDark;
			this.localVarList.LoadOnDemand = true;
			this.localVarList.Location = new System.Drawing.Point(3, 4);
			this.localVarList.Model = null;
			this.localVarList.Name = "localVarList";
			this.localVarList.SelectedNode = null;
			this.localVarList.Size = new System.Drawing.Size(539, 153);
			this.localVarList.TabIndex = 4;
			this.localVarList.UseColumns = true;
			// 
			// linkExceptionDetail
			// 
			this.linkExceptionDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkExceptionDetail.Location = new System.Drawing.Point(3, 142);
			this.linkExceptionDetail.Name = "linkExceptionDetail";
			this.linkExceptionDetail.Size = new System.Drawing.Size(543, 23);
			this.linkExceptionDetail.TabIndex = 5;
			this.linkExceptionDetail.TabStop = true;
			this.linkExceptionDetail.Text = "${res:MainWindow.Windows.Debug.ExceptionForm.ShowExceptionDetails}";
			this.linkExceptionDetail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExceptionDetailLinkClicked);
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.Location = new System.Drawing.Point(76, 16);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.linkExceptionDetail);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.localVarList);
			this.splitContainer.Size = new System.Drawing.Size(550, 327);
			this.splitContainer.SplitterDistance = 163;
			this.splitContainer.TabIndex = 6;
			// 
			// DebugeeExceptionForm
			// 
			this.ClientSize = new System.Drawing.Size(638, 399);
			this.Controls.Add(this.splitContainer);
			this.Name = "DebugeeExceptionForm";
			this.Controls.SetChildIndex(this.splitContainer, 0);
			this.Controls.SetChildIndex(this.textBox, 0);
			this.Controls.SetChildIndex(this.buttonBreak, 0);
			this.Controls.SetChildIndex(this.buttonContinue, 0);
			this.Controls.SetChildIndex(this.buttonTerminate, 0);
			this.Controls.SetChildIndex(this.pictureBox, 0);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.SplitContainer splitContainer;
		private Aga.Controls.Tree.TreeViewAdv localVarList;
		private System.Windows.Forms.LinkLabel linkExceptionDetail;
		#endregion
			
	}
}
