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

using Debugger;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : AbstractPadContent
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;

		TreeListView localVarList;
		
		ColumnHeader name = new ColumnHeader();
		ColumnHeader val  = new ColumnHeader();
		ColumnHeader type = new ColumnHeader();
		
		public override Control Control {
			get {
				return localVarList;
			}
		}
		
		public LocalVarPad() //: base("${res:MainWindow.Windows.Debug.Local}", null)
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			ImageList imageList = new ImageList();
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Class"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Field"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Property"));

			//iconsService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			localVarList = new TreeListView();
			localVarList.SmallImageList = imageList;
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
			
			debugger.DebugStopped += OnDebugStopped;
			
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

			debuggerCore.DebuggingPaused += new EventHandler<DebuggingPausedEventArgs>(debuggerService_OnDebuggingPaused);

			RefreshList();
		}

		// This is a walkarond for a visual issue
		void localVarList_SizeChanged(object sender, EventArgs e)
		{
			localVarList.Visible = true;
		}
		
		public override void RedrawContent()
		{
			name.Text = "Name";
			val.Text  = "Value";
			type.Text = "Type";
		}
		
		void OnDebugStopped(object sender, EventArgs e)
		{
			localVarList.Items.Clear();
		}

		private void debuggerService_OnDebuggingPaused(object sender, DebuggingPausedEventArgs e)
		{
			RefreshList();
		}

		void RefreshList()
		{
			UpdateVariables(localVarList.Items, debuggerCore.LocalVariables);
		}

		private void localVarList_BeforeExpand(object sender, TreeListViewCancelEventArgs e)
		{
			if (debuggerCore.IsPaused) {
				((VariableListItem)e.Item).PrepareForExpansion();
			} else {
				MessageBox.Show("You can not explore variables while the debuggee is running.");
				e.Cancel = true;
			}
		}

		static VariableItem FindVariableItem(TreeListViewItemCollection items, Variable variable)
		{
			foreach (VariableListItem item in items) {
				VariableItem variableItem = item as VariableItem;
				if (variableItem != null && variableItem.Variable.Name == variable.Name) {
					return variableItem;
				}
			}
			return null;
		}

		public static void UpdateVariables(TreeListViewItemCollection items, VariableCollection variables)
		{
			// Add new variables and refresh existing ones
			foreach (Variable variable in variables) {
				VariableItem item = FindVariableItem(items, variable);
				if (item != null) {
					item.Variable = variable;
					item.Refresh();
				} else {
					item = new VariableItem(variable);
					if (item.IsValid) {
						items.Add(item);
					}
				}
			}

			// Delete invalid or removed variables
			List<VariableListItem> toBeRemoved = new List<VariableListItem>();
			foreach (VariableListItem item in items) {
				if (!item.IsValid) {
					toBeRemoved.Add(item);
					continue;
				}

				VariableItem variableItem = item as VariableItem;
				if (variableItem != null && !(item is BaseClassItem) && !variables.Contains(variableItem.Variable.Name)) {
					toBeRemoved.Add(item);
				}
			}
			foreach (VariableListItem item in toBeRemoved) {
				item.Remove();
			}
		}
	}
}
