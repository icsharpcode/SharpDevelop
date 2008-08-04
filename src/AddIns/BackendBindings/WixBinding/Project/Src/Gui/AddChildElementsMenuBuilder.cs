// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.WixBinding
{
	public class AddChildElementsMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			StringCollection allowedChildElements = GetAllowedChildElements(owner);
			return CreateMenuItems(allowedChildElements);
		}
		
		ToolStripMenuItem[] CreateMenuItems(StringCollection elements)
		{
			List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
			foreach (string element in elements) {
				items.Add(new AddElementCommand(element));
			}
			return items.ToArray();
		}
		
		/// <summary>
		/// Gets the allowed child elements for the current selected node.
		/// </summary>
		StringCollection GetAllowedChildElements(object owner)
		{
			WixTreeNode node = owner as WixTreeNode;
			WixPackageFilesTreeView treeView = owner as WixPackageFilesTreeView;
			if (node != null) {
				treeView = node.TreeView as WixPackageFilesTreeView;
			}
			
			if (treeView != null) {
				return treeView.AllowedChildElements;
			}
			return new StringCollection();
		}
	}
}
