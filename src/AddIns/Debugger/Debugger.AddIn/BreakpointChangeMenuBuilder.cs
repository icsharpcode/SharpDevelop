// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.SharpDevelop.Gui.Pads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Debugger.AddIn.Service;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Debugger.AddIn
{
	public class BreakpointChangeMenuBuilder : ISubmenuBuilder, IMenuItemBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			List<ToolStripItem> items = new List<ToolStripItem>();
			
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorProvider;
			
			BreakpointBookmark point = null;
			
			foreach (BreakpointBookmark breakpoint in DebuggerService.Breakpoints) {
				if ((breakpoint.FileName == provider.TextEditor.FileName) &&
				    (breakpoint.LineNumber == provider.TextEditor.Caret.Line)) {
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
		
		public ICollection BuildItems(Codon codon, object owner)
		{
			return BuildSubmenu(codon, owner).TranslateToWpf();
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
						EditBreakpointScriptWindow window = new EditBreakpointScriptWindow(bookmark) {
							Owner = WorkbenchSingleton.MainWindow
						};
						
						if (window.ShowDialog() ?? false) {
							bookmark = window.Data;
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
