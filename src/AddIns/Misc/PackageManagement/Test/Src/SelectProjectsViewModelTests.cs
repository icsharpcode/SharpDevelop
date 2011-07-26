// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SelectProjectsViewModelTests
	{
		SelectProjectsViewModel viewModel;
		List<FakeSelectedProject> fakeSelectedProjects;
		
		void CreateSelectedProjects()
		{
			fakeSelectedProjects = new List<FakeSelectedProject>();
		}
		
		void CreateViewModel(List<FakeSelectedProject> fakeSelectedProjects)
		{
			viewModel = new SelectProjectsViewModel(fakeSelectedProjects);
		}
		
		void AddTwoProjects()
		{
			var project = new FakeSelectedProject("Test1");
			fakeSelectedProjects.Add(project);
			project = new FakeSelectedProject("Test2");
			fakeSelectedProjects.Add(project);
		}
		
		[Test]
		public void Projects_TwoSelectedProjects_ContainsTwoProjects()
		{
			CreateSelectedProjects();
			AddTwoProjects();
			CreateViewModel(fakeSelectedProjects);
			
			ObservableCollection<IPackageManagementSelectedProject> projects =
				viewModel.Projects;
			
			List<FakeSelectedProject> expectedProjects = fakeSelectedProjects;
			
			CollectionAssert.AreEqual(expectedProjects, projects);
		}
	}
}
