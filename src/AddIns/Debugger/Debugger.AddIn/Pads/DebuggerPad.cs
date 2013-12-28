// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Windows.Controls;
using Debugger;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public abstract class DebuggerPad : AbstractPadContent
	{
		protected DockPanel panel;
		ToolBar toolbar;
		protected WindowsDebugger debugger;
		
		public override object Control {
			get {
				return panel;
			}
		}
		
		public DebuggerPad()
		{
			// UI
			this.panel = new DockPanel();
			this.toolbar = BuildToolBar();
			
			if (this.toolbar != null) {
				this.toolbar.SetValue(DockPanel.DockProperty, Dock.Top);
				
				this.panel.Children.Add(toolbar);
			}
			
			// logic
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			InitializeComponents();
			
			debugger.ProcessSelected += delegate(object sender, ProcessEventArgs e) {
				SelectProcess(e.Process);
			};
			SelectProcess(debugger.DebuggedProcess);
		}
		
		protected virtual void InitializeComponents()
		{
			
		}
		
		protected virtual void SelectProcess(Process process)
		{
			
		}
		
		/// <summary>
		/// Never call this directly. Always use InvalidatePad()
		/// </summary>
		protected virtual void RefreshPad()
		{
			
		}
		
		bool invalidatePadEnqueued;
		
		public void InvalidatePad()
		{
			WorkbenchSingleton.AssertMainThread();
			if (invalidatePadEnqueued || WorkbenchSingleton.Workbench == null)
				return;
			invalidatePadEnqueued = true;
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					invalidatePadEnqueued = false;
					RefreshPad();
				});
			
		}
		
		protected virtual ToolBar BuildToolBar()
		{
			return null;
		}
	}
}
