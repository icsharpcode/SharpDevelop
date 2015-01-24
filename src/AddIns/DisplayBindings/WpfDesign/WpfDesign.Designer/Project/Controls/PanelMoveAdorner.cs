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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Designer.Converters;
using System.Globalization;
using System.Windows.Data;
using ICSharpCode.WpfDesign.UIExtensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public class PanelMoveAdorner : Control
	{
		static PanelMoveAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PanelMoveAdorner),
			                                         new FrameworkPropertyMetadata(typeof(PanelMoveAdorner)));
		}
		
		private ScaleTransform scaleTransform;

		public PanelMoveAdorner(DesignItem item)
		{
			this.item = item;
			
			scaleTransform = new ScaleTransform(1.0, 1.0);
			this.LayoutTransform = scaleTransform;
		}

		DesignItem item;

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			e.Handled = true;
			//item.Services.Selection.SetSelectedComponents(new DesignItem [] { item }, SelectionTypes.Auto);
			new DragMoveMouseGesture(item, false).Start(item.Services.DesignPanel, e);
		}
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var bnd = new Binding("IsVisible") {Source = item.Component};
			bnd.Converter = CollapsedWhenFalse.Instance;
			BindingOperations.SetBinding(this, UIElement.VisibilityProperty, bnd);

			var surface = this.TryFindParent<DesignSurface>();
			if (surface != null && surface.ZoomControl != null)
			{
				bnd = new Binding("CurrentZoom") {Source = surface.ZoomControl};
				bnd.Converter = InvertedZoomConverter.Instance;

				BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, bnd);
				BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, bnd);
			}
		}
	}
}
