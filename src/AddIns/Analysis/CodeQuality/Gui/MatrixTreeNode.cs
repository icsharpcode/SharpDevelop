// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeQuality.Gui
{
	/// <summary>
	/// Description of MatrixTreeNode.
	/// </summary>
	public class MatrixTreeNode : SharpTreeNode
	{
		INode node;
		
		public INode Node {
			get { return node; }
		}
		
		public MatrixTreeNode(INode node)
		{
			this.node = node;
		}
		
		public override object Icon {
			get { return NodeIconService.GetIcon(node); }
		}
		
		public override object Text {
			get { return node.Name; }
		}
	}
}
