using System;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ExtTreeViewComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			Debug.Assert(x != null);
			Debug.Assert(y != null);
			ExtTreeNode node1 = x as ExtTreeNode;
			ExtTreeNode node2 = y as ExtTreeNode;
			
			if (node1 == null || node2 == null) {
				return ((TreeNode)x).Text.CompareTo(((TreeNode)y).Text);
			}
			
			if (node1.SortOrder != node2.SortOrder) {
				return Math.Sign(node1.SortOrder - node2.SortOrder);
			}
			
			return node1.CompareString.CompareTo(node2.CompareString);
		}
	}
}
