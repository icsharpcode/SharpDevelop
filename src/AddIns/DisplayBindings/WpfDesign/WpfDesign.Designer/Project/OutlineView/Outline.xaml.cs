// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Ivan Shumilin"/>
//     <version>$Revision$</version>
// </file>

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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	public partial class Outline : IOutlineContentHost
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
