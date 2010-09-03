// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A button with a drop-down arrow.
	/// </summary>
	public class DropDownButton : Button
	{
		static readonly Geometry triangle = (Geometry)new GeometryConverter().ConvertFromInvariantString("M0,0 L1,0 0.5,1 z");
		
		public DropDownButton()
		{
			Content = new Path {
				Fill = Brushes.Black,
				Data = triangle,
				Width = 7,
				Height = 3.5,
				Stretch = Stretch.Fill
			};
		}
	}
}
