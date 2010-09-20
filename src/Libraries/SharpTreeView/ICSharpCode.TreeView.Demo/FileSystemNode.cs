// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.TreeView.Demo
{
	public abstract class FileSystemNode : SharpTreeNode
	{
		public abstract string FullPath { get; }

		public virtual long? FileSize
		{
			get { return null; }
		}

		public virtual DateTime? FileModified
		{
			get { return null; }
		}

		public override string ToString()
		{
			return FullPath;
		}

		public override bool CanCopy(SharpTreeNode[] nodes)
		{
			return true;
		}

		public override IDataObject Copy(SharpTreeNode[] nodes)
		{
			var data = new DataObject();
			var paths = SharpTreeNode.ActiveNodes.Cast<FileSystemNode>().Select(n => n.FullPath).ToArray();
			data.SetData(typeof(string[]), paths);
			return data;
		}

		public override bool CanPaste(IDataObject data)
		{
			return true;
		}

		public override bool CanDelete(SharpTreeNode[] nodes)
		{
			return nodes.All(n => n.Parent != null);
		}

		public override void Delete(SharpTreeNode[] nodes)
		{
			if (MessageBox.Show("Sure?", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				DeleteCore(nodes);
			}
		}

		public override void DeleteCore(SharpTreeNode[] nodes)
		{
			foreach (var node in nodes.ToArray()) {
				node.Parent.Children.Remove(node);
			}
		}

		public override bool CanDrag(SharpTreeNode[] nodes)
		{
			return true;
		}

		ContextMenu menu;

		public override ContextMenu GetContextMenu()
		{
			if (menu == null) {
				menu = new ContextMenu();
				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Cut });
				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Copy });
				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Paste });
				menu.Items.Add(new Separator());
				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Delete });
			}
			return menu;
		}
	}
}
