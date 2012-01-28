// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of DependencyTreeNode.
	/// </summary>
	public class DependecyTreeNode:SharpTreeNode
	{
		private INode node;
		public DependecyTreeNode(INode node)
		{
			this.node = node;
		}
		
		public override object Text
		{
			get { return node.Name; }
		}
		
		public override object ToolTip {
			get { return  node.Name; }
		}
		
		public override object Icon {
			get { return node.Icon; }
		}
		
		public INode INode {
			get { return node; }
		}
	}
}
