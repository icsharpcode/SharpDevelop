// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.NRefactory;
using System;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Debugger.AddIn.TreeModel;
using TreeNode = Debugger.AddIn.TreeModel.TreeNode;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class TextNode : Debugger.AddIn.TreeModel.TreeNode, ISetText
	{
		public TextNode(string text, SupportedLanguage language)
		{
			this.Name = text;
			this.Language = language;
		}
		
		public bool CanSetText {
			get {
				return true;
			}
		}
		
		public bool SetText(string text)
		{
			this.Text = text;
			return true;
		}
		
		public bool SetName(string name)
		{
			this.Name = name;
			return true;
		}
		
		public SupportedLanguage Language { get; set; }
	}
	
	public class ErrorInfoNode : ICorDebug.InfoNode
	{
		public ErrorInfoNode(string name, string text) : base(name, text)
		{
			IconImage = DebuggerResourceService.GetImage("Icons.16x16.Error");
		}
	}
	
	public sealed class WatchItemName: NodeTextBox {
		public WatchItemName()
		{
			this.EditEnabled = true;
			this.EditOnClick = true;
		}
		protected override bool CanEdit(TreeNodeAdv node)
		{
			TreeNode content = ((TreeViewVarNode)node).Content;
			return (content is ISetText) && ((ISetText)content).CanSetText;
		}
		public override object GetValue(TreeNodeAdv node)
		{
			if (node is TreeViewVarNode) {
				return ((TreeViewVarNode)node).Content.Name;
			} else {
				// Happens during incremental search
				return base.GetValue(node);
			}
		}
		public override void SetValue(TreeNodeAdv node, object value)
		{
			if (string.IsNullOrEmpty(value as string))
				MessageBox.Show("You can not set name to an empty string!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
			{
				if (((TreeViewVarNode)node).Content is ExpressionNode) {
					WatchPad.Instance.Watches.RemoveAll(item => item.Name == ((ExpressionNode)((TreeViewVarNode)node).Content).Name);
					((ExpressionNode)((TreeViewVarNode)node).Content).Name = value.ToString();
				} else {
					if (((TreeViewVarNode)node).Content is TextNode) {
						WatchPad.Instance.Watches.RemoveAll(item => item.Name == ((TextNode)((TreeViewVarNode)node).Content).Name);
						((TextNode)((TreeViewVarNode)node).Content).Name = value.ToString();
					}
				}
				
				WatchPad.Instance.Watches.Add(new TextNode(value as string, SupportedLanguage.CSharp));
			}
		}
		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			if (args.Node == null) {
				base.MouseDown(args);
				return;
			}
			TreeNode content = ((TreeViewVarNode)args.Node).Content;
			if (content is IContextMenu && args.Button == MouseButtons.Right) {
				ContextMenuStrip menu = ((IContextMenu)content).GetContextMenu();
				if (menu != null) {
					menu.Show(args.Node.Tree, args.Location);
					args.Handled = true;
				}
			} else {
				base.MouseDown(args);
			}
		}
	}
}
