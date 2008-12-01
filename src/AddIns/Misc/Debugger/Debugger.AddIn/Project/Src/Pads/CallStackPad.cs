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
using System.Text;
using System.Windows.Forms;
using Debugger;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
using Exception=System.Exception;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class CallStackPad : DebuggerPad
	{
		ListView  callStackList;
		Process debuggedProcess;
		
		ColumnHeader name     = new ColumnHeader();
		ColumnHeader language = new ColumnHeader();
		
		public override Control Control {
			get {
				return callStackList;
			}
		}
		
		protected override void InitializeComponents()
		{
			callStackList = new ListView();
			callStackList.FullRowSelect = true;
			callStackList.AutoArrange = true;
			callStackList.Alignment   = ListViewAlignment.Left;
			callStackList.View = View.Details;
			callStackList.Dock = DockStyle.Fill;
			callStackList.GridLines  = false;
			callStackList.Activation = ItemActivation.OneClick;
			callStackList.Columns.AddRange(new ColumnHeader[] {name, language} );
			callStackList.ContextMenuStrip = CreateContextMenuStrip();
			callStackList.ItemActivate += new EventHandler(CallStackListItemActivate);
			name.Width = 500;
			language.Width = 50;

			RedrawContent();
		}
		
		public override void RedrawContent()
		{
			name.Text     = ResourceService.GetString("Global.Name");
			language.Text = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.Language");
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
		
		void CallStackListItemActivate(object sender, EventArgs e)
		{
			if (debuggedProcess.IsPaused) {
				StackFrame frame = (StackFrame)(callStackList.SelectedItems[0].Tag);
				if (frame.HasSymbols) {
					if (debuggedProcess.SelectedThread != null) {
						debuggedProcess.SelectedThread.SelectedStackFrame = frame;
						debuggedProcess.OnPaused(); // Force refresh of pads
					}
				} else {
					MessageService.ShowMessage("${res:MainWindow.Windows.Debug.CallStack.CannotSwitchWithoutSymbols}", "${res:MainWindow.Windows.Debug.CallStack.FunctionSwitch}");
				}
			} else {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.CallStack.CannotSwitchWhileRunning}", "${res:MainWindow.Windows.Debug.CallStack.FunctionSwitch}");
			}
		}
		
		public override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedThread == null) {
				callStackList.Items.Clear();
				return;
			}
			
			using(new PrintTimes("Callstack refresh")) {
				try {
					Utils.DoEvents(debuggedProcess);
					List<ListViewItem> items = CreateItems();
					UpdateItems(items);
				} catch(AbortedBecauseDebuggeeResumedException) {
				} catch(Exception) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						throw;
					}
				}
			}
		}
		
		public List<ListViewItem> CreateItems()
		{
			bool showExternalMethods = DebuggingOptions.Instance.ShowExternalMethods;
			bool lastItemIsExternalMethod = false;
			
			List<ListViewItem> items = new List<ListViewItem>();
			foreach (StackFrame frame in debuggedProcess.SelectedThread.GetCallstack(100)) {
				ListViewItem item;
				if (frame.HasSymbols || showExternalMethods) {
					// Show the method in the list
					item = new ListViewItem(new string[] { GetFullName(frame), "" });
					lastItemIsExternalMethod = false;
				} else {
					// Show [External methods] in the list
					if (lastItemIsExternalMethod) continue;
						
					item = new ListViewItem(new string[] { ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ExternalMethods"), "" });
					lastItemIsExternalMethod = true;
				}
				item.Tag = frame;
				item.ForeColor = frame.HasSymbols ? Color.Black : Color.Gray;
				items.Add(item);
				
				Utils.DoEvents(debuggedProcess);
			}
			
			return items;
		}
		
		public void UpdateItems(List<ListViewItem> items)
		{
			callStackList.BeginUpdate();
			// Adjust count
			while (callStackList.Items.Count < items.Count) {
				callStackList.Items.Insert(0, new ListViewItem(new string[2]));
			}
			while (callStackList.Items.Count > items.Count) {
				callStackList.Items.RemoveAt(0);
			}
			// Overwrite
			for(int i = 0; i < items.Count; i++) {
				callStackList.Items[i].SubItems[0] = items[i].SubItems[0];
				callStackList.Items[i].SubItems[1] = items[i].SubItems[1];
				callStackList.Items[i].Tag         = items[i].Tag;
				callStackList.Items[i].ForeColor   = items[i].ForeColor;
				
				Utils.DoEvents(debuggedProcess);
			}
			callStackList.EndUpdate();
		}
		
		public string GetFullName(StackFrame frame)
		{
			bool showArgumentNames = DebuggingOptions.Instance.ShowArgumentNames;
			bool showArgumentValues = DebuggingOptions.Instance.ShowArgumentValues;
			
			StringBuilder name = new StringBuilder();
			name.Append(frame.MethodInfo.DeclaringType.Name);
			name.Append('.');
			name.Append(frame.MethodInfo.Name);
			if (showArgumentNames || showArgumentValues) {
				name.Append("(");
				for (int i = 0; i < frame.ArgumentCount; i++) {
					string parameterName = null;
					string argValue = null;
					if (showArgumentNames) {
						try {
							parameterName = frame.MethodInfo.GetParameterName(i);
						} catch { }
						if (parameterName == "") parameterName = null;
					}
					if (showArgumentValues) {
						try {
							argValue = frame.GetArgumentValue(i).AsString;
						} catch { }
					}
					if (parameterName != null && argValue != null) {
						name.Append(parameterName);
						name.Append("=");
						name.Append(argValue);
					}
					if (parameterName != null && argValue == null) {
						name.Append(parameterName);
					}
					if (parameterName == null && argValue != null) {
						name.Append(argValue);
					}
					if (parameterName == null && argValue == null) {
						name.Append(ResourceService.GetString("Global.NA"));
					}
					if (i < frame.ArgumentCount - 1) {
						name.Append(", ");
					}
				}
				name.Append(")");
			}
			return name.ToString();
		}
	}
}
