using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using SharpDevelop.XamlDesigner.Controls;
using System.Windows.Shapes;
using SharpDevelop.XamlDesigner.Placement;
using System.Windows;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Extensibility
{
	class DefaultInitializers
	{
		public static void NewContentControl(DesignItem item)
		{
			(item.Instance as ContentControl).Content = item.Type.Name.ToString();
		}

		public static void Panel(DesignItem item)
		{
			var panel = item.View as Panel;
			if (panel.Background == null) {
				panel.Background = Brushes.Transparent;
			}
			var panelAdorner = new GeneralAdorner(panel);
			panelAdorner.Child = new PanelAdorner();
			item.Context.DesignView.AdornerLayer.Add(panelAdorner);
		}

		public static void Border(DesignItem item)
		{
			var border = item.View as Border;
			border.Background = Brushes.Transparent;
		}

		public static void Shape(DesignItem item)
		{
			var shape = item.View as Shape;
			shape.Fill = Brushes.Transparent;
		}

		public static void Label(DesignItem item)
		{
			var label = item.View as Label;
			label.Padding = new Thickness(0, 0, 5, 5);
		}

		public static void GroupBox(DesignItem item)
		{
			var groupBox = item.View as GroupBox;
			groupBox.Padding = new Thickness(4, 6, 4, 6);
		}

		//public static void FrameworkElement(DesignItem item)
		//{
		//    var container = Activator.CreateInstance(MetadataStore.GetContainer(item)) as PlacementContainer;
		//    PlacementContainer.SetContainer(item, container);
		//}
	}
}
