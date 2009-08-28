// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2039 $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Aga.Controls.Tree;
using Debugger.AddIn.TreeModel;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace Debugger.AddIn
{
	public class AddWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				TreeViewAdv ctrl = (TreeViewAdv)pad.Control;
				
				string input = MessageService.ShowInputBox(StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.AddWatch}"),
				                                           StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.EnterExpression}"),
				                                           "");
				if (!string.IsNullOrEmpty(input)) {
					ctrl.BeginUpdate();
					TextNode text = new TextNode(input, SupportedLanguage.CSharp);
					TreeViewVarNode node = new TreeViewVarNode(pad.Process, ctrl, text);
					
					pad.Watches.Add(text);
					ctrl.Root.Children.Add(node);
					ctrl.EndUpdate();
				}
				
				pad.RefreshPad();
			}
		}
	}
	
	public class RemoveWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;

				TreeNodeAdv node = ((TreeViewAdv)pad.Control).SelectedNode;
				
				if (node == null)
					return;
				
				while (node.Parent != ((TreeViewAdv)pad.Control).Root)
				{
					node = node.Parent;
				}
				
				pad.Watches.RemoveAt(node.Index);
				((TreeViewAdv)pad.Control).Root.Children.Remove(node);
				
				((WatchPad)this.Owner).RefreshPad();
			}
		}
	}
	
	public class RefreshWatchesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				((WatchPad)this.Owner).RefreshPad();
			}
		}
	}
	
	public class ClearWatchesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				
				((TreeViewAdv)pad.Control).BeginUpdate();
				pad.Watches.Clear();
				((TreeViewAdv)pad.Control).Root.Children.Clear();
				((TreeViewAdv)pad.Control).EndUpdate();
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

				TreeViewVarNode node = ((TreeViewAdv)pad.Control).SelectedNode as TreeViewVarNode;
				
				if (node == null)
					return items.ToArray();
				
				while (node.Parent != ((TreeViewAdv)pad.Control).Root)
				{
					node = node.Parent as TreeViewVarNode;
				}
				
				if (!(node.Content is TextNode))
					return items.ToArray();
				
				foreach (string item in SupportedLanguage.GetNames(typeof(SupportedLanguage))) {
					items.Add(MakeItem(item, item, node.Content as TextNode, (sender, e) => HandleItem(sender)));
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