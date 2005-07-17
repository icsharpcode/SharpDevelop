// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//	   <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
//using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

//using ICSharpCode.Core.Properties;

using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class BreakPointsPad : AbstractPadContent
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;

		ListView  breakpointsList;
		
		ColumnHeader name     = new ColumnHeader();
		ColumnHeader path     = new ColumnHeader();
		
		public override Control Control {
			get {
				return breakpointsList;
			}
		}
		
		public BreakPointsPad() //: base("${res:MainWindow.Windows.Debug.Breakpoints}", null)
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;

			breakpointsList = new ListView();
			breakpointsList.FullRowSelect = true;
			breakpointsList.AutoArrange = true;
			breakpointsList.Alignment   = ListViewAlignment.Left;
			breakpointsList.View = View.Details;
			breakpointsList.Dock = DockStyle.Fill;
			breakpointsList.GridLines  = false;
			breakpointsList.Activation = ItemActivation.OneClick;
			breakpointsList.CheckBoxes = true;
			breakpointsList.Columns.AddRange(new ColumnHeader[] {name, path} );
			breakpointsList.ItemCheck += new ItemCheckEventHandler(BreakpointsListItemCheck);
			
			name.Width = 300;
			path.Width = 400;

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

			debuggerCore.DebuggingResumed += new DebuggerEventHandler(debuggerService_OnDebuggingResumed);
			debuggerCore.BreakpointAdded += new DebuggerLibrary.BreakpointEventHandler(AddBreakpoint);
			debuggerCore.BreakpointStateChanged += new DebuggerLibrary.BreakpointEventHandler(RefreshBreakpoint);
			debuggerCore.BreakpointRemoved += new DebuggerLibrary.BreakpointEventHandler(RemoveBreakpoint);
			debuggerCore.BreakpointHit += new DebuggerLibrary.BreakpointEventHandler(Breakpoints_OnBreakpointHit);

			RefreshList();
		}
		
		public override void RedrawContent()
		{
			name.Text        = "Name";
			path.Text        = "Path";
		}
		
		void BreakpointsListItemCheck(object sender, ItemCheckEventArgs e)
		{
			DebuggerLibrary.Breakpoint breakpoint = breakpointsList.Items[e.Index].Tag as DebuggerLibrary.Breakpoint;
			if (breakpoint != null) {
				breakpoint.Enabled = (e.NewValue == CheckState.Checked);
			}
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.RedrawContent();
			}
		}
		
		void RefreshList()
		{
			breakpointsList.ItemCheck -= new ItemCheckEventHandler(BreakpointsListItemCheck);
			breakpointsList.BeginUpdate();
			breakpointsList.Items.Clear();
			foreach(DebuggerLibrary.Breakpoint b in debuggerCore.Breakpoints) {
				AddBreakpoint(new BreakpointEventArgs(b));
			}
			breakpointsList.EndUpdate();
			breakpointsList.ItemCheck += new ItemCheckEventHandler(BreakpointsListItemCheck);
		}
		
		void AddBreakpoint(object sender, BreakpointEventArgs e)
		{
			breakpointsList.ItemCheck -= new ItemCheckEventHandler(BreakpointsListItemCheck);
			AddBreakpoint(e);
			breakpointsList.ItemCheck += new ItemCheckEventHandler(BreakpointsListItemCheck);
		}
		
		void AddBreakpoint(BreakpointEventArgs e)
		{
			ListViewItem item = new ListViewItem();
			item.Tag = e.Breakpoint;
			breakpointsList.Items.Add(item);
			RefreshBreakpoint(item, e);
		}
		
		void RefreshBreakpoint(object sender, BreakpointEventArgs e)
		{
			breakpointsList.ItemCheck -= new ItemCheckEventHandler(BreakpointsListItemCheck);
			foreach (ListViewItem item in breakpointsList.Items) {
				if (e.Breakpoint == item.Tag) {
					RefreshBreakpoint(item, e);
					break;
				}
			}
			breakpointsList.ItemCheck += new ItemCheckEventHandler(BreakpointsListItemCheck);
		}
		
		void RefreshBreakpoint(ListViewItem item, BreakpointEventArgs e)
		{
			item.SubItems.Clear();
			item.Checked = e.Breakpoint.Enabled;
			item.Text = Path.GetFileName(e.Breakpoint.SourcecodeSegment.SourceFullFilename) + ", Line = " + e.Breakpoint.SourcecodeSegment.StartLine.ToString();
			item.ForeColor = e.Breakpoint.HadBeenSet ? Color.Black : Color.Gray;
			item.SubItems.AddRange(new string[] { Path.GetDirectoryName(e.Breakpoint.SourcecodeSegment.SourceFullFilename) });
		}
		
		void RemoveBreakpoint(object sender, BreakpointEventArgs e)
		{
			foreach (ListViewItem item in breakpointsList.Items) {
				if (e.Breakpoint == item.Tag) {
					item.Remove();
				}
			}
		}
		
		void Breakpoints_OnBreakpointHit(object sender, BreakpointEventArgs e)
		{
			foreach (ListViewItem item in breakpointsList.Items) {
				if (e.Breakpoint == item.Tag) {
					item.BackColor = System.Drawing.Color.DarkRed;
					item.ForeColor = System.Drawing.Color.White;
				}
			}
		}
		
		void debuggerService_OnDebuggingResumed(object sender, DebuggerEventArgs e)
		{
			breakpointsList.BeginUpdate();
			foreach(DebuggerLibrary.Breakpoint b in debuggerCore.Breakpoints)
				RefreshBreakpoint(this, new BreakpointEventArgs(b));
			breakpointsList.EndUpdate();
		}
	}
}
