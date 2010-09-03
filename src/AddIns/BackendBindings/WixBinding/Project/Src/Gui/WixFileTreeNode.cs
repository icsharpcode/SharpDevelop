// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.WixBinding
{
	public class WixFileTreeNode : WixTreeNode
	{	
		WixFileElement fileElement;
		
		public WixFileTreeNode(WixFileElement element) : base(element)
		{
			fileElement = element;
			ContextmenuAddinTreePath = "/AddIns/WixBinding/PackageFilesView/ContextMenu/FileTreeNode";
			Refresh();
		}
		
		public override void Refresh()
		{
			Text = fileElement.FileName;
			SetIcon(IconService.GetImageForFile(GetFileName()));
		}
		
		string GetFileName()
		{
			string fileName = fileElement.FileName;
			if (!String.IsNullOrEmpty(fileName)) {
				return fileName;
			}
			return fileElement.Source;
		}
	}
}
