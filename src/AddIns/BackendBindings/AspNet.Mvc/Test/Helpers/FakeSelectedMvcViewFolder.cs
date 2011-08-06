// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeSelectedMvcViewFolder : ISelectedMvcViewFolder
	{
		public string Path { get; set; }
		
		public string ProjectLanguage {
			get { return FakeProject.Language; }
			set { FakeProject.SetLanguage(value); }
		}
		
		public TestableProject FakeProject = TestableProject.CreateProject();
		
		public IProject Project {
			get { return FakeProject; }
		}
		
		public string FileNamePassedToAddFile;
		
		public void AddFileToProject(string fileName)
		{
			FileNamePassedToAddFile = fileName;
		}
	}
}
