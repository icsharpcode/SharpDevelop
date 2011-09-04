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
using ICSharpCode.CodeQualityAnalysis.Utility;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	/// <summary>
	/// Interaction logic for TreeMatrixControl.xaml
	/// </summary>
	public partial class TreeMatrixControl : System.Windows.Controls.UserControl
	{
		public Matrix<INode, Relationship> Matrix
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
			
			matrixControl.Colorizer = new DependencyColorizer();
		}
		
		public void DrawTree(Module module)
		{
			Helper.FillTree(leftTree,module);
			Helper.FillTree(topTree,module);
		}
	}
}
