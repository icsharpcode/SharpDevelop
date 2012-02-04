// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows;

using Debugger.AddIn.TreeModel;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace Debugger.AddIn.Pads.Controls
{
	public class TreeNodeWrapper : SharpTreeNode
	{
		public TreeNodeWrapper(TreeNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			Node = node;
			LazyLoading = true;
		}
		
		public TreeNode Node { get; private set; }
		
		public override bool Equals(object obj)
		{
			var w = obj as TreeNodeWrapper;
			return w != null && Node.Equals(w.Node);
		}
		
		public override int GetHashCode()
		{
			return Node.GetHashCode();
		}
		
		public override object Text {
			get { return Node.Name; }
		}
		
		public override object Icon {
			get { return Node.ImageSource; }
		}
		
		public override bool ShowExpander {
			get { return Node.HasChildNodes; }
		}
		
		protected override void LoadChildren()
		{
			if (Node.HasChildNodes) {
				this.Children.AddRange(Node.ChildNodes.Select(node => node.ToSharpTreeNode()));
			}
		}
	}
	
	public class WatchRootNode : SharpTreeNode
	{
		public override bool CanDrop(System.Windows.DragEventArgs e, int index)
		{
			e.Effects = DragDropEffects.None;
			if (e.Data.GetDataPresent(DataFormats.StringFormat)) {
				e.Effects = DragDropEffects.Copy;
				return true;
			}
			return false;
		}
		
		public override void Drop(DragEventArgs e, int index)
		{
			if (ProjectService.CurrentProject == null) return;
			if (e.Data == null) return;
			if (!e.Data.GetDataPresent(DataFormats.StringFormat)) return;
			if (string.IsNullOrEmpty(e.Data.GetData(DataFormats.StringFormat).ToString())) return;
			
			string language = ProjectService.CurrentProject.Language;
			
			// FIXME languages
			TextNode text = new TextNode(null, e.Data.GetData(DataFormats.StringFormat).ToString(),
			                             language == "VB" || language == "VBNet" ? SupportedLanguage.VBNet : SupportedLanguage.CSharp);

			var node = text.ToSharpTreeNode();
			if (!WatchPad.Instance.WatchList.WatchItems.Contains(node))
				WatchPad.Instance.WatchList.WatchItems.Add(node);
			
			WatchPad.Instance.RefreshPad();
		}
	}
}
