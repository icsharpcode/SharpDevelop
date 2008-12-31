using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SharpDevelop.XamlDesigner
{
	class ContainerAdorner : Control
	{
		static ContainerAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ContainerAdorner),
				new FrameworkPropertyMetadata(typeof(ContainerAdorner)));
		}
	}
}
