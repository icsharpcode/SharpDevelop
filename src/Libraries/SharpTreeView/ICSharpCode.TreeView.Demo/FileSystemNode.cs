// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			return nodes.All(n => n is FileSystemNode);
		}
		
		protected override IDataObject GetDataObject(SharpTreeNode[] nodes)
		{
			var data = new DataObject();
			var paths = nodes.OfType<FileSystemNode>().Select(n => n.FullPath).ToArray();
			data.SetData(DataFormats.FileDrop, paths);
			return data;
		}
		
		public override bool CanDelete(SharpTreeNode[] nodes)
		{
			return nodes.All(n => n is FileSystemNode);
		}
		
		public override void Delete(SharpTreeNode[] nodes)
		{
			if (MessageBox.Show("Are you sure you want to delete " + nodes.Length + " items?", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				DeleteWithoutConfirmation(nodes);
			}
		}
		
		public override void DeleteWithoutConfirmation(SharpTreeNode[] nodes)
		{
			foreach (var node in nodes) {
				if (node.Parent != null)
					node.Parent.Children.Remove(node);
			}
		}

		
//		ContextMenu menu;
//
//		public override ContextMenu GetContextMenu()
//		{
//			if (menu == null) {
//				menu = new ContextMenu();
//				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Cut });
//				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Copy });
//				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Paste });
//				menu.Items.Add(new Separator());
//				menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Delete });
//			}
//			return menu;
//		}
	}
}
