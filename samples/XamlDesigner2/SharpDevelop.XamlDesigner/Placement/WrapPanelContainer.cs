using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SharpDevelop.XamlDesigner.Placement
{
	class WrapPanelContainer : PreviewContainer
	{
		public override Orientation GetOrientation(FrameworkElement parentView)
		{
			return (parentView as WrapPanel).Orientation;
		}
	}
}
