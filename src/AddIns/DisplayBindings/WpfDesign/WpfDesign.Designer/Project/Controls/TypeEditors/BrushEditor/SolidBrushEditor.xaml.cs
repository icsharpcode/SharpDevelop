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

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors.BrushEditor
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

		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
	}
}
