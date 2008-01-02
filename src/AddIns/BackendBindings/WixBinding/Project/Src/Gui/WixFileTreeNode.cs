// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
