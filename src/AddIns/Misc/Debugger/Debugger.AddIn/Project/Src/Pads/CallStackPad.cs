// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using Debugger;

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
						string name = f.Name;
						if (showArgumentNames || showArgumentValues) {
							name += "(";
							for (int i = 0; i < f.ArgumentCount; i++) {
								string parameterName = null;
								string argValue = null;
								if (showArgumentNames) {
									try {
										parameterName = f.GetParameterName(i);
									} catch { }
									if (parameterName == "") parameterName = null;
								}
								if (showArgumentValues) {
									try {
										argValue = f.GetArgumentVariable(i).Value.AsString;
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
									name += "n/a";
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
							item = new ListViewItem(new string[] { "[External methods]", "" });
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
