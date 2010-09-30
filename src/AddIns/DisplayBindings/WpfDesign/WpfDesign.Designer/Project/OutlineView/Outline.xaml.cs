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

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	public partial class Outline
	{
		public Outline()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty RootProperty =
			DependencyProperty.Register("Root", typeof(OutlineNode), typeof(Outline));

		public OutlineNode Root {
			get { return (OutlineNode)GetValue(RootProperty); }
			set { SetValue(RootProperty, value); }
		}
		
		public object OutlineContent {
			get { return this; }
		}
	}
}
