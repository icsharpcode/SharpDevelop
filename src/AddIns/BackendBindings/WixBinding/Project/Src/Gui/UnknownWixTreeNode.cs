// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
