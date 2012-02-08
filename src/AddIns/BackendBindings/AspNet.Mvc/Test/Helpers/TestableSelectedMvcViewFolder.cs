// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class TestableSelectedMvcViewFolder : SelectedMvcViewFolder
	{
		public FakeMvcFileService FakeFileService;
		public FakeSelectedFolderNodeInProjectsView FakeSelectedFolderNodeInProjectsView;
		
		public TestableSelectedMvcViewFolder()
			: this(new FakeSelectedFolderNodeInProjectsView(), new FakeMvcFileService())
		{
		}
		
		public TestableSelectedMvcViewFolder(
			FakeSelectedFolderNodeInProjectsView selectedFolderNode,
			FakeMvcFileService fileService)
			: base(selectedFolderNode, fileService)
		{
			this.FakeSelectedFolderNodeInProjectsView = selectedFolderNode;
			this.FakeFileService = fileService;
		}
	}
}
