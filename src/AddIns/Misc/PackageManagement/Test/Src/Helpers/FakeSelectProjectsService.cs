// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakeSelectProjectsService : ISelectProjectsService
	{
		public bool SelectProjectsReturnValue;
		public IEnumerable<IPackageManagementSelectedProject> ProjectsPassedToSelectProjects;
		public bool IsSelectProjectsCalled;
		
		public bool SelectProjects(IEnumerable<IPackageManagementSelectedProject> projects)
		{
			IsSelectProjectsCalled = true;
			ProjectsPassedToSelectProjects = projects;
			return SelectProjectsReturnValue;
		}
	}
}
