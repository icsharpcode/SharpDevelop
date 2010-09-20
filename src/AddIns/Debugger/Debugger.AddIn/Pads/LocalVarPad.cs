// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Debugger;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using Exception=System.Exception;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : DebuggerPad
	{
		TreeViewAdv localVarList;
		Process debuggedProcess;
		static LocalVarPad instance;
		
		readonly TreeColumn nameColumn = new TreeColumn();
		readonly TreeColumn valColumn  = new TreeColumn();
		readonly TreeColumn typeColumn = new TreeColumn();
		
		public LocalVarPad()
		{
			instance = this;
		}
		
		/// <remarks>Always check if Instance is null, might be null if pad is not opened!</remarks>
		public static LocalVarPad Instance {
			get { return instance; }
		}
		
		/// <remarks>
		/// This is not used anywhere, but it is neccessary to be overridden in children of AbstractPadContent.
		/// </remarks>
		public override object Control {
			get {
				return localVarList;
			}
		}
		
		public Process Process {
			get { return debuggedProcess; }
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
			
			localVarList.AutoRowHeight = true;
			
			RedrawContent();
			ResourceService.LanguageChanged += delegate { RedrawContent(); };
		}
		
		public void RedrawContent()
		{
			nameColumn.Header = ResourceService.GetString("Global.Name");
			nameColumn.Width = 250;
			valColumn.Header  = ResourceService.GetString("Dialog.HighlightingEditor.Properties.Value");
			valColumn.Width = 300;
			typeColumn.Header = ResourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
			typeColumn.Width = 250;
		}
		
		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= debuggedProcess_Paused;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += debuggedProcess_Paused;
			}
			RefreshPad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		public override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedStackFrame == null) {
				localVarList.Root.Children.Clear();
				return;
			}
			
			using(new PrintTimes("Local Variables refresh")) {
				try {
					localVarList.BeginUpdate();
					Utils.DoEvents(debuggedProcess);
					TreeViewVarNode.SetContentRecursive(debuggedProcess, localVarList, new StackFrameNode(debuggedProcess.SelectedStackFrame).ChildNodes);
				} catch(AbortedBecauseDebuggeeResumedException) {
				} catch(Exception) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						throw;
					}
				} finally {
					localVarList.EndUpdate();
				}
			}
		}
	}
}
