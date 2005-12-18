// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Commands;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ShowBufferOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			OptionsCommand.ShowTabbedOptions("ShowTabbedOptions",
			                                 AddInTree.GetTreeNode("/SharpDevelop/ViewContent/DefaultTextEditor/OptionsDialog"));
		}
	}
	
	
	public class HighlightingTypeBuilder : ISubmenuBuilder
	{
		TextEditorControl  control      = null;
		ToolStripItem[] menuCommands = null;
		
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			control = (TextEditorControl)owner;
			
			ArrayList menuItems = new ArrayList();
			
			foreach (DictionaryEntry entry in HighlightingManager.Manager.HighlightingDefinitions) {
				MenuCheckBox item = new MenuCheckBox(entry.Key.ToString());
				item.Click    += new EventHandler(ChangeSyntax);
				item.Checked = control.Document.HighlightingStrategy.Name == entry.Key.ToString();
				menuItems.Add(item);
			}
			menuCommands = (ToolStripItem[])menuItems.ToArray(typeof(ToolStripItem));
			return menuCommands;
		}
		
		void ChangeSyntax(object sender, EventArgs e)
		{
			if (control != null) {
				MenuCheckBox item = (MenuCheckBox)sender;
				foreach (MenuCheckBox i in menuCommands) {
					i.Checked = false;
				}
				item.Checked = true;
				IHighlightingStrategy strat = HighlightingStrategyFactory.CreateHighlightingStrategy(item.Text);
				if (strat == null) {
					throw new Exception("Strategy can't be null");
				}
				control.Document.HighlightingStrategy = strat;
				control.Refresh();
			}
		}
	}
}
