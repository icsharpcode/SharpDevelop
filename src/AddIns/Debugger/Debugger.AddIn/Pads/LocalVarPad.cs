// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using Debugger;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
using Exception = System.Exception;
using TreeNode = Debugger.AddIn.TreeModel.TreeNode;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : AbstractPadContent
	{
		DockPanel panel;
		WatchList localVarList;
		static LocalVarPad instance;
		
		public override object Control {
			get { return panel; }
		}
		
		public LocalVarPad()
		{
			this.panel = new DockPanel();
			instance = this;
			
			localVarList = new WatchList(WatchListType.LocalVar);
			panel.Children.Add(localVarList);
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}
		
		/// <remarks>Always check if Instance is null, might be null if pad is not opened!</remarks>
		public static LocalVarPad Instance {
			get { return instance; }
		}
		
		void RefreshPad()
		{
			StackFrame frame = WindowsDebugger.CurrentStackFrame;
			
			if (frame == null) {
				localVarList.WatchItems.Clear();
			} else {
				localVarList.WatchItems.Clear();
				frame.Process.EnqueueForEach(
					Dispatcher.CurrentDispatcher,
					ValueNode.GetLocalVariables().ToList(),
					n => localVarList.WatchItems.Add(n.ToSharpTreeNode())
				);
			}
		}
	}
}
