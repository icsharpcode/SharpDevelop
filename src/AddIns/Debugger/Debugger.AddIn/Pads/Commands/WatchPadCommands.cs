// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Debugger.AddIn.Pads;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Project;

namespace Debugger.AddIn
{
	public class AddWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				
				var inputWindow = new WatchInputBox(StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.AddWatch}"),
				                                    StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.EnterExpression}"));
				inputWindow.Owner = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWindow;
				if (inputWindow.ShowDialog() != true)
					return;
				
				string input = inputWindow.CommandText;
				
				if (!string.IsNullOrEmpty(input)) {
					var text = new TextNode(null, input, inputWindow.ScriptLanguage).ToSharpTreeNode();
					var list = pad.WatchList;
					
					if(!list.WatchItems.Any(n => text.Node.FullName == ((TreeNodeWrapper)n).Node.FullName))
						list.WatchItems.Add(text);
				}
				
				pad.InvalidatePad();
			}
		}
	}
	
	public class RemoveWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				var list = pad.WatchList;
				var node = list.SelectedNode;
				
				if (node == null)
					return;
				
				list.WatchItems.Remove(node);
				((WatchPad)this.Owner).InvalidatePad();
			}
		}
	}
	
	public class RefreshWatchesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				((WatchPad)this.Owner).InvalidatePad();
			}
		}
	}
	
	public class ClearWatchesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				var list =  pad.WatchList;
				list.WatchItems.Clear();
			}
		}
	}
	
	public class CopyToClipboardCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				var node =  pad.WatchList.SelectedNode;
				if (node != null && node.Node is ExpressionNode) {
					string text = ((ExpressionNode)node.Node).FullText;
					ClipboardWrapper.SetText(text);
				}
			}
		}
	}
	
	public class WatchScriptingLanguageMenuBuilder : ISubmenuBuilder, IMenuItemBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			List<ToolStripItem> items = new List<ToolStripItem>();
			
			if (owner is WatchPad) {
				WatchPad pad = (WatchPad)owner;
				
				if (pad.WatchList.SelectedNode == null)
					return items.ToArray();
				
				var node = pad.WatchList.SelectedNode.Node;
				
				while (node.Parent != null && node.Parent.Parent != null)
				{
					node = node.Parent;
				}
				
				if (!(node is TextNode))
					return items.ToArray();
				
				foreach (string item in SupportedLanguage.GetNames(typeof(SupportedLanguage))) {
					items.Add(MakeItem(item, item, node as TextNode, (sender, e) => HandleItem(sender)));
				}
			}
			
			return items.ToArray();
		}
		
		ToolStripMenuItem MakeItem(string title, string name, TextNode tag, EventHandler onClick)
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem(StringParser.Parse(title));
			menuItem.Click += onClick;
			menuItem.Name = name;
			menuItem.Tag = tag;
			
			if (name == tag.Language.ToString())
				menuItem.Checked = true;
			
			return menuItem;
		}
		
		
		void HandleItem(object sender)
		{
			ToolStripMenuItem item = null;
			if (sender is ToolStripMenuItem)
				item = (ToolStripMenuItem)sender;
			
			if (item != null) {
				TextNode node = (TextNode)item.Tag;
				node.Language = (SupportedLanguage)SupportedLanguage.Parse(typeof(SupportedLanguage), item.Text);
			}
		}
		
		public System.Collections.ICollection BuildItems(Codon codon, object owner)
		{
			return BuildSubmenu(codon, owner).TranslateToWpf();
		}
	}
}
