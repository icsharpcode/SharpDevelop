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
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using Debugger;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : DebuggerPad
	{
		DebuggerTreeListView localVarList;
		
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
		
		
		protected override void RegisterDebuggerEvents()
		{
			debuggerCore.DebuggeeStateChanged += delegate { RefreshPad(); };
		}
		
		public override void RefreshPad()
		{
			localVarList.BeginUpdate();
			localVarList.Items.Clear();
			AddVariableCollectionToTree(debuggerCore.LocalVariables, localVarList.Items);
			localVarList.EndUpdate();
		}
		
		delegate void AddVariableMethod(Variable variable);
		
		public static void AddVariableCollectionToTree(VariableCollection varCollection, TreeListViewItemCollection tree)
		{
			TreeListViewItem privateInstanceMenu = new TreeListViewItem(privateMembersName, 0);
			TreeListViewItem staticMenu = new TreeListViewItem(staticMembersName, 0);
			TreeListViewItem privateStaticMenu = new TreeListViewItem(privateStaticMembersName, 0);
			
			foreach(Variable variable in varCollection) {
				if (variable.IsPublic) {
					if (variable.IsStatic) {
						// Public static
						if (staticMenu.TreeListView == null) {
							tree.Add(staticMenu);
							tree.Sort(false);
						}
						staticMenu.Items.Add(new TreeListViewDebuggerItem(variable));
					} else {
						// Public instance
						tree.Add(new TreeListViewDebuggerItem(variable));
					}
				} else {
					if (variable.IsStatic) {
						// Private static
						if (staticMenu.TreeListView == null) {
							tree.Add(staticMenu);
							tree.Sort(false);
						}
						if (privateStaticMenu.TreeListView == null) {
							staticMenu.Items.Add(privateStaticMenu);
							staticMenu.Items.Sort(false);
						}
						privateStaticMenu.Items.Add(new TreeListViewDebuggerItem(variable));
					} else {
						// Private instance
						if (privateInstanceMenu.TreeListView == null) {
							tree.Add(privateInstanceMenu);
							tree.Sort(false);
						}
						privateInstanceMenu.Items.Add(new TreeListViewDebuggerItem(variable));
					}
				}
			}
		}
		
		void localVarList_BeforeExpand(object sender, TreeListViewCancelEventArgs e)
		{
			if (debuggerCore.IsPaused) {
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
