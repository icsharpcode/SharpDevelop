using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class SelectionFrame : Control
	{
		static SelectionFrame()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionFrame),
				new FrameworkPropertyMetadata(typeof(SelectionFrame)));
		}
	}
}
