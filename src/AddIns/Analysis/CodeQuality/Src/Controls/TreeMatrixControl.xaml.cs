// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms;
using System.Linq;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	/// <summary>
	/// Interaction logic for TreeMatrixControl.xaml
	/// </summary>
	public partial class TreeMatrixControl : System.Windows.Controls.UserControl
	{
		public Matrix<INode> Matrix
		{
			get
			{
				return matrixControl.Matrix;
			}
			
			set
			{
				matrixControl.Matrix = value;
			}
		}
		
		public TreeMatrixControl()
		{
			InitializeComponent();
		}
		
		
		public void DrawTree(Module module)
		{
			FillTree (leftTree,module);
			FillTree (topTree,module);
		}
		
		
		private void FillTree (ICSharpCode.TreeView.SharpTreeView tree,Module module)
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
