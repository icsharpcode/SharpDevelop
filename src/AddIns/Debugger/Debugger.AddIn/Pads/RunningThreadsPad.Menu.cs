// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Debugger;
using Debugger.AddIn.Pads.ParallelPad;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class RunningThreadsPad
	{
		ContextMenu CreateContextMenuStrip()
		{
			ContextMenu menu = new ContextMenu();
			menu.Opened += FillContextMenuStrip;
			return menu;
		}

		void FillContextMenuStrip(object sender, RoutedEventArgs e)
		{
			var items = runningThreadsList.SelectedItems;
			if (items == null || items.Count == 0) {
				e.Handled = true;
				return;
			}
			
			ThreadModel item = items[0] as ThreadModel;
			if (item == null)
				return;
			
			ContextMenu menu = sender as ContextMenu;
			menu.Items.Clear();
			
			MenuItem freezeItem;
			freezeItem = new MenuItem();
			freezeItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.Threads.Freeze");
			freezeItem.IsChecked = item.Thread.Suspended;
			freezeItem.Click +=
				delegate {
				if (items == null || items.Count == 0) {
					e.Handled = true;
					return;
				}
				bool suspended = item.Thread.Suspended;
				
				if (!debuggedProcess.IsPaused) {
					MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotFreezeWhileRunning}", "${res:MainWindow.Windows.Debug.Threads.Freeze}");
					return;
				}
				
				foreach(ThreadModel current in items.OfType<ThreadModel>()) {
					current.Thread.Suspended = !suspended;
				}
				InvalidatePad();
			};
			
			menu.Items.Add(freezeItem);
		}
	}
}
