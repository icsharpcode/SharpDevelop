// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using System.Windows.Controls;
using System.Windows;
using ICSharpCode.WpfDesign.Designer.Controls;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(Panel))]
	[ExtensionFor(typeof(Border))]
	[ExtensionFor(typeof(ContentControl))]
	[ExtensionFor(typeof(Viewbox))]
	public class BorderForInvisibleControl : PermanentAdornerProvider
	{
		AdornerPanel adornerPanel;
		
		protected override void OnInitialized()
		{
			base.OnInitialized();

			if (ExtendedItem.Component is Border)
			{
				ExtendedItem.PropertyChanged+= (s, e) => ExtendedItem_PropertyChanged();
				
				ExtendedItem_PropertyChanged();
			}
			else if (ExtendedItem.Component is Panel || ExtendedItem.Component is Viewbox || ExtendedItem.Component is ContentControl)
			{
				CreateAdorner();
			}			
		}

		void ExtendedItem_PropertyChanged()
		{
			if (ExtendedItem.Component is Border)
			{
				var border = ExtendedItem.Component as Border;
				if (border.ReadLocalValue(Border.BorderBrushProperty) == DependencyProperty.UnsetValue || border.ReadLocalValue(Border.BorderThicknessProperty) == DependencyProperty.UnsetValue)
				{
					CreateAdorner();
				}
				else
				{
					RemoveAdorner();
				}
			}			
		}
		
		private void CreateAdorner()
		{
			if (adornerPanel == null)
			{
				adornerPanel = new AdornerPanel();
				adornerPanel.Order = AdornerOrder.Background;
				var border = new Border();
				border.BorderThickness = new Thickness(1);
				border.BorderBrush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
				border.IsHitTestVisible = false;
				AdornerPanel.SetPlacement(border, AdornerPlacement.FillContent);
				adornerPanel.Children.Add(border);
				Adorners.Add(adornerPanel);
			}
		}
		
		private void RemoveAdorner()
		{
			if (adornerPanel != null)
			{
				Adorners.Remove(adornerPanel);
				adornerPanel = null;
			}
		}
	}
}
