// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Used to generate code for a TreeView component currently being designed.
	/// </summary>
	public class PythonTreeViewComponent : PythonDesignerComponent
	{
		List<PythonDesignerTreeNode> rootTreeNodes;
		int treeNodeCount = 1;
		
		public PythonTreeViewComponent(IComponent component) : this(null, component)
		{
		}
		
		public PythonTreeViewComponent(PythonDesignerComponent parent, IComponent component) 
			: base(parent, component)
		{
		}
		
		/// <summary>
		/// Appends code that creates an instance of the tree view.
		/// </summary>
		public override void AppendCreateInstance(PythonCodeBuilder codeBuilder)
		{
			// Append tree node creation first.
			AppendCreateInstance(codeBuilder, GetRootTreeNodes(Component));
			
			// Append tree view creation.
			base.AppendCreateInstance(codeBuilder);
		}
		
		/// <summary>
		/// Appends the component's properties.
		/// </summary>
		public override void AppendComponent(PythonCodeBuilder codeBuilder)
		{
			AppendComment(codeBuilder);
			AppendTreeNodeProperties(codeBuilder, GetRootTreeNodes(Component));
			AppendComponentProperties(codeBuilder, true, false);
		}
		
		void AppendCreateInstance(PythonCodeBuilder codeBuilder, List<PythonDesignerTreeNode> nodes)
		{
			object[] parameters = new object[0];
			foreach (PythonDesignerTreeNode node in nodes) {
				AppendCreateInstance(codeBuilder, node.TreeNode, node.Number, parameters);
				AppendCreateInstance(codeBuilder, node.ChildNodes);
			}
		}
		
		List<PythonDesignerTreeNode> GetRootTreeNodes(IComponent component)
		{
			if (rootTreeNodes == null) {
				rootTreeNodes = new List<PythonDesignerTreeNode>();
				
				TreeView treeView = (TreeView)component;
				AddTreeNodes(rootTreeNodes, treeView.Nodes);
			}
			return rootTreeNodes;
		}
		
		void AddTreeNodes(List<PythonDesignerTreeNode> designerNodes, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes) {
				PythonDesignerTreeNode designerNode = new PythonDesignerTreeNode(node, treeNodeCount);
				designerNodes.Add(designerNode);
				++treeNodeCount;
				
				// Add child nodes.
				AddTreeNodes(designerNode.ChildNodes, node.Nodes);
			}
		}
		
		void AppendTreeNodeProperties(PythonCodeBuilder codeBuilder, List<PythonDesignerTreeNode> nodes)
		{
			foreach (PythonDesignerTreeNode node in nodes) {
				AppendObjectProperties(codeBuilder, node.TreeNode, node.Number);
				
				// Append child nodes to parent tree node.
				if (node.ChildNodes.Count > 0) {
					codeBuilder.AppendIndented(node.Name);
					codeBuilder.Append(".Nodes.AddRange(");
					AppendSystemArray(codeBuilder, typeof(TreeNode).FullName, node.ChildNodes);
					codeBuilder.Append(")");
					codeBuilder.AppendLine();
				}
				
				// Append child node properties.
				AppendTreeNodeProperties(codeBuilder, node.ChildNodes);				
			}
		}
	}
}
