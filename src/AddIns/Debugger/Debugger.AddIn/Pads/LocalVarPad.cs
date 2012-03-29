// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
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
			localVarList = new WatchList(WatchListType.LocalVar);
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
			InvalidatePad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			InvalidatePad();
		}
		
		protected override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning) {
				localVarList.WatchItems.Clear();
				return;
			}
			
			LoggingService.Info("Local Variables refresh");
			try {
				StackFrame frame = debuggedProcess.GetCurrentExecutingFrame();
				localVarList.WatchItems.Clear();
				if (frame == null) return;
				
				debuggedProcess.EnqueueForEach(
					Dispatcher.CurrentDispatcher,
					new StackFrameNode(frame).ChildNodes.ToList(),
					n => localVarList.WatchItems.Add(n.ToSharpTreeNode())
				);
			} catch (Exception ex) {
				if (debuggedProcess == null || debuggedProcess.HasExited) {
					// Process unexpectedly exited
				} else {
					MessageService.ShowException(ex);
				}
			}
		}
	}
}
