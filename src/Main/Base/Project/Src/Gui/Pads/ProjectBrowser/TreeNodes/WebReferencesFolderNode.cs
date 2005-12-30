// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferencesFolderNode : DirectoryNode
	{		
		public WebReferencesFolderNode(WebReferencesProjectItem projectItem) : this(projectItem.Directory)
		{
			ProjectItem = projectItem;
		}
		
		public WebReferencesFolderNode(string directory) : base(directory)
		{
			sortOrder = 0;
			SpecialFolder = SpecialFolder.WebReferencesFolder;
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WebReferencesFolderNode";
		}
	}
}
