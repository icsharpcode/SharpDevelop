/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.09.2011
 * Time: 23:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of Helpers.
	/// </summary>
	internal class Helpers
	{
		public void FillTree (ICSharpCode.TreeView.SharpTreeView tree,Module module)
		{
			var root = CreateTreeItem(module);
			tree.Root = root;
			
			foreach (var ns in module.Namespaces)
			{
				var namespaceNode = CreateTreeItem(ns);
				tree.Root.Children.Add(namespaceNode);
				
				foreach (var type in ns.Types)
				{
					var typeNode = CreateTreeItem(type);
					namespaceNode.Children.Add(typeNode);

					foreach (var method in type.Methods)
					{
						var methodName = CreateTreeItem(method);
						namespaceNode.Children.Add(methodName);
					}

					foreach (var field in type.Fields)
					{
						var fieldNode = CreateTreeItem(field);
						namespaceNode.Children.Add(fieldNode);
					}
				}
			}
		}
		
		private DependecyTreeNode CreateTreeItem (INode node)
		{
			DependecyTreeNode dtn = new DependecyTreeNode(node);
			return dtn;
		}
	}
}
