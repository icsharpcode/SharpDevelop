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
		
		private void FillTree (System.Windows.Controls.TreeView tree,Module module)
		{
			foreach (var ns in module.Namespaces)
			{
				var leftType = CreateTreeItem(ns.Name);

				tree.Items.Add(leftType);

				foreach (var type in ns.Types)
				{
					var lType = CreateTreeItem(type.Name);
					
					leftType.Items.Add(lType);

					foreach (var method in type.Methods)
					{
						var leftMethod = CreateTreeItem(method.Name);
						leftType.Items.Add(leftMethod);
					}

					foreach (var field in type.Fields)
					{
						var leftField = CreateTreeItem(field.Name);
						leftType.Items.Add(leftField);
					}
				}
			}
		}
			
			
		private TreeViewItem CreateTreeItem (string ns)
		{
			var nsType = new TreeViewItem
			{
				Header = ns,
				ToolTip = ns
			};
//			nsType.Height = matrixControl.CellHeight;
			return nsType;
		}
	}
}
