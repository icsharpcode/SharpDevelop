// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2039 $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Debugger.AddIn.Service;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Debugger.AddIn
{
	public class IsBreakpointCondition : IConditionEvaluator
	{
		public IsBreakpointCondition()
		{
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null)
				return false;
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			if (provider == null)
				return false;
			if (string.IsNullOrEmpty(provider.TextEditorControl.FileName))
				return false;
			
			foreach (BreakpointBookmark mark in DebuggerService.Breakpoints) {
				if ((mark.FileName == provider.TextEditorControl.FileName) &&
				    (mark.LineNumber == provider.TextEditorControl.ActiveTextAreaControl.Caret.Line))
					return true;
			}
			
			return false;
		}
	}
	
	public class IsActiveBreakpointCondition : IConditionEvaluator
	{
		public IsActiveBreakpointCondition()
		{
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null)
				return false;
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			if (provider == null)
				return false;
			if (string.IsNullOrEmpty(provider.TextEditorControl.FileName))
				return false;
			
			BreakpointBookmark point = null;
			
			foreach (BreakpointBookmark breakpoint in DebuggerService.Breakpoints) {
				if ((breakpoint.FileName == provider.TextEditorControl.FileName) &&
				    (breakpoint.LineNumber == provider.TextEditorControl.ActiveTextAreaControl.Caret.Line)) {
					point = breakpoint;
					break;
				}
			}
			
			if (point != null) {
				return point.IsEnabled;
			}
			
			return false;
		}
	}
	
	public class EnableBreakpointMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			
			BreakpointBookmark point = null;
			
			foreach (BreakpointBookmark breakpoint in DebuggerService.Breakpoints) {
				if ((breakpoint.FileName == provider.TextEditorControl.FileName) &&
				    (breakpoint.LineNumber == provider.TextEditorControl.ActiveTextAreaControl.Caret.Line)) {
					point = breakpoint;
					break;
				}
			}
			
			if (point != null) {
				point.IsEnabled = true;
			}
		}
	}
	
	public class DisableBreakpointMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			
			BreakpointBookmark point = null;
			
			foreach (BreakpointBookmark breakpoint in DebuggerService.Breakpoints) {
				if ((breakpoint.FileName == provider.TextEditorControl.FileName) &&
				    (breakpoint.LineNumber == provider.TextEditorControl.ActiveTextAreaControl.Caret.Line)) {
					point = breakpoint;
					break;
				}
			}
			
			if (point != null) {
				point.IsEnabled = false;
			}
		}
	}
	
	public class BreakpointChangeMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			List<ToolStripItem> items = new List<ToolStripItem>();
			
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			
			BreakpointBookmark point = null;
			
			foreach (BreakpointBookmark breakpoint in DebuggerService.Breakpoints) {
				if ((breakpoint.FileName == provider.TextEditorControl.FileName) &&
				    (breakpoint.LineNumber == provider.TextEditorControl.ActiveTextAreaControl.Caret.Line)) {
					point = breakpoint;
					break;
				}
			}
			
			if (point != null) {
				foreach (string item in BreakpointAction.GetNames(typeof(BreakpointAction))) {
					items.Add(MakeItem("${res:MainWindow.Windows.Debug.Conditional.Breakpoints." + item + "}", item, point, point.Action.ToString(), delegate(object sender, EventArgs e) {HandleItem(sender);}));
				}
			}
			
			return items.ToArray();
		}
		
		void HandleItem(object sender)
		{
			ToolStripMenuItem item = null;
			if (sender is ToolStripMenuItem)
				item = (ToolStripMenuItem)sender;
			
			if (item != null) {
				BreakpointBookmark bookmark = (BreakpointBookmark)item.Tag;
				
				switch (item.Name) {
					case "Break":
						bookmark.Action = BreakpointAction.Break;
						break;
					case "Condition":
						EditBreakpointScriptForm form = new EditBreakpointScriptForm(bookmark);
						
						if (form.ShowDialog() == DialogResult.OK) {
							bookmark = form.Data;
						}
						break;
					case "Trace":
						bookmark.Action = BreakpointAction.Trace;
						break;
				}
			}
		}
		
		ToolStripMenuItem MakeItem(string title, string name, BreakpointBookmark tag, string data, EventHandler onClick)
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem(StringParser.Parse(title));
			menuItem.Click += onClick;
			menuItem.Name = name;
			menuItem.Tag = tag;
			
			if (name == tag.Action.ToString())
				menuItem.Checked = true;
			
			return menuItem;
		}
	}
}
