// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace SharpServerTools.Forms
{
	public class NodeAwareContextMenuStrip : ContextMenuStrip
	{
		TreeNode treeNodeAttached;
		
		public NodeAwareContextMenuStrip(TreeNode treeNodeAttached) : base()
		{
			this.treeNodeAttached = treeNodeAttached;		
		}
		
		public TreeNode TreeNode {
			get {
				return treeNodeAttached;
			}
		}
	}
}
