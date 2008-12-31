using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	public class PropertyNodeView : ContentControl
	{
		static PropertyNodeView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyNodeView),
				new FrameworkPropertyMetadata(typeof(PropertyNodeView)));
		}
	}
}
