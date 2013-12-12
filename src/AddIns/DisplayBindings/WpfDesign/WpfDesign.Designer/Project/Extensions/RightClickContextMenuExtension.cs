// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	[ExtensionFor(typeof(UIElement))]
	public sealed class RightClickContextMenuExtension : PrimarySelectionAdornerProvider
	{
		DesignPanel panel;
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			
			panel = ExtendedItem.Context.Services.DesignPanel as DesignPanel;
			panel.ContextMenu = new RightClickContextMenu(ExtendedItem);
		}
		
		protected override void OnRemove()
		{
			panel.ContextMenu = null;
			
			base.OnRemove();
		}
	}
}
