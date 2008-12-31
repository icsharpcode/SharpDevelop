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
using System.Windows.Controls.Primitives;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors.BrushEditor
{
	public partial class BrushTypeEditor : UserControl
	{
		public BrushTypeEditor()
		{
			InitializeComponent();
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			new BrushEditorPopup().Show(this);
		}
	}
}
