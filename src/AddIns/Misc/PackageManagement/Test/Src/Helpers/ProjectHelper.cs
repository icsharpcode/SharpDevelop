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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public static class ProjectHelper
	{
		public static ISolution CreateSolution()
		{
			SD.InitializeForUnitTests();
			ISolution solution = MockRepository.GenerateStrictMock<ISolution>();
			solution.Stub(s => s.MSBuildProjectCollection).Return(new Microsoft.Build.Evaluation.ProjectCollection());
			solution.Stub(s => s.Projects).Return(new NullSafeSimpleModelCollection<IProject>());
			solution.Stub(s => s.ActiveConfiguration).Return(new ConfigurationAndPlatform("Debug", "Any CPU"));
			//solution.Stub(s => s.FileName).Return(FileName.Create(@"d:\projects\Test\TestSolution.sln"));
			return solution;
		}
		
		public static TestableProject CreateTestProject()
		{
			return CreateTestProject("TestProject");
		}
		
		public static TestableProject CreateTestProject(string name)
		{
			ISolution solution = CreateSolution();
			
			return CreateTestProject(solution, name);
		}
		
		public static TestableProject CreateTestProject(
			ISolution parentSolution,
			string name,
			string fileName = null)
		{
			var createInfo = new ProjectCreateInformation(parentSolution, new FileName(fileName ?? (@"d:\projects\Test\TestProject\" + name + ".csproj")));
			
			var project = new TestableProject(createInfo);
			((ICollection<IProject>)parentSolution.Projects).Add(project);
			return project;
		}
		
		public static TestableProject CreateTestWebApplicationProject()
		{
			TestableProject project = CreateTestProject();
			AddWebApplicationProjectType(project);
			return project;
		}
		
		public static TestableProject CreateTestWebSiteProject()
		{
			TestableProject project = CreateTestProject();
			AddWebSiteProjectType(project);
			return project;
		}
		
		public static void AddWebApplicationProjectType(MSBuildBasedProject project)
		{
			AddProjectType(project, ProjectTypeGuids.WebApplication);
		}
		
		public static void AddWebSiteProjectType(TestableProject project)
		{
			AddProjectType(project, ProjectTypeGuids.WebSite);
		}
		
		public static void AddProjectType(MSBuildBasedProject project, Guid guid)
		{
			project.AddProjectType(guid);
		}
		
		public static void AddReference(MSBuildBasedProject project, string referenceName)
		{
			var referenceProjectItem = new ReferenceProjectItem(project, referenceName);
			ProjectService.AddProjectItem(project, referenceProjectItem);
		}
		
		public static void AddFile(MSBuildBasedProject project, string fileName)
		{
			var fileProjectItem = new FileProjectItem(project, ItemType.Compile);
			fileProjectItem.FileName = FileName.Create(fileName);
			ProjectService.AddProjectItem(project, fileProjectItem);
		}
		
		public static ReferenceProjectItem GetReference(MSBuildBasedProject project, string referenceName)
		{
			foreach (ReferenceProjectItem referenceProjectItem in project.GetItemsOfType(ItemType.Reference)) {
				if (referenceProjectItem.Include == referenceName) {
					return referenceProjectItem;
				}
			}
			return null;
		}
		
		public static FileProjectItem GetFile(MSBuildBasedProject project, string fileName)
		{
			return project.FindFile(FileName.Create(fileName));
		}
		
		public static FileProjectItem GetFileFromInclude(TestableProject project, string include)
		{
			foreach (ProjectItem projectItem in project.Items) {
				FileProjectItem fileItem = projectItem as FileProjectItem;
				if (fileItem != null) {
					if (fileItem.Include == include) {
						return fileItem;
					}
				}
			}
			return null;
		}
	}
}
