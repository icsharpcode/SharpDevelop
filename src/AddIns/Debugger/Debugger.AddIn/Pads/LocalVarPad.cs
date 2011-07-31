// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.ObjectModel;
using Debugger;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using Exception = System.Exception;
using TreeNode = Debugger.AddIn.TreeModel.TreeNode;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : DebuggerPad
	{
		WatchList localVarList;
		Process debuggedProcess;
		static LocalVarPad instance;
		
		public LocalVarPad()
		{
			instance = this;
		}
		
		/// <remarks>Always check if Instance is null, might be null if pad is not opened!</remarks>
		public static LocalVarPad Instance {
			get { return instance; }
		}
		
		public Process Process {
			get { return debuggedProcess; }
		}
		
		protected override void InitializeComponents()
		{
			localVarList = new WatchList();
			localVarList.WatchType = WatchListType.LocalVar;
			panel.Children.Add(localVarList);
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
				localVarList.WatchItems.Clear();
				return;
			}
			localVarList.WatchItems.Clear();
			using(new PrintTimes("Local Variables refresh")) {
				try {
					Utils.DoEvents(debuggedProcess);
					foreach (var item in new StackFrameNode(debuggedProcess.SelectedStackFrame).ChildNodes) {
						localVarList.WatchItems.Add(item);
					}
				} 
				catch(AbortedBecauseDebuggeeResumedException) { } 
				catch(Exception ex) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						MessageService.ShowException(ex);
					}
				}
			}
		}
	}
	
	public static class ExtensionForWatchItems
	{
		public static bool ContainsItem(this ObservableCollection<TreeNode> collection, TreeNode node)
		{
			foreach (var item in collection)
				if (item.CompareTo(node) == 0)
					return true;
			
			return false;
		}
	}
}
