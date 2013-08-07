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
using ICSharpCode.WpfDesign.Designer.Converters;
using System.Globalization;
using System.Windows.Data;

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
		
		private ScaleTransform scaleTransform;

		public ContainerDragHandle()
		{
			scaleTransform = new ScaleTransform(1.0, 1.0);
			this.LayoutTransform = scaleTransform;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var surface = this.TryFindParent<DesignSurface>();
			if (surface != null && surface.ZoomControl != null)
			{
				var bnd = new Binding("CurrentZoom") { Source = surface.ZoomControl };
				bnd.Converter = InvertedZoomConverter.Instance;

				BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, bnd);
				BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, bnd);
			}
		}
	}
}
