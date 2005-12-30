// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferenceNode : DirectoryNode
	{
		public WebReferenceNode(WebReference webReference) : this(webReference.Directory)
		{
			ProjectItem = webReference.WebReferenceUrl;
		}
		
		public WebReferenceNode(string directory) : base(directory)
		{
			SpecialFolder = SpecialFolder.WebReference;
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WebReferenceNode";
		}
	}
}
