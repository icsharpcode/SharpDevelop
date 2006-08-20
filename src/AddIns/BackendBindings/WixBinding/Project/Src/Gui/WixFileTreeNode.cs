// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Xml;

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
			Text = GetDisplayText();
			SetIcon(IconService.GetImageForFile(GetFileName()));
		}
		
		string GetFileName()
		{
			string source = fileElement.Source;
			if (!String.IsNullOrEmpty(source)) {
				return source;
			}
			string longName = fileElement.LongName;
			if (!String.IsNullOrEmpty(longName)) {
				return longName;
			}
			return fileElement.ShortName;
		}
		
		/// <summary>
		/// Gets the text that will be displayed for the tree node.
		/// </summary>
		string GetDisplayText()
		{
			string longName = fileElement.LongName;
			if (!String.IsNullOrEmpty(longName)) {
				return longName;
			}
			return fileElement.ShortName;
		}
	}
}
