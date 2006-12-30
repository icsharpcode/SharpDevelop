// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// The resize thumb at the top left edge of a component.
	/// </summary>
	[ExtensionFor(typeof(FrameworkElement))]
	public class TopLeftResizeThumb : PrimarySelectionAdornerProvider
	{
		/// <summary></summary>
		public TopLeftResizeThumb()
		{
			ResizeThumb resizeThumb = new ResizeThumb();
			
			RelativePlacement p = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top);
			AddAdorner(p, AdornerOrder.Foreground, resizeThumb);
		}
	}
}
