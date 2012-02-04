// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace Gui.Pads.ProjectBrowser.TreeNodes
{
	public class ServiceReferencesFolderNode : DirectoryNode
	{
		public ServiceReferencesFolderNode(ProjectItem projectItem, FileNodeStatus status)
			: this((ServiceReferencesProjectItem)projectItem, status)
		{
		}
		
		public ServiceReferencesFolderNode(ServiceReferencesProjectItem projectItem, FileNodeStatus status)
			: this(projectItem.Directory, status)
		{
			this.ProjectItem = projectItem;
		}
		
		public ServiceReferencesFolderNode(string directory)
			: this(directory, FileNodeStatus.None)
		{
		}
		
		public ServiceReferencesFolderNode(string directory, FileNodeStatus status)
			: base(directory, status)
		{	
			this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ServiceReferencesFolderNode";
			this.sortOrder = 0;
			this.SpecialFolder = SpecialFolder.ServiceReferencesFolder;
		}
	}
}
