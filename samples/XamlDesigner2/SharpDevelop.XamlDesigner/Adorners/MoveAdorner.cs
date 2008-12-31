using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SharpDevelop.XamlDesigner
{
	class MoveAdorner : Control
	{
		static MoveAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveAdorner),
				new FrameworkPropertyMetadata(typeof(MoveAdorner)));
		}
	}
}
