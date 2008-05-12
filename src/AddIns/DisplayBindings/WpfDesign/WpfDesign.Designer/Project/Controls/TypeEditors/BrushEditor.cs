// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors
{
	/// <summary>
	/// Type editor used to edit Brush properties.
	/// </summary>
	[TypeEditor(typeof(Brush))]
	public sealed class BrushEditor : DockPanel
	{
		readonly IPropertyEditorDataProperty property;
		
		Border brushShowingBorder = new Border {
			SnapsToDevicePixels = true,
			BorderThickness = new Thickness(1)
		};
		DropDownButton ddb = new DropDownButton {
			HorizontalAlignment = HorizontalAlignment.Right
		};
		
		/// <summary>
		/// Creates a new BooleanEditor instance.
		/// </summary>
		public BrushEditor(IPropertyEditorDataProperty property)
		{
			this.property = property;
			
			PropertyEditorBindingHelper.AddValueChangedEventHandler(this, property, OnValueChanged);
			OnValueChanged(null, null);
			
			ddb.Click += new RoutedEventHandler(DropDownButtonClick);
			SetDock(ddb, Dock.Right);
			this.Children.Add(ddb);
			this.Children.Add(brushShowingBorder);
			
			this.Unloaded += delegate {
				if (dlg != null)
					dlg.Close();
			};
		}
		
		BrushEditorDialog dlg;
		
		void DropDownButtonClick(object sender, RoutedEventArgs e)
		{
			dlg = new BrushEditorDialog(property);
			Point pos = ddb.PointToScreen(new Point(ddb.ActualWidth, ddb.ActualHeight));
			dlg.Left = pos.X - dlg.Width;
			dlg.Top = pos.Y;
			dlg.SelectedBrush = property.Value as Brush;
			dlg.SelectedBrushChanged += delegate {
				property.Value = dlg.SelectedBrush;
			};
			dlg.Show();
		}
		
		void OnValueChanged(object sender, EventArgs e)
		{
			Brush val = property.Value as Brush;
			brushShowingBorder.Background = val;
			if (val == null) {
				brushShowingBorder.BorderBrush = null;
			} else if (property.IsSet) {
				brushShowingBorder.BorderBrush = Brushes.Black;
			} else {
				brushShowingBorder.BorderBrush = Brushes.Gray;
			}
		}
	}
}
