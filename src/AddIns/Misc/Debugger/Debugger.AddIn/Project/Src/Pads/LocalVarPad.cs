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
		
		
		protected override void RegisterDebuggerEvents()
		{
			debuggerCore.DebuggeeStateChanged += delegate { debuggerCore.LocalVariables.Update(); };
		}
		
		public override void RefreshPad()
		{
			localVarList.BeginUpdate();
			localVarList.Items.Clear();
			AddVariableCollectionToTree(debuggerCore.LocalVariables, localVarList.Items);
			localVarList.EndUpdate();
		}
		
		public static void AddVariableCollectionToTree(VariableCollection varCollection, TreeListViewItemCollection tree)
		{
			varCollection.VariableAdded += delegate(object sender, VariableEventArgs e) {
				AddVariableToTree(e.Variable, tree);
			};
			
			List<Variable> publicStatic = new List<Variable>();
			List<Variable> publicInstance = new List<Variable>();
			List<Variable> privateStatic = new List<Variable>();
			List<Variable> privateInstance = new List<Variable>();
			
			foreach(Variable variable in varCollection) {
				ClassVariable classVariable = variable as ClassVariable;
				if (classVariable == null) {
					publicInstance.Add(variable);
				} else {
					if (classVariable.IsPublic) {
						if (classVariable.IsStatic) {
							publicStatic.Add(variable);
						} else {
							publicInstance.Add(variable);
						}
					} else {
						if (classVariable.IsStatic) {
							privateStatic.Add(variable);
						} else {
							privateInstance.Add(variable);
						}
					}
				}
			}
			
			if (privateInstance.Count > 0) {
				AddMenu("<Private Members>", privateInstance, tree);
			}
			if (publicStatic.Count > 0) {
				AddMenu("<Public Static Members>", publicStatic, tree);
			}
			if (privateStatic.Count > 0) {
				AddMenu("<Private Static Members>", privateStatic, tree);
			}
			// Public Members
			foreach(Variable variable in publicInstance) {
				AddVariableToTree(variable, tree);
			}
		}
		
		public static void AddMenu(string name, List<Variable> variables, TreeListViewItemCollection tree)
		{
			TreeListViewItem menu = new TreeListViewItem(name, 0);
			tree.Add(menu);
			foreach(Variable variable in variables) {
				AddVariableToTree(variable, menu.Items);
			}
		}
		
		public static void AddVariableToTree(Variable variableToAdd, TreeListViewItemCollection tree)
		{
			TreeListViewDebuggerItem newItem = new TreeListViewDebuggerItem(variableToAdd);
			
			tree.Add(newItem);
		}

		private void localVarList_BeforeExpand(object sender, TreeListViewCancelEventArgs e)
		{
			if (debuggerCore.IsPaused) {
				if (e.Item is TreeListViewDebuggerItem) {
					((TreeListViewDebuggerItem)e.Item).BeforeExpand();
				}
			} else {
				MessageBox.Show("You can not explore variables while the debuggee is running.");
				e.Cancel = true;
			}
		}
	}
}
