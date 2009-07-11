// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	public class PythonDesignerTreeNode : IArrayItem
	{
		TreeNode node;
		int num;
		List<PythonDesignerTreeNode> childNodes = new List<PythonDesignerTreeNode>();
		
		public PythonDesignerTreeNode(TreeNode node, int num)
		{
			this.node = node;
			this.num = num;
		}
		
		public TreeNode TreeNode {
			get { return node; }
		}
		
		public int Number {
			get { return num; }
		}
		
		public string Name {
			get { return "treeNode" + num.ToString(); }
		}
		
		public List<PythonDesignerTreeNode> ChildNodes {
			get { return childNodes; }
		}
	}
}
