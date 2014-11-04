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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(Image))]
	public class BorderForImageControl : PermanentAdornerProvider
	{
		AdornerPanel adornerPanel;
		AdornerPanel cachedAdornerPanel;
		Border border;
		
		protected override void OnInitialized()
		{
			base.OnInitialized();

			this.ExtendedItem.PropertyChanged += OnPropertyChanged;

			UpdateAdorner();
		}

		protected override void OnRemove()
		{
			this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
			base.OnRemove();
		}

		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (sender == null || e.PropertyName == "Width" || e.PropertyName == "Height")
			{
				((DesignPanel) this.ExtendedItem.Services.DesignPanel).AdornerLayer.UpdateAdornersForElement(this.ExtendedItem.View, true);
			}
		}

		void UpdateAdorner()
		{
			var element = ExtendedItem.Component as UIElement;
			if (element != null) {
				CreateAdorner();
			}
		}

		private void CreateAdorner()
		{
			if (adornerPanel == null) {
				
				if (cachedAdornerPanel == null) {
					cachedAdornerPanel = new AdornerPanel();
					cachedAdornerPanel.Order = AdornerOrder.Background;
					border = new Border();
					border.BorderThickness = new Thickness(1);
					border.BorderBrush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
					border.Background = Brushes.Transparent;
					border.IsHitTestVisible = true;
					border.MouseDown += border_MouseDown;
					border.MinWidth = 1;
					border.MinHeight = 1;

					AdornerPanel.SetPlacement(border, AdornerPlacement.FillContent);
					cachedAdornerPanel.Children.Add(border);
				}
				
				adornerPanel = cachedAdornerPanel;
				Adorners.Add(adornerPanel);
			}
		}

		void border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (!Keyboard.IsKeyDown(Key.LeftAlt) && ((Image) this.ExtendedItem.View).Source == null)
			{
				e.Handled = true;
				this.ExtendedItem.Services.Selection.SetSelectedComponents(new DesignItem[] {this.ExtendedItem},
				                                                           SelectionTypes.Auto);
				((DesignPanel) this.ExtendedItem.Services.DesignPanel).Focus();
			}
		}
	}
}
