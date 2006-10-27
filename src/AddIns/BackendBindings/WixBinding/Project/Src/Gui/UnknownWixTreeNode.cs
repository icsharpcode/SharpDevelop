// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Represents an xml element that the UI is not currently handling.
	/// </summary>
	public class UnknownWixTreeNode : WixTreeNode
	{
		public UnknownWixTreeNode(XmlElement element) : base(element)
		{
			SetIcon("Icons.16x16.MiscFiles");
			ContextmenuAddinTreePath = "/AddIns/WixBinding/PackageFilesView/ContextMenu/UnknownTreeNode";
			Refresh();
		}
		
		public override void Refresh()
		{
			base.Refresh();
			Text = XmlElement.LocalName;
		}
	}
}
