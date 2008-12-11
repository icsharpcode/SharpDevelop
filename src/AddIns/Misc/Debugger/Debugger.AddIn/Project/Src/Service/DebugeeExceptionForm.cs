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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Debugger;
using Debugger.AddIn.TreeModel;
using Debugger.Expressions;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	internal sealed partial class DebugeeExceptionForm
	{
		private DebugeeExceptionForm()
		{
			InitializeComponent();
			
            // Windows form designer cannot place a component declared in a base class in a container component
            // declared in a child class. Hence we do it manually.
            Controls.Remove(textBox);
            textBox.Location = new Point(0, 4);
            splitContainer.Panel1.Controls.Add(textBox);
            
            // To make the exceptionDetails size properly, it must be rendered full size in the designer.
            // To get the text right we set the panel to the opposite of what we want and then fire linkExceptionDetail.Click().
            splitContainer.Panel2Collapsed = DebuggingOptions.Instance.ShowExceptionDetails;
            DebuggingOptions.Instance.ShowExceptionDetails = ! DebuggingOptions.Instance.ShowExceptionDetails;
            linkExceptionDetailLinkClicked(linkExceptionDetail, new EventArgs());
            
            WindowState = DebuggingOptions.Instance.DebuggeeExceptionWindowState;
			Size = DebuggingOptions.Instance.DebuggeeExceptionWindowSize;
		           
			splitContainer.SplitterDistance = DebuggingOptions.Instance.DebugeeExceptionSplitterDistance;
			// Set this here so it doesnt fire on startup replacing the saved value with a default.
			this.splitContainer.SplitterMoved += new SplitterEventHandler(this.splitContainerMoved);
			
			InitializeExceptionDetails();
		}
		
		/// <summary>
		/// Setup the columns for the exceptionDetails.
		/// </summary>
		private void InitializeExceptionDetails() {
			NodeIcon iconControl = new ItemIcon();
			NodeTextBox nameControl = new ItemName();
			NodeTextBox textControl = new ItemText();
			NodeTextBox typeControl = new ItemType();
			
			TreeColumn nameColumn = new TreeColumn();
			TreeColumn valColumn  = new TreeColumn();
			TreeColumn typeColumn = new TreeColumn();
			
			exceptionDetails.Columns.Add(nameColumn);
			exceptionDetails.Columns.Add(valColumn);
			exceptionDetails.Columns.Add(typeColumn);
			exceptionDetails.UseColumns = true;
			exceptionDetails.SelectionMode = TreeSelectionMode.Single;
			exceptionDetails.LoadOnDemand = true;
			
			iconControl.ParentColumn = nameColumn;
			exceptionDetails.NodeControls.Add(iconControl);
			
			nameControl.ParentColumn = nameColumn;
			exceptionDetails.NodeControls.Add(nameControl);
			
			textControl.ParentColumn = valColumn;
			exceptionDetails.NodeControls.Add(textControl);
			
			typeControl.ParentColumn = typeColumn;
			exceptionDetails.NodeControls.Add(typeControl);
		
			nameColumn.Header = ResourceService.GetString("Global.Name");
			nameColumn.Width = 165;
			valColumn.Header  = ResourceService.GetString("Dialog.HighlightingEditor.Properties.Value");
			valColumn.Width = 200;
			typeColumn.Header = ResourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
			typeColumn.Width = 170;
		}
		
		public static Result Show(Process process, string title, string message, Bitmap icon, bool canContinue)
		{
			using (DebugeeExceptionForm form = new DebugeeExceptionForm()) {
				form.Text = title;
				form.pictureBox.Image = icon;
				form.textBox.Text = message;
				
				IList<AbstractNode> exceptionNodes = new List<AbstractNode>();
				exceptionNodes.Add(ValueNode.Create(new CurrentExceptionExpression()));
				
				form.exceptionDetails.BeginUpdate();
				TreeViewVarNode.SetContentRecursive(process, form.exceptionDetails, exceptionNodes);
				form.exceptionDetails.EndUpdate();
				form.exceptionDetails.Refresh();

				form.buttonContinue.Enabled = canContinue;
				form.ShowDialog(WorkbenchSingleton.MainForm);
				return form.result;
			}
		}
		
		void debugeeExceptionFormResize(object sender, EventArgs e)
		{
			DebuggingOptions.Instance.DebuggeeExceptionWindowSize = Size;
			DebuggingOptions.Instance.DebuggeeExceptionWindowState = WindowState;
		}
						
		void linkExceptionDetailLinkClicked(object sender, EventArgs e)
		{
            splitContainer.Panel2Collapsed = ! splitContainer.Panel2Collapsed;
            DebuggingOptions.Instance.ShowExceptionDetails = ! splitContainer.Panel2Collapsed;
			linkExceptionDetail.Text = splitContainer.Panel2Collapsed 
				? StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.ShowExceptionDetails}")
				: StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.HideExceptionDetails}");
		}
		
		void splitContainerMoved(object sender, EventArgs e)
		{
			DebuggingOptions.Instance.DebugeeExceptionSplitterDistance = splitContainer.SplitterDistance;
		}
	}
}
