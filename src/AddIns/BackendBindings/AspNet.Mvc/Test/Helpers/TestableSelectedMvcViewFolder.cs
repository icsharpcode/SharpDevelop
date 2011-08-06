// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class TestableSelectedMvcViewFolder : SelectedMvcViewFolder
	{
		public TestableSelectedMvcViewFolder(DirectoryNode directoryNode)
			: base(directoryNode)
		{
		}
		
		public string PathPassedToAddNewFileToDirectoryNode;
		
		protected override void AddNewFileToDirectoryNode(string path)
		{
			PathPassedToAddNewFileToDirectoryNode = path;
		}
	}
}
