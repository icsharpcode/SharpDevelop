// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2263 $</version>
// </file>

using System;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Makes TabItems clickable.
	/// </summary>
	[ExtensionFor(typeof(TabItem))]
	[ExtensionServer(typeof(PrimarySelectionExtensionServer))]
	public sealed class TabItemClickableExtension : DefaultExtension
	{
		/// <summary/>
		protected override void OnInitialized()
		{
			// When tab item becomes primary selection, make it the active tab page in its parent tab control.
			TabItem tabItem = (TabItem)this.ExtendedItem.Component;
			TabControl tabControl = tabItem.Parent as TabControl;
			if (tabControl != null) {
				tabControl.SelectedItem = tabItem;
			}
		}
	}
}
