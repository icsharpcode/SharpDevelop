// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class SelectedFolderNodeInProjectsView : ISelectedFolderNodeInProjectsView
	{
		DirectoryNode directoryNode;
		MvcProject project;
		
		public SelectedFolderNodeInProjectsView(DirectoryNode directoryNode)
		{
			this.directoryNode = directoryNode;
			this.project = new MvcProject(directoryNode.Project);
		}
		
		public string Folder {
			get { return directoryNode.Directory; }
		}
		
		public IMvcProject Project {
			get { return project; }
		}
		
		public void AddNewFile(string path)
		{
			directoryNode.AddNewFile(path);
		}
	}
}
