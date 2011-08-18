// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class SelectedMvcViewFolder : SelectedMvcFolder, ISelectedMvcFolder
	{
		public SelectedMvcViewFolder(DirectoryNode directoryNode)
			: base(directoryNode)
		{
		}
		
		public SelectedMvcViewFolder(
			ISelectedFolderNodeInProjectsView selectedFolderNodeInProjectsView,
			IMvcFileService fileService)
			: base(selectedFolderNodeInProjectsView, fileService)
		{
		}
		
		public SelectedMvcViewFolder()
			: base()
		{
		}
	}
}
