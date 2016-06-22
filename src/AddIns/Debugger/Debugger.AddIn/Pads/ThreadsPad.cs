// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Debugger;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Workbench;
using Thread = Debugger.Thread;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ThreadsPad : AbstractPadContent
	{
		ListView listView;
		
		public override object Control {
			get { return listView; }
		}
		
		public ThreadsPad()
		{
			var res = new CommonResources();
			res.InitializeComponent();
			
			ContextMenu contextMenu = new ContextMenu();
			contextMenu.Opened += FillContextMenuStrip;
			
			listView = new ListView();
			listView.View = (GridView)res["threadsGridView"];
			listView.ContextMenu = contextMenu;
			listView.MouseDoubleClick += listView_MouseDoubleClick;
			listView.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "70;100%;75;75");
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}
		
		void RefreshPad()
		{
			Process process = WindowsDebugger.CurrentProcess;
			
			if (process == null || process.IsRunning) {
				listView.ItemsSource = null;
			} else {
				List<ThreadItem> items = new List<ThreadItem>();
				foreach(var thread in process.Threads) {
					items.Add(new ThreadItem(thread));
				}
				listView.ItemsSource = items;
			}
		}

		void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ThreadItem item = listView.SelectedItem as ThreadItem;
			
			if (WindowsDebugger.CurrentProcess != null) {
				if (WindowsDebugger.CurrentProcess.IsPaused) {
					WindowsDebugger.CurrentThread = item.Thread;
					WindowsDebugger.Instance.JumpToCurrentLine();
					WindowsDebugger.RefreshPads();
				} else {
					MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotSwitchWhileRunning}", "${res:MainWindow.Windows.Debug.Threads.ThreadSwitch}");
				}
			}
		}
		
		void FillContextMenuStrip(object sender, RoutedEventArgs e)
		{
			var items = listView.SelectedItems.OfType<ThreadItem>();
			// Suspended property is safe to access at any point
			bool suspended = items.All(t => t.Thread.Suspended);
			
			ContextMenu menu = sender as ContextMenu;
			menu.Items.Clear();
			
			MenuItem freezeItem;
			freezeItem = new MenuItem();
			freezeItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.Threads.Freeze");
			freezeItem.IsChecked = suspended;
			freezeItem.Click += delegate {
				if (WindowsDebugger.CurrentProcess == null || WindowsDebugger.CurrentProcess.IsRunning) {
					MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotFreezeWhileRunning}", "${res:MainWindow.Windows.Debug.Threads.Freeze}");
					return;
				}
				foreach(ThreadItem item in items.OfType<ThreadItem>()) {
					item.Thread.Suspended = !suspended;
				}
				WindowsDebugger.RefreshPads();
			};
			
			menu.Items.Add(freezeItem);
		}
	}
	
	public class ThreadItem
	{
		public Thread Thread { get; private set; }
		public uint ID { get; private set; }
		public string Name { get; private set; }
		public string Priority { get; private set; }
		public string Frozen { get; private set; }
		
		public ThreadItem(Thread thread)
		{
			// We want to egarly evaluate the properties while the process is paused
			// rather then wait until the GUI acesses them at some unspecified point
			this.Thread = thread;
			this.ID = thread.ID;
			this.Name = thread.Name;
			this.Priority = PriorityToString(thread.Priority);
			this.Frozen = ResourceService.GetString(thread.Suspended ? "Global.Yes" : "Global.No");
		}
		
		static string PriorityToString(System.Threading.ThreadPriority priority)
		{
			switch (priority) {
				case System.Threading.ThreadPriority.Highest:
					return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Highest");
				case System.Threading.ThreadPriority.AboveNormal:
					return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.AboveNormal");
				case System.Threading.ThreadPriority.Normal:
					return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Normal");
				case System.Threading.ThreadPriority.BelowNormal:
					return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.BelowNormal");
				case System.Threading.ThreadPriority.Lowest:
					return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Lowest");
				default:
					return ResourceService.GetString("Global.NA");
			}
		}
	}
}
