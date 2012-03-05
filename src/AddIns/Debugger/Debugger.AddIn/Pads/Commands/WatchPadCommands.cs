// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Linq;
using Debugger.AddIn.Pads;
using Debugger.AddIn.Pads.Controls;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
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
				var result = inputWindow.ShowDialog();
				if (!result.HasValue || !result.Value)
					return;
				
				string input = inputWindow.CommandText;
				
				if (!string.IsNullOrEmpty(input)) {
					// get language
					if (ProjectService.CurrentProject == null) return;
					
					string language = ProjectService.CurrentProject.Language;
					
					var text = new TreeNode(input, null).ToSharpTreeNode();
					var list = pad.WatchList;
					
					if(!list.WatchItems.Any(n => text.Node.Name == ((TreeNodeWrapper)n).Node.Name))
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
}
