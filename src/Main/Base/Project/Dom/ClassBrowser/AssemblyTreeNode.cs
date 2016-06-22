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
using System.Windows.Controls;
using ICSharpCode.Core.Presentation;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class AssemblyTreeNode : ModelCollectionTreeNode
	{
		IAssemblyModel model;
		
		public AssemblyTreeNode(IAssemblyModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			this.model = model;
		}
		
		protected override object GetModel()
		{
			return model;
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get {
				return NodeTextComparer;
			}
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				return model.Namespaces;
			}
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			if (model.Context.IsValid) {
				base.LoadChildren();
			} else {
				// This assembly could not be loaded correctly, add sub-node with error text
				Children.Add(new AssemblyLoadErrorTreeNode());
			}
		}
		
		public override object Text {
			get {
				return model.AssemblyName;
			}
		}
		
		public override object Icon {
			get {
				if (model.Context.IsValid) {
					if (model.IsUnpinned()) {
						return SD.ResourceService.GetImageSource("Icons.16x16.AssemblyUnpinned");
					} else {
						return SD.ResourceService.GetImageSource("Icons.16x16.Assembly");
					}
				} else {
					return SD.ResourceService.GetImageSource("Icons.16x16.AssemblyError");
				}
			}
		}
		
		public override void ShowContextMenu(ContextMenuEventArgs e)
		{
			MenuService.ShowContextMenu(null, model, "/SharpDevelop/Pads/ClassBrowser/AssemblyContextMenu");
		}
	}
}
