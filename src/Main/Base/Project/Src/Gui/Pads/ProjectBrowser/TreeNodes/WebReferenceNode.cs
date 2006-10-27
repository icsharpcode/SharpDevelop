// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;

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
