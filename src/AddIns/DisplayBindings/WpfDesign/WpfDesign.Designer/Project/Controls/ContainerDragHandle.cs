// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A thumb where the look can depend on the IsPrimarySelection property.
	/// Used by UIElementSelectionRectangle.
	/// </summary>
	public class ContainerDragHandle : Control
	{
		static ContainerDragHandle()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ContainerDragHandle), new FrameworkPropertyMetadata(typeof(ContainerDragHandle)));
		}
	}
}
