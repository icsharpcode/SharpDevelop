// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
