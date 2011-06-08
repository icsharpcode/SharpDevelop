// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class TestableDTEProject : Project
	{
		public TestableProject TestableProject;
		public FakePackageManagementProjectService FakeProjectService;
		public FakeFileService FakeFileService;
		
		public TestableDTEProject()
			: this(ProjectHelper.CreateTestProject())
		{
		}
		
		public TestableDTEProject(TestableProject project)
			: this(
			project,
			new FakePackageManagementProjectService(),
			new FakeFileService(project))
		{
		}
		
		public TestableDTEProject(
			TestableProject project,
			FakePackageManagementProjectService projectService,
			FakeFileService fileService)
			: base(project, projectService, fileService)
		{
			this.TestableProject = project;
			this.FakeProjectService = projectService;
			this.FakeFileService = fileService;
		}
	}
}
