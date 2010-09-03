// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.ComponentModel;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class RunningThreadsPad
	{
		ContextMenuStrip CreateContextMenuStrip()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			menu.Opening += FillContextMenuStrip;
			return menu;
		}
		
		void FillContextMenuStrip(object sender, CancelEventArgs e)
		{
			ListView.SelectedListViewItemCollection items = runningThreadsList.SelectedItems;
			
			if (items.Count == 0) {
				e.Cancel = true;
				return;
			}
			
			ListViewItem item = items[0];
			
			ContextMenuStrip menu = sender as ContextMenuStrip;
			menu.Items.Clear();
			
			ToolStripMenuItem freezeItem;
			freezeItem = new ToolStripMenuItem();
			freezeItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.Threads.Freeze");
			freezeItem.Checked = (item.Tag as Thread).Suspended;
			freezeItem.Click +=
				delegate {
				ListView.SelectedListViewItemCollection selItems = runningThreadsList.SelectedItems;
				if (selItems.Count == 0) {
					return;
				}
				bool suspended = (selItems[0].Tag as Thread).Suspended;
				
				if (!debuggedProcess.IsPaused) {
					MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotFreezeWhileRunning}", "${res:MainWindow.Windows.Debug.Threads.Freeze}");
					return;
				}
				
				foreach(ListViewItem i in selItems) {
					(i.Tag as Thread).Suspended = !suspended;
				}
				RefreshPad();
			};
			
			menu.Items.AddRange(new ToolStripItem[] {
			                    	freezeItem,
			                    });
			
			e.Cancel = false;
		}
	}
}
