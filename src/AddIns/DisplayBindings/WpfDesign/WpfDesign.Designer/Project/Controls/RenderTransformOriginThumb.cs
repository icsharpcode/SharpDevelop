// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Description of RenderTransformThumb.
	/// </summary>
	public class RenderTransformOriginThumb : Thumb
	{
		static RenderTransformOriginThumb()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RenderTransformOriginThumb), new FrameworkPropertyMetadata(typeof(RenderTransformOriginThumb)));
		}
	}
}
