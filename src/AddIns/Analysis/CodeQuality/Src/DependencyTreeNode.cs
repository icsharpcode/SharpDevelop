/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.09.2011
 * Time: 23:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		
		public INode INode
		{
			get {return node;}
		}
	}
}
