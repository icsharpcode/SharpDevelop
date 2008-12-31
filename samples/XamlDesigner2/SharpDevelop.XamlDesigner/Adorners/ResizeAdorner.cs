using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace SharpDevelop.XamlDesigner
{
	class ResizeAdorner : Control
	{
		static ResizeAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeAdorner),
				new FrameworkPropertyMetadata(typeof(ResizeAdorner)));
		}
	}
}
