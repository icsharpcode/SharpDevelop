// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Windows;
using System.Windows.Controls;
using Debugger;
using Debugger.AddIn.Pads.Controls;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class BreakPointsPad : BookmarkPadBase
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;
		
		public BreakPointsPad()
		{
			InitializeComponents();
			
			myPanel.Children.Add(CreateToolBar());
			
			CreateColumns();
		}
		
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;

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
		}
		
		protected override ToolBar CreateToolBar()
		{
			ToolBar toolbar = ToolBarService.CreateToolBar(myPanel, this, "/SharpDevelop/Pads/BreakpointPad/Toolbar");
			toolbar.SetValue(Grid.RowProperty, 0);
			return toolbar;
		}
		
		protected override void CreateColumns() 
		{
			string conditionHeader = StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.ConditionalColumnHeader}");
			
			// HACK
			DataTemplate cellTemplate = new ConditionCell().FindResource("ConditionCellTemplate") as DataTemplate;
			
			listView.AddColumn(conditionHeader, cellTemplate);
		}
		
		protected override bool ShowBookmarkInThisPad(SDBookmark mark)
		{
			return mark.IsVisibleInBookmarkPad && mark is BreakpointBookmark;
		}
	}
}
