// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
