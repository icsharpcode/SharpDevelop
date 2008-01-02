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
using System.Drawing;
using System.Windows.Forms;

using Debugger;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class CallStackPad : DebuggerPad
	{
		ListView  callStackList;
		Debugger.Process debuggedProcess;
		
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
		
		void CallStackListItemActivate(object sender, EventArgs e)
		{
			if (debuggedProcess.IsPaused) {
				Function f = (Function)(callStackList.SelectedItems[0].Tag);
				if (f.HasSymbols) {
					if (debuggedProcess.SelectedThread != null) {
						debuggedProcess.SelectedThread.SelectedFunction = f;
						debuggedProcess.OnDebuggeeStateChanged(); // Force refresh of pads
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
			bool showArgumentNames = ShowArgumentNames;
			bool showArgumentValues = ShowArgumentValues;
			bool showExternalMethods = ShowExternalMethods;
			bool lastItemIsExternalMethod = false;
			int callstackItems = 0;
			
			callStackList.BeginUpdate();
			callStackList.Items.Clear();
			if (debuggedProcess != null && debuggedProcess.SelectedThread != null && debuggedProcess.IsPaused) {
				foreach (Function f in debuggedProcess.SelectedThread.Callstack) {
					ListViewItem item;
					if (f.HasSymbols || showExternalMethods) {
						// Show the method in the list
						string name = f.MethodInfo.Name;
						if (showArgumentNames || showArgumentValues) {
							name += "(";
							for (int i = 0; i < f.ArgumentCount; i++) {
								string parameterName = null;
								string argValue = null;
								if (showArgumentNames) {
									try {
										parameterName = f.MethodInfo.GetParameterName(i);
									} catch { }
									if (parameterName == "") parameterName = null;
								}
								if (showArgumentValues) {
									try {
										argValue = f.GetArgument(i).AsString;
									} catch { }
								}
								if (parameterName != null && argValue != null) {
									name += parameterName + "=" + argValue;
								}
								if (parameterName != null && argValue == null) {
									name += parameterName;
								}
								if (parameterName == null && argValue != null) {
									name += argValue;
								}
								if (parameterName == null && argValue == null) {
									name += ResourceService.GetString("Global.NA");
								}
								if (i < f.ArgumentCount - 1) {
									name += ", ";
								}
							}
							name += ")";
						}
						item = new ListViewItem(new string[] { name, "" });
						lastItemIsExternalMethod = false;
					} else {
						// Show [External methods] in the list
						if (lastItemIsExternalMethod) {
							continue;
						} else {
							item = new ListViewItem(new string[] { ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ExternalMethods"), "" });
							lastItemIsExternalMethod = true;
						}
					}
					item.Tag = f;
					item.ForeColor = f.HasSymbols ? Color.Black : Color.Gray;
					callStackList.Items.Add(item);
					callstackItems++;
					if (callstackItems >= 100) break;
				}
			}
			callStackList.EndUpdate();
		}
	}
}
