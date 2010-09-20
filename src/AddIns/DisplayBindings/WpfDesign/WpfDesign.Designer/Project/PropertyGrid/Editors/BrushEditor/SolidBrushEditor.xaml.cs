// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors.BrushEditor
{
	public partial class SolidBrushEditor
	{
		public SolidBrushEditor()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(SolidBrushEditor),
			                            new FrameworkPropertyMetadata(new Color(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
	}
}
