// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	
	/// <summary>
	/// The drag handle displayed for panels.
	/// </summary>
	[ExtensionServer(typeof(PrimarySelectionExtensionServer))]
	[ExtensionFor(typeof(Panel))]
	[ExtensionFor(typeof(Image))]
	[ExtensionFor(typeof(MediaElement))]
	[ExtensionFor(typeof(ItemsControl))]
	public class TopLeftContainerDragHandle : AdornerProvider
	{
		/// <summary/>
		public TopLeftContainerDragHandle()
		{
			ContainerDragHandle rect = new ContainerDragHandle();
			
			rect.PreviewMouseDown += delegate(object sender, MouseButtonEventArgs e) {
				Services.Selection.SetSelectedComponents(new DesignItem[] { this.ExtendedItem }, SelectionTypes.Auto);
				new DragMoveMouseGesture(this.ExtendedItem, false).Start(this.ExtendedItem.Services.DesignPanel,e);
				e.Handled=true;                                                  
			};
			
			RelativePlacement p = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top);
			p.XOffset = -7;
			p.YOffset = -7;
			
			AddAdorner(p, AdornerOrder.Background, rect);
		}
	}
	
}
