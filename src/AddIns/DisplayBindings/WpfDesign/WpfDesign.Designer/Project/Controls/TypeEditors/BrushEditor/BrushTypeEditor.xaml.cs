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
using ICSharpCode.WpfDesign.PropertyEditor;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors.BrushEditor
{
	[TypeEditor(typeof(Brush))]
	public partial class BrushTypeEditor
	{
		public BrushTypeEditor(IPropertyEditorDataProperty property)
		{
			this.property = property;
			DataContext = property;
			InitializeComponent();
		}

		IPropertyEditorDataProperty property;

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			BrushEditor.Instance.Property = property;
			BrushEditorPopup.Instance.PlacementTarget = this;
			BrushEditorPopup.Instance.IsOpen = true;
		}
	}
}
