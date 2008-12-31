using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	public class CategoryView : ItemsControl
	{
		static CategoryView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CategoryView),
				new FrameworkPropertyMetadata(typeof(CategoryView)));
		}
	}
}
