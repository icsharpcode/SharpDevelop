// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2039 $</version>
// </file>
using ICSharpCode.SharpDevelop.Gui.Pads;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Debugger;
using Debugger.AddIn;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;

namespace Debugger.AddIn
{
	public class AddWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
				
				((TreeViewAdv)pad.Content).BeginUpdate();
				TextNode text = new TextNode(MessageService.ShowInputBox(StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.AddWatch}"),
				                                                         StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.EnterExpression}"),
				                                                         ""));
				TreeViewVarNode node = new TreeViewVarNode(pad.Process, (TreeViewAdv)pad.Content, text);		
				
				pad.Watches.Add(text);
				((TreeViewAdv)pad.Content).Root.Children.Add(node);
				((TreeViewAdv)pad.Content).EndUpdate();
				
				((WatchPad)this.Owner).RefreshPad();
			}
		}
	}
	
	public class RemoveWatchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (this.Owner is WatchPad) {
				WatchPad pad = (WatchPad)this.Owner;
								
				// TODO : Implement remove
				
				TreeNodeAdv node = ((TreeViewAdv)pad.Content).SelectedNode;
				
				if (node == null)
					return;
				
				while (node.Parent != ((TreeViewAdv)pad.Content).Root)
				{
					node = node.Parent;
				}
				
				pad.Watches.RemoveAt(node.Index);
				((TreeViewAdv)pad.Content).Root.Children.Remove(node);
				
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
				
				((TreeViewAdv)pad.Content).BeginUpdate();					
				pad.Watches.Clear();
				((TreeViewAdv)pad.Content).Root.Children.Clear();
				((TreeViewAdv)pad.Content).EndUpdate();
			}
		}
	}
}
