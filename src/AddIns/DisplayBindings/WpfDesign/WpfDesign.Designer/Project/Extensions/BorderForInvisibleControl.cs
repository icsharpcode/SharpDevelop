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
	public class BorderForInvisibleControl : PermanentAdornerProvider
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();

			var adornerPanel = new AdornerPanel();
			var border = new Border();
			border.BorderThickness = new Thickness(1);
			border.BorderBrush = Brushes.WhiteSmoke;
			border.IsHitTestVisible = false;
			AdornerPanel.SetPlacement(border, AdornerPlacement.FillContent);
			adornerPanel.Children.Add(border);
			Adorners.Add(adornerPanel);
		}
	}
}
