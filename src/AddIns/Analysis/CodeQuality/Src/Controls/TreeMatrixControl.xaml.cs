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
		
		public void DrawMatrix()
		{
			// matrixControl.DrawMatrix();
		}
		
		public void DrawTree(Module module)
		{
			foreach (var ns in module.Namespaces)
			{
				var nsType = new TreeViewItem
				{
					Header = ns.Name
				};

				leftTree.Items.Add(nsType);

				foreach (var type in ns.Types)
				{
					var itemType = new TreeViewItem
					{
						Header = type.Name
					};

					nsType.Items.Add(itemType);

					foreach (var method in type.Methods)
					{
						var itemMethod = new TreeViewItem
						{
							Header = method.Name
						};

						itemType.Items.Add(itemMethod);
					}

					foreach (var field in type.Fields)
					{
						var itemField = new TreeViewItem
						{
							Header = field.Name
						};

						itemType.Items.Add(itemField);
					}
				}
			}
		}
	}
}
