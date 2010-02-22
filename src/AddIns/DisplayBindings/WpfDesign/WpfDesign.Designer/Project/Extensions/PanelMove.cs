// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Ivan Shumilin"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using System.Windows.Controls;
using System.Windows;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(Panel))]
	public class PanelMove : PermanentAdornerProvider
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();

			var adornerPanel = new AdornerPanel();
			var adorner = new PanelMoveAdorner(ExtendedItem);
			AdornerPanel.SetPlacement(adorner, AdornerPlacement.FillContent);
			adornerPanel.Children.Add(adorner);
			Adorners.Add(adornerPanel);			
		}
	}
}
