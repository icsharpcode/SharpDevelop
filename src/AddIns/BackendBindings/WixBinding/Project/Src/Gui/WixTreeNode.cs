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
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Base class for all Wix tree nodes.
	/// </summary>
	public class WixTreeNode : ExtTreeNode, IOwnerState
	{
		XmlElement element;
		ExtTreeNode dummyChildNode = null;
		
		public WixTreeNode(XmlElement element)
		{
			this.element = element;
			sortOrder = 10;
			
			if (element.HasChildNodes) {
				dummyChildNode = new ExtTreeNode();
				dummyChildNode.AddTo(this);
			}
		}
		
		public Enum InternalState {
			get {
				WixPackageFilesTreeView treeView = WixPackageFilesTreeView;
				if (treeView != null) {
					return treeView.InternalState;
				}
				return WixPackageFilesTreeView.WixPackageFilesTreeViewState.None;
			}
		}
		
		/// <summary>
		/// Can delete all Wix tree nodes.
		/// </summary>
		public override bool EnableDelete {
			get { return true; }
		}
		
		public override void Delete()
		{
			RemoveElementCommand command = new RemoveElementCommand();
			command.Run();
		}
		
		/// <summary>
		/// Gets the XmlElement associated with this tree node.
		/// </summary>
		public XmlElement XmlElement {
			get { return element; }
		}
		
		public WixPackageFilesTreeView WixPackageFilesTreeView {
			get { return (WixPackageFilesTreeView)TreeView; }
		}
		
		/// <summary>
		/// Adds child nodes to this tree node.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			
			if (dummyChildNode != null) {
				Nodes.Remove(dummyChildNode);
				dummyChildNode = null;
			}
			
			WixTreeNodeBuilder.AddNodes(this, element.ChildNodes);
		}
	}
}
