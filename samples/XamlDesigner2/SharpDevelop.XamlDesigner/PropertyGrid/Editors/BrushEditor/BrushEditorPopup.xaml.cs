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
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using SharpDevelop.XamlDesigner.Dom.UndoSystem;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors.BrushEditor
{
	public partial class BrushEditorPopup : Popup
	{
		public BrushEditorPopup()
		{
			InitializeComponent();
			SetResourceReference(StyleProperty, typeof(Popup));
		}

		public PropertyNode PropertyNode
		{
			get { return DataContext as PropertyNode; }
		}

		public void Show(FrameworkElement editor)
		{
			PlacementTarget = editor;
			IsOpen = true;
			uxView.Focus();
			PropertyNode.Context.UndoManager.OpenTransaction();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				IsOpen = false;
			}
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == IsOpenProperty) {
				if (!(bool)e.NewValue) {
					PropertyNode.Context.UndoManager.CommitTransaction();
				}
			}
		}
	}
}
