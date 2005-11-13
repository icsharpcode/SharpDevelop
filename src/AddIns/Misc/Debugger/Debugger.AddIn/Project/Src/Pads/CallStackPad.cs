// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	public class CallStackPad : AbstractPadContent
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;

		ListView  callStackList;
		
		ColumnHeader name     = new ColumnHeader();
		ColumnHeader language = new ColumnHeader();
		
		public override Control Control {
			get {
				return callStackList;
			}
		}
		
		public CallStackPad()// : base("${res:MainWindow.Windows.Debug.CallStack}", null)
		{
			InitializeComponents();	
		}		
				
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;

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

			if (debugger.ServiceInitialized) {
				InitializeDebugger();
			} else {
				debugger.Initialize += delegate {
					InitializeDebugger();
				};
			}
		}

		public void InitializeDebugger()
		{
			debuggerCore = debugger.DebuggerCore;

			debuggerCore.DebuggingPaused += new EventHandler<DebuggingPausedEventArgs>(DebuggingPaused);
			debuggerCore.DebuggingResumed += new EventHandler<DebuggerEventArgs>(DebuggingResumed);

			RefreshList();
		}
		
		public override void RedrawContent()
		{
			name.Text        = "Name";
			language.Text    = "Language";
		}
		
		public bool ShowArgumentNames {
			get {
				return debugger.Properties.Get("ShowArgumentNames", true);
			}
			set {
				debugger.Properties.Set("ShowArgumentNames", value);
			}
		}
		
		public bool ShowArgumentValues {
			get {
				return debugger.Properties.Get("ShowArgumentValues", true);
			}
			set {
				debugger.Properties.Set("ShowArgumentValues", value);
			}
		}
		
		public bool ShowExternalMethods {
			get {
				return debugger.Properties.Get("ShowExternalMethods", false);
			}
			set {
				debugger.Properties.Set("ShowExternalMethods", value);
			}
		}
		
		ContextMenuStrip CreateContextMenuStrip()
		{
			ContextMenuStrip menu = new ContextMenuStrip();			
			menu.Opening += FillContextMenuStrip;
			return menu;
		}
		
		void FillContextMenuStrip(object sender, CancelEventArgs e)
		{
			ContextMenuStrip menu = sender as ContextMenuStrip;
			menu.Items.Clear();
			
			ToolStripMenuItem argNamesItem;
			argNamesItem = new ToolStripMenuItem();
			argNamesItem.Text = "Show argument names";
			argNamesItem.Checked = ShowArgumentNames;	
			argNamesItem.Click +=
				delegate {
					ShowArgumentNames = !ShowArgumentNames;
					RefreshList();
				};
			
			ToolStripMenuItem argValuesItem;
			argValuesItem = new ToolStripMenuItem();
			argValuesItem.Text = "Show argument values";
			argValuesItem.Checked = ShowArgumentValues;	
			argValuesItem.Click +=
				delegate {
					ShowArgumentValues = !ShowArgumentValues;
					RefreshList();
				};
			
			ToolStripMenuItem extMethodsItem;
			extMethodsItem = new ToolStripMenuItem();
			extMethodsItem.Text = "Show external methods";
			extMethodsItem.Checked = ShowExternalMethods;	
			extMethodsItem.Click +=
				delegate {
					ShowExternalMethods = !ShowExternalMethods;
					RefreshList();
				};
			
			menu.Items.AddRange(new ToolStripItem[] {
			                    	argNamesItem,
			                    	argValuesItem,
			                    	extMethodsItem
			                    });
			
			e.Cancel = false;
		}
		
		void CallStackListItemActivate(object sender, EventArgs e)
		{
			if (debuggerCore.IsPaused) {
				Function f = (Function)(callStackList.SelectedItems[0].Tag);
				if (f.HasSymbols) {
					if (debuggerCore.CurrentThread != null) {
						debuggerCore.CurrentThread.SetCurrentFunction(f);
					}
				} else {
					MessageBox.Show("You can not switch to function without symbols", "Function switch");
				}
			} else {
				MessageBox.Show("You can not switch functions while the debugger is running.", "Function switch");
			}
		}

		void DebuggingPaused(object sender, DebuggingPausedEventArgs e)
		{
			RefreshList();
		}

		void DebuggingResumed(object sender, DebuggerEventArgs e)
		{
			RefreshList();
		}
			
		public void RefreshList()
		{
			bool showArgumentNames = ShowArgumentNames;
			bool showArgumentValues = ShowArgumentValues;
			bool showExternalMethods = ShowExternalMethods;
			bool lastItemIsExternalMethod = false;
			int callstackItems = 0;
			
			callStackList.BeginUpdate();
			callStackList.Items.Clear();
			if (debuggerCore != null && debuggerCore.CurrentThread != null) {
				foreach (Function f in debuggerCore.CurrentThread.Callstack) {
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
										argValue = f.GetArgumentVariable(i).AsString.ToString();
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
