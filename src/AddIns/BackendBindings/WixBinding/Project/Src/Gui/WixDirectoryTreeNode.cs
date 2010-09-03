// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class WixDirectoryTreeNode : WixTreeNode
	{
		string openImage = "ProjectBrowser.Folder.Open";
		string closedImage = "ProjectBrowser.Folder.Closed";
		
		public WixDirectoryTreeNode(WixDirectoryElement directoryElement) : base(directoryElement)
		{
			ContextmenuAddinTreePath = "/AddIns/WixBinding/PackageFilesView/ContextMenu/DirectoryTreeNode";
			SetIcon(closedImage);
			Refresh();
			sortOrder = 0;
		}	
		
		public override void Refresh()
		{
			base.Refresh();
			Text = DirectoryElement.DirectoryName;
		}
		
		public override void Expanding()
		{
			SetIcon(openImage);
			base.Expanding();
		}
		
		public override void Collapsing()
		{
			SetIcon(closedImage);
			base.Collapsing();
		}
		
		WixDirectoryElement DirectoryElement {
			get {
				return (WixDirectoryElement)XmlElement;
			}
		}
	}
}
