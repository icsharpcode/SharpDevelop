// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;

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
		
		static string privateMembersName, staticMembersName, privateStaticMembersName;
		
		public override void RedrawContent()
		{
			name.Text = ResourceService.GetString("Global.Name");
			val.Text  = ResourceService.GetString("Dialog.HighlightingEditor.Properties.Value");
			type.Text = ResourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
			
			privateMembersName = StringParser.Parse("<${res:MainWindow.Windows.Debug.LocalVariables.PrivateMembers}>");
			staticMembersName = StringParser.Parse("<${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}>");
			privateStaticMembersName = StringParser.Parse("<${res:MainWindow.Windows.Debug.LocalVariables.PrivateStaticMembers}>");
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
				AddVariableCollectionToTree(debuggedProcess.LocalVariables, localVarList.Items);
			}
			localVarList.EndUpdate();
		}
		
		//delegate void AddVariableMethod(NamedValue val);
		
		public static void AddVariableCollectionToTree(NamedValueCollection collection, TreeListViewItemCollection tree)
		{
//			foreach(VariableCollection sub in varCollection.SubCollections) {
//				VariableCollection subCollection = sub;
//				TreeListViewItem subMenu = new TreeListViewItem("<" + subCollection.Name + ">", 0);
//				subMenu.SubItems.Add(subCollection.Value);
//				tree.Add(subMenu);
//				TreeListViewItem.TreeListViewItemHanlder populate = null;
//				populate = delegate {
//					AddVariableCollectionToTree(subCollection, subMenu.Items);
//					subMenu.AfterExpand -= populate;
//				};
//				subMenu.AfterExpand += populate;
//			}
			foreach(NamedValue val in collection) {
				tree.Add(new TreeListViewDebuggerItem(val));
			}
		}
		
		void localVarList_BeforeExpand(object sender, TreeListViewCancelEventArgs e)
		{
			if (debuggedProcess.IsPaused) {
				if (e.Item is TreeListViewDebuggerItem) {
					((TreeListViewDebuggerItem)e.Item).BeforeExpand();
				}
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
				if (item is TreeListViewDebuggerItem) {
					((TreeListViewDebuggerItem)item).Update();
				}
				if (item.IsExpanded) UpdateSubTree(item);
			}
		}
	}
}
