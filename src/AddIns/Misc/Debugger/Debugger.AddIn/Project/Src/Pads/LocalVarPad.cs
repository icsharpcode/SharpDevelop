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

using ICSharpCode.Core;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

using Debugger;
using Debugger.AddIn.TreeModel;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : DebuggerPad
	{
		class ItemIcon: NodeIcon {
			protected override System.Drawing.Image GetIcon(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Content.Image;
			}
		}
		
		class ItemName: NodeTextBox {
			protected override bool CanEdit(TreeNodeAdv node)
			{
				return false;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				if (node is TreeViewNode) {
					return ((TreeViewNode)node).Content.Name;
				} else {
					// Happens during incremental search
					return base.GetValue(node);
				}
			}
		}
		
		class ItemText: NodeTextBox {
			public ItemText()
			{
				this.EditEnabled = true;
				this.EditOnClick = true;
			}
			protected override bool CanEdit(TreeNodeAdv node)
			{
				AbstractNode content = ((TreeViewNode)node).Content;
				return (content is ISetText) && ((ISetText)content).CanSetText;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				if (node is TreeViewNode) {
					return ((TreeViewNode)node).Content.Text;
				} else {
					// Happens during incremental search
					return base.GetValue(node);
				}
			}
			public override void SetValue(TreeNodeAdv node, object value)
			{
				ISetText content = (ISetText)((TreeViewNode)node).Content;
				if (content.CanSetText) {
					content.SetText(value.ToString());
				}
			}
			protected override void OnDrawText(DrawEventArgs args)
			{
				AbstractNode content = ((TreeViewNode)args.Node).Content;
				if (content is ErrorNode) {
					args.TextColor = Color.Red;
				} else if (((TreeViewNode)args.Node).TextChanged) {
					args.TextColor = Color.Blue;
				}
				base.OnDrawText(args);
			}
			public override void MouseDown(TreeNodeAdvMouseEventArgs args)
			{
				AbstractNode content = ((TreeViewNode)args.Node).Content;
				if (content is IContextMenu && args.Button == MouseButtons.Right) {
					ContextMenuStrip menu = ((IContextMenu)content).GetContextMenu();
					if (menu != null) {
						menu.Show(args.Node.Tree, args.Location);
					}
				} else {
					base.MouseDown(args);
				}
			}
		}
		
		class ItemType: NodeTextBox {
			protected override bool CanEdit(TreeNodeAdv node)
			{
				return false;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				if (node is TreeViewNode) {
					return ((TreeViewNode)node).Content.Type;
				} else {
					// Happens during incremental search
					return base.GetValue(node);
				}
			}
		}
		
		TreeViewAdv localVarList;
		Debugger.Process debuggedProcess;
		
		TreeColumn nameColumn = new TreeColumn();
		TreeColumn valColumn  = new TreeColumn();
		TreeColumn typeColumn = new TreeColumn();
		
		public override Control Control {
			get {
				return localVarList;
			}
		}
		
		public TreeViewAdv LocalVarList {
			get { return localVarList; }
		}
		
		public Process Process {
			get { return debuggedProcess; }
		}
		
		protected override void InitializeComponents()
		{
			localVarList = new TreeViewAdv();
			localVarList.Columns.Add(nameColumn);
			localVarList.Columns.Add(valColumn);
			localVarList.Columns.Add(typeColumn);
			localVarList.UseColumns = true;
			localVarList.SelectionMode = TreeSelectionMode.Single;
			localVarList.LoadOnDemand = true;
			
			NodeIcon iconControl = new ItemIcon();
			iconControl.ParentColumn = nameColumn;
			localVarList.NodeControls.Add(iconControl);
			
			NodeTextBox nameControl = new ItemName();
			nameControl.ParentColumn = nameColumn;
			localVarList.NodeControls.Add(nameControl);
			
			NodeTextBox textControl = new ItemText();
			textControl.ParentColumn = valColumn;
			localVarList.NodeControls.Add(textControl);
			
			NodeTextBox typeControl = new ItemType();
			typeControl.ParentColumn = typeColumn;
			localVarList.NodeControls.Add(typeControl);
			
			RedrawContent();
		}
		
		public override void RedrawContent()
		{
			nameColumn.Header = ResourceService.GetString("Global.Name");
			nameColumn.Width = 250;
			valColumn.Header  = ResourceService.GetString("Dialog.HighlightingEditor.Properties.Value");
			valColumn.Width = 300;
			typeColumn.Header = ResourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
			typeColumn.Width = 250;
		}
		
		protected override void SelectProcess(Debugger.Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= debuggedProcess_Paused;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += debuggedProcess_Paused;
			}
			RefreshPad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		public override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedStackFrame == null) {
				localVarList.Root.Children.Clear();
				return;
			}
			
			using(new PrintTimes("Local Variables refresh")) {
				try {
					localVarList.BeginUpdate();
					Utils.DoEvents(debuggedProcess.DebuggeeState);
					TreeViewNode.SetContentRecursive(debuggedProcess, LocalVarList, new StackFrameNode(debuggedProcess.SelectedStackFrame).ChildNodes);
				} catch(AbortedBecauseDebuggeeResumedException) {
				} finally {
					localVarList.EndUpdate();
				}
			}
		}
	}
}
