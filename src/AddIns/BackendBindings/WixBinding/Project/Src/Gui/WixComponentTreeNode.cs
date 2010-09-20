// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class WixComponentTreeNode : WixTreeNode
	{		
		public WixComponentTreeNode(WixComponentElement element) : base(element)
		{
			SetIcon("Setup.Icons.16x16.Component");
			ContextmenuAddinTreePath = "/AddIns/WixBinding/PackageFilesView/ContextMenu/ComponentTreeNode";
			Refresh();
		}
		
		public override void Refresh()
		{
			base.Refresh();
			Text = XmlElement.GetAttribute("Id");
		}
	}
}
