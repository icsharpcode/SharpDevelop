using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SharpDevelop.XamlDesigner
{
	class PanelAdorner : Control
	{
		static PanelAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PanelAdorner),
				new FrameworkPropertyMetadata(typeof(PanelAdorner)));
		}
	}
}
