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

using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Debugger;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using Exception=System.Exception;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : DebuggerPad
	{
		TreeViewAdv localVarList;
		Process debuggedProcess;
		static LocalVarPad instance;
		
		readonly TreeColumn nameColumn = new TreeColumn();
		readonly TreeColumn valColumn  = new TreeColumn();
		readonly TreeColumn typeColumn = new TreeColumn();
		
		public LocalVarPad()
		{
			instance = this;
		}
		
		/// <remarks>Always check if Instance is null, might be null if pad is not opened!</remarks>
		public static LocalVarPad Instance {
			get { return instance; }
		}
		
		/// <remarks>
		/// This is not used anywhere, but it is neccessary to be overridden in children of AbstractPadContent.
		/// </remarks>
		public override Control Control {
			get {
				return localVarList;
			}
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
			
			localVarList.AutoRowHeight = true;
			
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
		
		protected override void SelectProcess(Process process)
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
					Utils.DoEvents(debuggedProcess);
					TreeViewVarNode.SetContentRecursive(debuggedProcess, localVarList, new StackFrameNode(debuggedProcess.SelectedStackFrame).ChildNodes);
				} catch(AbortedBecauseDebuggeeResumedException) {
				} catch(Exception) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						throw;
					}
				} finally {
					localVarList.EndUpdate();
				}
			}
		}
	}
}
