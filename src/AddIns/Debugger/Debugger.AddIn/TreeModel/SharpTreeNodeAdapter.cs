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
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Debugger.AddIn.TreeModel;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.TreeView;

namespace Debugger.AddIn.Pads.Controls
{
	public class SharpTreeNodeAdapter : SharpTreeNode
	{
		public SharpTreeNodeAdapter(TreeNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			this.Node = node;
			this.LazyLoading = true;
		}
		
		public TreeNode Node { get; private set; }
		
		public override object Icon {
			get { return this.Node.Image != null ? this.Node.Image.ImageSource : null; }
		}
		
		public override bool ShowExpander {
			get { return this.Node.GetChildren != null; }
		}
		
		public override bool CanDelete(SharpTreeNode[] nodes)
		{
			return nodes.All(n => n is SharpTreeNodeAdapter)
				&& nodes.Cast<SharpTreeNodeAdapter>().All(n => n.Node.CanDelete);
		}
		
		public override void Delete(SharpTreeNode[] nodes)
		{
			foreach (var node in nodes)
				node.Parent.Children.Remove(this);
		}
		
		protected override void LoadChildren()
		{
			if (this.Node.GetChildren != null) {
				var process = WindowsDebugger.CurrentProcess;
				process.EnqueueWork(Dispatcher.CurrentDispatcher, () => Children.AddRange(this.Node.GetChildren().Select(node => node.ToSharpTreeNode())));
			}
		}
	}
}
