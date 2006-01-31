// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageModuleTreeNode : CodeCoverageTreeNode
	{
		public CodeCoverageModuleTreeNode(CodeCoverageModule module) : base(module.Name, module.VisitedSequencePointsCount, module.NotVisitedSequencePointsCount)
		{
			ImageIndex = 2;
			SelectedImageIndex = ImageIndex;
			
			foreach (CodeCoverageMethod method in module.Methods) {
				AddMethod(method);
			}
			
			// Update percentage totals for namespace nodes.
			UpdateNamespacePercentages(Nodes);
		}
		
		void AddMethod(CodeCoverageMethod method)
		{
			ExtTreeNode parentNode = GetParentNamespaceNode(method.ClassNamespace);
			
			// Add class node.
			ExtTreeNode classNode = FindNode(parentNode.Nodes, method.ClassName);
			if (classNode == null) {
				classNode = new CodeCoverageClassTreeNode(method.ClassName);
				classNode.AddTo(parentNode);
			}
			
			// Add method node.
			CodeCoverageMethodTreeNode methodNode = new CodeCoverageMethodTreeNode(method);
			methodNode.AddTo(classNode);
		}
		
		ExtTreeNode GetParentNamespaceNode(string classNamespace)
		{
			string[] treePath = classNamespace.Split('.');
			
			ExtTreeNode node = this;
			ExtTreeNode currentNode = node;

			foreach (string path in treePath) {
				if (currentNode != null) {
					currentNode = FindNode(currentNode.Nodes, path);
				} 
				
				if (currentNode == null) {
					CodeCoverageNamespaceTreeNode newNode = new CodeCoverageNamespaceTreeNode(path);
					newNode.AddTo(node);
					node = newNode;
				} else {
					node = currentNode;
				}
			}
			
			return node;
		}
		
		ExtTreeNode FindNode(TreeNodeCollection nodes, string name)
		{
			foreach (ExtTreeNode node in nodes) {
				if (node.Text == name) {
					return node;
				}
			}
			return null;
		}
		
		void UpdateNamespacePercentages(TreeNodeCollection nodes)
		{
			foreach (ExtTreeNode node in nodes) {
				CodeCoverageNamespaceTreeNode namespaceNode = node as CodeCoverageNamespaceTreeNode;
				if (namespaceNode != null) {
					SumVisits(namespaceNode);
				} 
			}
		}
		
		void SumVisits(CodeCoverageNamespaceTreeNode parentNode)
		{
			int visitedCount = 0;
			int notVisitedCount = 0;
			
			foreach (CodeCoverageTreeNode node in parentNode.Nodes) {
				CodeCoverageNamespaceTreeNode namespaceNode = node as CodeCoverageNamespaceTreeNode;
				CodeCoverageClassTreeNode classNode = node as CodeCoverageClassTreeNode;
				if (namespaceNode != null) {
					SumVisits(namespaceNode);
				} else if (classNode != null) {
					SumVisits(classNode);
				}
				
				visitedCount += node.VisitedCount;
				notVisitedCount += node.NotVisitedCount;
			}
			
			parentNode.VisitedCount = visitedCount;
			parentNode.NotVisitedCount = notVisitedCount;
		}
		
		void SumVisits(CodeCoverageClassTreeNode parentNode)
		{
			int visitedCount = 0;
			int notVisitedCount = 0;
			
			foreach (CodeCoverageTreeNode node in parentNode.Nodes) {				
				visitedCount += node.VisitedCount;
				notVisitedCount += node.NotVisitedCount;
			}
			
			parentNode.VisitedCount = visitedCount;
			parentNode.NotVisitedCount = notVisitedCount;
		}
	}
}
