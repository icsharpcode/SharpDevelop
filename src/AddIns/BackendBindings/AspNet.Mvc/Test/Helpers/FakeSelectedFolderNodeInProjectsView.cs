// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeSelectedFolderNodeInProjectsView : ISelectedFolderNodeInProjectsView
	{
		public FakeSelectedFolderNodeInProjectsView()
		{
			Folder = @"d:\projects\MyMvcApp\Views\Home";
		}
		
		public string Folder { get; set; }
		
		public FakeMvcProject FakeMvcProject = new FakeMvcProject();
		
		public IMvcProject Project {
			get { return FakeMvcProject; }
		}
		
		public string PathPassedToAddNewFile;
		
		public void AddNewFile(string path)
		{
			PathPassedToAddNewFile = path;
		}
	}
}
