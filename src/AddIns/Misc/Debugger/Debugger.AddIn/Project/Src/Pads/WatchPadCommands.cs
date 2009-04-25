// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2039 $</version>
// </file>
using Aga.Controls.Tree;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
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
					TextNode text = new TextNode(input);
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
}
