// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
using ICSharpCode.WpfDesign.UIExtensions;

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
