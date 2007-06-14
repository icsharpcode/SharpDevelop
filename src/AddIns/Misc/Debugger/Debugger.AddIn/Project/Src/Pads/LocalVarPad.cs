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
using System.Windows.Forms;

using ICSharpCode.Core;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : DebuggerPad
	{
		class ItemIcon: NodeIcon {
			protected override System.Drawing.Image GetIcon(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Icon;
			}
		}
		
		class ItemName: NodeTextBox {
			protected override bool CanEdit(TreeNodeAdv node)
			{
				return false;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Name;
			}
		}
		
		class ItemText: NodeTextBox {
			protected override bool CanEdit(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).CanEditText;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Text;
			}
		}
		
		class ItemType: NodeTextBox {
			protected override bool CanEdit(TreeNodeAdv node)
			{
				return false;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Type;
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
		
		protected override void InitializeComponents()
		{
			localVarList = new TreeViewAdv();
			localVarList.Columns.Add(nameColumn);
			localVarList.Columns.Add(valColumn);
			localVarList.Columns.Add(typeColumn);
			localVarList.UseColumns = true;
			localVarList.SelectionMode = TreeSelectionMode.Single;
			localVarList.LoadOnDemand = true;
			localVarList.VisibleChanged += delegate { if (localVarList.Visible) RefreshPad(); };
			localVarList.SizeChanged += delegate { RefreshPad(); };
			
			localVarList.Expanding += delegate(object sender, TreeViewAdvEventArgs e) {
				if (e.Node is TreeViewNode) ((TreeViewNode)e.Node).OnExpanding();
			};
			localVarList.Expanded += delegate(object sender, TreeViewAdvEventArgs e) {
				if (e.Node is TreeViewNode) ((TreeViewNode)e.Node).OnExpanded();
			};
			localVarList.Collapsed += delegate(object sender, TreeViewAdvEventArgs e) {
				if (e.Node is TreeViewNode) ((TreeViewNode)e.Node).OnCollapsed();
			};
			
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
				debuggedProcess.DebuggeeStateChanged -= debuggedProcess_DebuggeeStateChanged;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.DebuggeeStateChanged += debuggedProcess_DebuggeeStateChanged;
			}
			RefreshPad();
		}
		
		void debuggedProcess_DebuggeeStateChanged(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		public override void RefreshPad()
		{
			DateTime start = Debugger.Util.HighPrecisionTimer.Now;
			if (debuggedProcess != null && debuggedProcess.SelectedFunction != null) {
				TreeViewNode.UpdateNodes(localVarList, localVarList.Root.Children, new FunctionItem(debuggedProcess.SelectedFunction).SubItems);
			} else {
				TreeViewNode.UpdateNodes(localVarList, localVarList.Root.Children, new ListItem[0]);
			}
			DateTime end = Debugger.Util.HighPrecisionTimer.Now;
			LoggingService.InfoFormatted("Local Variables pad refreshed ({0} ms)", (end - start).TotalMilliseconds);
		}
	}
}
