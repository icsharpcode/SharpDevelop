/*
 * User: dickon
 * Date: 23/09/2006
 * Time: 23:09
 * 
 */

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
