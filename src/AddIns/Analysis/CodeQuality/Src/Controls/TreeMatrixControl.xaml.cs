/*
 * Created by SharpDevelop.
 * User: Tomas
 * Date: 26.7.2010
 * Time: 10:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			
			matrixControl.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			matrixControl.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			matrixControl.BorderStyle = BorderStyle.FixedSingle;
			
			matrixControl.RowHeadersVisible = false;
			matrixControl.ColumnHeadersVisible = false;
		}
		
		public void DrawMatrix()
		{
			matrixControl.DrawMatrix();
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