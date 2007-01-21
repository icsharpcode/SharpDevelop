// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : DebuggerPad
	{
		class ItemIcon: NodeIcon {
			protected override System.Drawing.Image GetIcon(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Icon;
			}
		}
		
		class ItemName: NodeTextBox {
			protected override bool CanEdit(TreeNodeAdv node)
			{
				return false;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Name;
			}
		}
		
		class ItemText: NodeTextBox {
			protected override bool CanEdit(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).CanEditText;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Text;
			}
		}
		
		class ItemType: NodeTextBox {
			protected override bool CanEdit(TreeNodeAdv node)
			{
				return false;
			}
			public override object GetValue(TreeNodeAdv node)
			{
				return ((TreeViewNode)node).Type;
			}
		}
		
		TreeViewAdv localVarList;
		Debugger.Process debuggedProcess;
		
		TreeColumn nameColumn = new TreeColumn();
		TreeColumn valColumn  = new TreeColumn();
		TreeColumn typeColumn = new TreeColumn();
		
		public override Control Control {
			get {
				return localVarList;
			}
		}
		
		protected override void InitializeComponents()
		{
			localVarList = new TreeViewAdv();
			localVarList.Columns.Add(nameColumn);
			localVarList.Columns.Add(valColumn);
			localVarList.Columns.Add(typeColumn);
			localVarList.UseColumns = true;
			localVarList.SelectionMode = TreeSelectionMode.Single;
			localVarList.LoadOnDemand = true;
			localVarList.VisibleChanged += delegate { if (localVarList.Visible) RefreshPad(); };
			localVarList.SizeChanged += delegate { RefreshPad(); };
			
			localVarList.Expanding += delegate(object sender, TreeViewAdvEventArgs e) {
				if (e.Node is TreeViewNode) ((TreeViewNode)e.Node).OnExpanding();
			};
			localVarList.Expanded += delegate(object sender, TreeViewAdvEventArgs e) {
				if (e.Node is TreeViewNode) ((TreeViewNode)e.Node).OnExpanded();
			};
			localVarList.Collapsed += delegate(object sender, TreeViewAdvEventArgs e) {
				if (e.Node is TreeViewNode) ((TreeViewNode)e.Node).OnCollapsed();
			};
			
			NodeIcon iconControl = new ItemIcon();
			iconControl.ParentColumn = nameColumn;
			localVarList.NodeControls.Add(iconControl);
			
			NodeTextBox nameControl = new ItemName();
			nameControl.ParentColumn = nameColumn;
			localVarList.NodeControls.Add(nameControl);
			
			NodeTextBox textControl = new ItemText();
			textControl.ParentColumn = valColumn;
			localVarList.NodeControls.Add(textControl);
			
			NodeTextBox typeControl = new ItemType();
			typeControl.ParentColumn = typeColumn;
			localVarList.NodeControls.Add(typeControl);
			
			RedrawContent();
		}
		
		public override void RedrawContent()
		{
			nameColumn.Header = ResourceService.GetString("Global.Name");
			nameColumn.Width = 250;
			valColumn.Header  = ResourceService.GetString("Dialog.HighlightingEditor.Properties.Value");
			valColumn.Width = 300;
			typeColumn.Header = ResourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
			typeColumn.Width = 250;
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
			DateTime start = Debugger.Util.HighPrecisionTimer.Now;
			if (debuggedProcess != null && debuggedProcess.SelectedFunction != null) {
				TreeViewNode.UpdateNodes(localVarList, localVarList.Root.Children, new FunctionItem(debuggedProcess.SelectedFunction).SubItems);
			} else {
				TreeViewNode.UpdateNodes(localVarList, localVarList.Root.Children, new ListItem[0]);
			}
			DateTime end = Debugger.Util.HighPrecisionTimer.Now;
			LoggingService.InfoFormatted("Local Variables pad refreshed ({0} ms)", (end - start).TotalMilliseconds);
		}
	}
}
