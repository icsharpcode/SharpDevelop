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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(Panel))]
	[ExtensionFor(typeof(Border))]
	[ExtensionFor(typeof(ContentControl))]
	[ExtensionFor(typeof(Viewbox))]
	public class BorderForInvisibleControl : PermanentAdornerProvider
	{
		AdornerPanel adornerPanel;
		AdornerPanel cachedAdornerPanel;
		
		protected override void OnInitialized()
		{
			base.OnInitialized();

			if (ExtendedItem.Component is Border)
			{
				ExtendedItem.PropertyChanged += (s, e) => UpdateAdorner();
			}
			
			// If component is a ContentControl it must be of type ContentControl specifically, and not derived types like Label and Button.
			if (!(ExtendedItem.Component is ContentControl) || ExtendedItem.Component.GetType() == typeof(ContentControl)) {
				UpdateAdorner();
				
				var element = ExtendedItem.Component as UIElement;
				if (element != null) {
					element.IsVisibleChanged += (s, e) => UpdateAdorner();
				}
			}
		}

		void UpdateAdorner()
		{
			var element = ExtendedItem.Component as UIElement;
			if (element != null) {
				var border = element as Border;
				if (element.IsVisible && (border == null || IsAnyBorderEdgeInvisible(border))) {
					CreateAdorner();
					
					if (border != null) {
						var adornerBorder = (Border)adornerPanel.Children[0];
						
						if (IsBorderBrushInvisible(border))
							adornerBorder.BorderThickness = new Thickness(1);
						else
							adornerBorder.BorderThickness = new Thickness(border.BorderThickness.Left   > 0 ? 0 : 1,
							                                              border.BorderThickness.Top    > 0 ? 0 : 1,
							                                              border.BorderThickness.Right  > 0 ? 0 : 1,
							                                              border.BorderThickness.Bottom > 0 ? 0 : 1);
					}
				}
				else {
					RemoveAdorner();
				}
			}
		}
		
		bool IsAnyBorderEdgeInvisible(Border border)
		{
			return IsBorderBrushInvisible(border) || border.BorderThickness.Left == 0 || border.BorderThickness.Top == 0 || border.BorderThickness.Right == 0 || border.BorderThickness.Bottom == 0;
		}
		
		bool IsBorderBrushInvisible(Border border)
		{
			return border.BorderBrush == null || border.BorderBrush == Brushes.Transparent;
		}
		
		private void CreateAdorner()
		{
			if (adornerPanel == null) {
				
				if (cachedAdornerPanel == null) {
					cachedAdornerPanel = new AdornerPanel();
					cachedAdornerPanel.Order = AdornerOrder.Background;
					var border = new Border();
					border.BorderThickness = new Thickness(1);
					border.BorderBrush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
					border.IsHitTestVisible = false;
					AdornerPanel.SetPlacement(border, AdornerPlacement.FillContent);
					cachedAdornerPanel.Children.Add(border);
				}
				
				adornerPanel = cachedAdornerPanel;
				Adorners.Add(adornerPanel);
			}
		}
		
		private void RemoveAdorner()
		{
			if (adornerPanel != null) {
				Adorners.Remove(adornerPanel);
				adornerPanel = null;
			}
		}
	}
}
