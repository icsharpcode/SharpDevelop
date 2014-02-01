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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Debugger;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : AbstractPadContent
	{
		SharpTreeView tree;
		
		public override object Control {
			get { return tree; }
		}
		
		SharpTreeNodeCollection Items {
			get { return tree.Root.Children; }
		}
		
		public LocalVarPad()
		{
			var res = new CommonResources();
			res.InitializeComponent();
			
			tree = new SharpTreeView();
			tree.Root = new SharpTreeNode();
			tree.ShowRoot = false;
			tree.View = (GridView)res["variableGridView"];
			tree.ItemContainerStyle = (Style)res["itemContainerStyle"];
			tree.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "50%;25%;25%");
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}
		
		void RefreshPad()
		{
			StackFrame frame = WindowsDebugger.CurrentStackFrame;
			
			if (frame == null) {
				this.Items.Clear();
			} else {
				this.Items.Clear();
				frame.Process.EnqueueForEach(
					Dispatcher.CurrentDispatcher,
					ValueNode.GetLocalVariables().ToList(),
					n => this.Items.Add(n.ToSharpTreeNode())
				);
			}
		}
	}
}
