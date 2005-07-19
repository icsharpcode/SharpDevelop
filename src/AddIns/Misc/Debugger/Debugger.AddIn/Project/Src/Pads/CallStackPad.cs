// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//	   <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using DebuggerLibrary;

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
			callStackList.ItemActivate += new EventHandler(CallStackListItemActivate);
			name.Width = 300;
			language.Width = 400;

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

			debuggerCore.IsDebuggingChanged += new EventHandler<DebuggerEventArgs>(DebuggerStateChanged);
			debuggerCore.IsProcessRunningChanged += new EventHandler<DebuggerEventArgs>(DebuggerStateChanged);

			RefreshList();
		}
		
		public override void RedrawContent()
		{
			name.Text        = "Name";
			language.Text    = "Language";
		}
		
		void CallStackListItemActivate(object sender, EventArgs e)
		{
			if (!debuggerCore.IsProcessRunning) {
				debuggerCore.CurrentThread.CurrentFunction = (Function)(callStackList.SelectedItems[0].Tag);
			}
		}

		public void DebuggerStateChanged(object sender, DebuggerEventArgs e)
		{
			RefreshList();
		}
			
		public void RefreshList()
		{
			callStackList.BeginUpdate();
			callStackList.Items.Clear();
			if (debugger.IsProcessRunning == false && debuggerCore.CurrentThread != null) {
				foreach (Function f in debuggerCore.CurrentThread.Callstack) {
					ListViewItem item = new ListViewItem(new string[] { f.Name, "" });
					item.Tag = f;
					item.ForeColor = f.Module.SymbolsLoaded ? Color.Black : Color.Gray;
					callStackList.Items.Add(item);
				}
			}
			callStackList.EndUpdate();
		}
	}
}
