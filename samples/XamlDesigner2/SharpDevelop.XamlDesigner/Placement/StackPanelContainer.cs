using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace SharpDevelop.XamlDesigner.Placement
{
	class StackPanelContainer : PreviewContainer
	{
		public override Orientation GetOrientation(FrameworkElement parentView)
		{
			return (parentView as StackPanel).Orientation;
		}
	}
}
