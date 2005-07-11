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
//using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

//using ICSharpCode.Core.Properties;

using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class RunningThreadsPad : AbstractPadContent
	{
		WindowsDebugger debugger;

		ListView  runningThreadsList;
		
		ColumnHeader id          = new ColumnHeader();
		ColumnHeader name        = new ColumnHeader();
		ColumnHeader location    = new ColumnHeader();
		ColumnHeader priority    = new ColumnHeader();
		ColumnHeader breaked     = new ColumnHeader();
		
		public override Control Control {
			get {
				return runningThreadsList;
			}
		}
		
		public RunningThreadsPad() //: base("${res:MainWindow.Windows.Debug.Threads}", null)
		{
			InitializeComponents();
		}
			
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;

			runningThreadsList = new ListView();
			runningThreadsList.FullRowSelect = true;
			runningThreadsList.AutoArrange = true;
			runningThreadsList.Alignment   = ListViewAlignment.Left;
			runningThreadsList.View = View.Details;
			runningThreadsList.Dock = DockStyle.Fill;
			runningThreadsList.GridLines  = false;
			runningThreadsList.Activation = ItemActivation.OneClick;
			runningThreadsList.Columns.AddRange(new ColumnHeader[] {id, name, location, priority, breaked} );
			runningThreadsList.ItemActivate += new EventHandler(RunningThreadsListItemActivate);
			id.Width = 100;
			name.Width = 300;
			location.Width = 250;
			priority.Width = 120;
			breaked.Width = 80;
		
			
			debugger.ThreadStarted += new ThreadEventHandler(AddThread);
			debugger.ThreadStateChanged += new ThreadEventHandler(RefreshThread);
			debugger.ThreadExited += new ThreadEventHandler(RemoveThread);
			debugger.IsProcessRunningChanged += new DebuggerEventHandler(DebuggerStateChanged);
	
			RedrawContent();
		}
		
		public override void RedrawContent()
		{
			id.Text          = "ID";
			name.Text        = "Name";
			location.Text    = "Location";
			priority.Text    = "Priority";
			breaked.Text     = "Breaked";

            RefreshAllItems();
		}

		void RunningThreadsListItemActivate(object sender, EventArgs e)
		{
			debugger.SelectThread((Thread)(runningThreadsList.SelectedItems[0].Tag));
			debugger.JumpToCurrentLine();
			CallStackPad callStackPad = (CallStackPad)WorkbenchSingleton.Workbench.GetPad(typeof(CallStackPad)).PadContent;
			callStackPad.RefreshList();
		}


		private void AddThread(object sender, ThreadEventArgs e) 
		{
			runningThreadsList.Items.Add(new ListViewItem(e.Thread.ID.ToString()));
			RefreshThread(this, e);
		}

		private void RefreshThread(object sender, ThreadEventArgs e) 
		{
			foreach (ListViewItem item in runningThreadsList.Items) {
				if (e.Thread.ID.ToString() == item.Text) {
					item.SubItems.Clear();
					item.Text = e.Thread.ID.ToString();
					item.Tag = e.Thread;
					item.SubItems.Add(e.Thread.Name);
					try {
						item.SubItems.Add(e.Thread.CurrentFunction.Name);
					} catch (CurrentFunctionNotAviableException) {
						item.SubItems.Add("N/A");
					}
					item.SubItems.Add(e.Thread.Priority.ToString());
					item.SubItems.Add(e.Thread.Suspended.ToString());
                    return;
				}
            }
            AddThread(this, e);
		}

		private void RemoveThread(object sender, ThreadEventArgs e) 
		{
			foreach (ListViewItem item in runningThreadsList.Items) {
				if (e.Thread.ID.ToString() == item.Text)
					item.Remove();
			}
		}
		
		public void DebuggerStateChanged(object sender, DebuggerEventArgs e)
		{
			RefreshAllItems();
		}

		private void RefreshAllItems()
		{
			foreach (Thread t in NDebugger.Instance.Threads) {
				RefreshThread(this, new ThreadEventArgs(t));
			}
		}
	}
}
