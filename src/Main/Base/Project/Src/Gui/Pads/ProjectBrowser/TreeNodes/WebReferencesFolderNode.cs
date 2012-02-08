// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferencesFolderNode : DirectoryNode
	{		
		public WebReferencesFolderNode(WebReferencesProjectItem projectItem)
			: this(projectItem, FileNodeStatus.None)
		{
		}
		
		public WebReferencesFolderNode(WebReferencesProjectItem projectItem, FileNodeStatus status)
			: this(projectItem.Directory, status)
		{
			ProjectItem = projectItem;
		}
		
		public WebReferencesFolderNode(string directory)
			: this(directory, FileNodeStatus.None)
		{
		}
		
		public WebReferencesFolderNode(string directory, FileNodeStatus status)
			: base(directory)
		{
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WebReferencesFolderNode";
			FileNodeStatus = status;
			sortOrder = 0;
			SpecialFolder = SpecialFolder.WebReferencesFolder;
		}
	}
}
