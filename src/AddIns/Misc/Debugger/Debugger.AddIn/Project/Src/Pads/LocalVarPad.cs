// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : DebuggerPad
	{
		DebuggerTreeListView localVarList;
		Debugger.Process debuggedProcess;
		
		ColumnHeader name = new ColumnHeader();
		ColumnHeader val  = new ColumnHeader();
		ColumnHeader type = new ColumnHeader();
		
		public override Control Control {
			get {
				return localVarList;
			}
		}
		
		protected override void InitializeComponents()
		{
			//iconsService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			localVarList = new DebuggerTreeListView();
			localVarList.SmallImageList = DebuggerIcons.ImageList;
			localVarList.ShowPlusMinus = true;
			localVarList.FullRowSelect = true;
			localVarList.Dock = DockStyle.Fill;
			localVarList.Sorting = SortOrder.Ascending;
			//localVarList.GridLines  = false;
			//localVarList.Activation = ItemActivation.OneClick;
			localVarList.Columns.AddRange(new ColumnHeader[] {name, val, type} );
			name.Width = 250;
			val.Width = 300;
			type.Width = 250;
			localVarList.Visible = false;
			localVarList.SizeChanged += new EventHandler(localVarList_SizeChanged);
			localVarList.BeforeExpand += new TreeListViewCancelEventHandler(localVarList_BeforeExpand);
			localVarList.AfterExpand += new TreeListViewEventHandler(localVarList_AfterExpand);
			
			
			RedrawContent();
		}
		
		public override void RedrawContent()
		{
			name.Text = ResourceService.GetString("Global.Name");
			val.Text  = ResourceService.GetString("Dialog.HighlightingEditor.Properties.Value");
			type.Text = ResourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
		}
		
		// This is a walkarond for a visual issue
		void localVarList_SizeChanged(object sender, EventArgs e)
		{
			localVarList.Visible = true;
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
		
		public override void RefreshPad()
		{
			localVarList.BeginUpdate();
			localVarList.Items.Clear();
			if (debuggedProcess != null) {
				foreach(NamedValue val in debuggedProcess.LocalVariables) {
					localVarList.Items.Add(new TreeListViewDebuggerItem(val));
				}
			}
			localVarList.EndUpdate();
		}
		
		void localVarList_BeforeExpand(object sender, TreeListViewCancelEventArgs e)
		{
			if (debuggedProcess.IsPaused) {
				((TreeListViewDebuggerItem)e.Item).Populate();
			} else {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.LocalVariables.CannotExploreVariablesWhileRunning}");
				e.Cancel = true;
			}
		}
		
		void localVarList_AfterExpand(object sender, TreeListViewEventArgs e)
		{
			UpdateSubTree(e.Item);
		}
		
		static void UpdateSubTree(TreeListViewItem tree)
		{
			foreach(TreeListViewItem item in tree.Items) {
				((TreeListViewDebuggerItem)item).Update();
				if (item.IsExpanded) UpdateSubTree(item);
			}
		}
	}
}
