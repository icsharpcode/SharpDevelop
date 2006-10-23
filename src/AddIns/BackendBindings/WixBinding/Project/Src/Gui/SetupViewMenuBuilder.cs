// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.WixBinding
{
	/*
	/// <summary>
	/// Builds the View | Setup menu items.
	/// </summary>
	public class SetupViewMenuBuilder : ISubmenuBuilder
	{
		public SetupViewMenuBuilder()
		{
		}
		
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
			AddInTreeNode node = AddInTree.GetTreeNode("/AddIns/WixBinding/View");
			foreach (Codon childCodon in node.Codons) {
				items.Add(CreateMenuItem(childCodon, owner));
			}
			return items.ToArray();
		}
		
		/// <summary>
		/// Creates the toolstrip menu item and assumes that there are never any
		/// child menu items.
		/// </summary>
		public ToolStripMenuItem CreateMenuItem(Codon codon, object owner)
		{			
			ToolStripMenuItem item = (ToolStripMenuItem)codon.BuildItem(owner, null);

			// Set shortcut.
			string shortcut = codon.Properties["shortcut"];
			if (shortcut != null) {
				item.ShortcutKeys = MenuCommand.ParseShortcut(shortcut);
			}
			
			// Set icon
			string icon = codon.Properties["icon"];
			if (!String.IsNullOrEmpty(icon)) {
				item.Image = IconService.GetBitmap(icon);
			}
			return item;
		}
	}
	*/
}
