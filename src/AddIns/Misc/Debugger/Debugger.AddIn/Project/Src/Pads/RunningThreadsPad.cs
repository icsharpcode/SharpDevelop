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
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class RunningThreadsPad : AbstractPadContent
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;

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

			debuggerCore.ThreadStarted += new EventHandler<ThreadEventArgs>(AddThread);
			debuggerCore.ThreadStateChanged += new EventHandler<ThreadEventArgs>(RefreshThread);
			debuggerCore.ThreadExited += new EventHandler<ThreadEventArgs>(RemoveThread);
			debuggerCore.DebuggingPaused += new EventHandler<DebuggingPausedEventArgs>(OnDebuggingPaused);

			RefreshList();
		}
		
		public override void RedrawContent()
		{
			id.Text          = "ID";
			name.Text        = "Name";
			location.Text    = "Location";
			priority.Text    = "Priority";
			breaked.Text     = "Breaked";
		}

		void RunningThreadsListItemActivate(object sender, EventArgs e)
		{
			if (debuggerCore.IsCurrentProcessSafeForInspection) {
				debuggerCore.CurrentThread = (Thread)(runningThreadsList.SelectedItems[0].Tag);
			}
		}

		void OnDebuggingPaused(object sender, DebuggingPausedEventArgs e)
		{
			RefreshList();
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
					Function location;
					location = e.Thread.LastFunctionWithLoadedSymbols;
					if (location == null) {
						location = e.Thread.LastFunction;
					}
					if (location != null) {
						item.SubItems.Add(location.Name);
					} else {
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

		private void RefreshList()
		{
			foreach (Thread t in debuggerCore.Threads) {
				RefreshThread(this, new ThreadEventArgs(debuggerCore, t));
			}
		}
	}
}
