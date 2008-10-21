// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/*
	/// <summary>
	/// The drag handle displayed for panels.
	/// </summary>
	[ExtensionServer(typeof(PrimarySelectionExtensionServer))]
	[ExtensionFor(typeof(Panel), OverrideExtension = typeof(TopLeftResizeThumb))]
	public class TopLeftContainerDragHandle : AdornerProvider
	{
		/// <summary/>
		public TopLeftContainerDragHandle()
		{
			ContainerDragHandle rect = new ContainerDragHandle();
			
			rect.PreviewMouseDown += delegate {
				Services.Selection.SetSelectedComponents(new DesignItem[] { this.ExtendedItem }, SelectionTypes.Auto);
			};
			
			RelativePlacement p = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top);
			p.XOffset = -1;
			p.YOffset = -1;
			
			AddAdorner(p, AdornerOrder.Background, rect);
		}
	}
	*/
}
