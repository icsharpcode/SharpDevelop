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
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Project
{
	[TestFixture]
	public class BeforeBuildCustomToolProjectItemsTests
	{
		ProjectHelper projectHelper;
		BeforeBuildCustomToolProjectItems beforeBuildCustomToolProjectItems;
		ISolution solution;
		
		IProject CreateProject(string fileName = @"d:\MyProject\MyProject.csproj")
		{
			projectHelper = new ProjectHelper(fileName);
			return projectHelper.Project;
		}
		
		void CreateSolution(params IProject[] projects)
		{
			solution = MockRepository.GenerateStrictMock<ISolution>();
			solution.Stub(s => s.Projects).Return(new SimpleModelCollection<IProject>(projects));
		}
		
		void ConfigureCustomToolFileNamesForProject(string fileNames)
		{
			var customToolOptions = new ProjectCustomToolOptions(projectHelper.Project);
			customToolOptions.FileNames = fileNames;
		}
		
		void EnableCustomToolRunForProject()
		{
			SetCustomToolRunForProject(true);
		}
		
		void SetCustomToolRunForProject(bool enabled)
		{
			var customToolOptions = new ProjectCustomToolOptions(projectHelper.Project);
			customToolOptions.RunCustomToolOnBuild = enabled;
		}
		
		void DisableCustomToolRunForProject()
		{
			SetCustomToolRunForProject(false);
		}
		
		List<FileProjectItem> GetProjectItems()
		{
			return beforeBuildCustomToolProjectItems.GetProjectItems().ToList();
		}
		
		void CreateBeforeBuildCustomToolProjectItems()
		{
			CreateBeforeBuildCustomToolProjectItems(new[] { projectHelper.Project });
		}
		
		void CreateBeforeBuildCustomToolProjectItems(IReadOnlyList<IProject> projects)
		{
			beforeBuildCustomToolProjectItems = new BeforeBuildCustomToolProjectItems(projects);
		}
		
		void CreateBeforeBuildCustomToolProjectItemsUsingSolution()
		{
			CreateBeforeBuildCustomToolProjectItems(solution.Projects.ToList());
		}
		
		FileProjectItem AddFileToProject(string include)
		{
			var projectItem = new FileProjectItem(projectHelper.Project, ItemType.Compile, include);
			projectHelper.Project.Items.Add(projectItem);
			return projectItem;
		}
		
		[Test]
		public void GetProjectItems_BuildSingleProjectNotConfiguredToRunCustomToolsOnBuild_ReturnsNoItems()
		{
			CreateProject();
			CreateBeforeBuildCustomToolProjectItems();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			Assert.AreEqual(0, projectItems.Count);
		}
		
		[Test]
		public void GetProjectItems_BuildSingleProjectWithOneFileMatchingCustomToolRunConfiguration_OneProjectItemReturned()
		{
			CreateProject(@"d:\MyProject\MyProject.csproj");
			FileProjectItem projectItem = AddFileToProject("template.tt");
			EnableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("template.tt");
			CreateBeforeBuildCustomToolProjectItems();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			FileProjectItem[] expectedProjectItems = new FileProjectItem[] {
				projectItem
			};
			CollectionAssert.AreEqual(expectedProjectItems, projectItems);
		}
		
		[Test]
		public void GetProjectItems_BuildSingleProjectWithOneFileMatchingCustomToolFileNamesConfigured_NoProjectItemsReturnedWhenRunCustomToolIsDisabledForProject()
		{
			CreateProject(@"d:\MyProject\MyProject.csproj");
			AddFileToProject("template.tt");
			DisableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("template.tt");
			CreateBeforeBuildCustomToolProjectItems();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			Assert.AreEqual(0, projectItems.Count);
		}
		
		[Test]
		public void GetProjectItems_BuildSingleProjectWithOneFileMatchingCustomToolRunConfiguration_OtherNonMatchingProjectItemsNotReturned()
		{
			CreateProject(@"d:\MyProject\MyProject.csproj");
			FileProjectItem projectItem = AddFileToProject("template.t4");
			AddFileToProject("test.cs");
			EnableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("template.t4");
			CreateBeforeBuildCustomToolProjectItems();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			FileProjectItem[] expectedProjectItems = new FileProjectItem[] {
				projectItem
			};
			CollectionAssert.AreEqual(expectedProjectItems, projectItems);
		}
		
		[Test]
		public void GetProjectItems_BuildSingleProjectWithOneFileMatchingCustomToolRunConfiguration_ProjectItemInSubdirectoryReturned()
		{
			CreateProject(@"d:\MyProject\MyProject.csproj");
			FileProjectItem projectItem = AddFileToProject(@"Model\template.tt");
			EnableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("template.tt");
			CreateBeforeBuildCustomToolProjectItems();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			FileProjectItem[] expectedProjectItems = new FileProjectItem[] {
				projectItem
			};
			CollectionAssert.AreEqual(expectedProjectItems, projectItems);
		}
		
		[Test]
		public void GetProjectItems_BuildSingleProjectWithOneFileMatchingCustomToolRunConfiguration_ProjectItemReturnedWhenFileNameCaseIsDifferent()
		{
			CreateProject(@"d:\MyProject\MyProject.csproj");
			FileProjectItem projectItem = AddFileToProject("template.tt");
			EnableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("TEMPLATE.TT");
			CreateBeforeBuildCustomToolProjectItems();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			FileProjectItem[] expectedProjectItems = new FileProjectItem[] {
				projectItem
			};
			CollectionAssert.AreEqual(expectedProjectItems, projectItems);
		}
		
		[Test]
		public void GetProjectItems_SolutionContainingOneProjectWithMatchingCustomToolFileName_ReturnsOneProjectItem()
		{
			IProject project = CreateProject(@"d:\MyProject\MyProject.csproj");
			FileProjectItem projectItem = AddFileToProject("template.tt");
			EnableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("TEMPLATE.TT");
			CreateSolution(project);
			CreateBeforeBuildCustomToolProjectItemsUsingSolution();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			FileProjectItem[] expectedProjectItems = new FileProjectItem[] {
				projectItem
			};
			CollectionAssert.AreEqual(expectedProjectItems, projectItems);
		}
		
		[Test]
		public void GetProjectItems_SolutionWithNoProjects_ReturnsNoProjectItems()
		{
			CreateSolution();
			CreateBeforeBuildCustomToolProjectItemsUsingSolution();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			Assert.AreEqual(0, projectItems.Count);
		}
		
		[Test]
		public void GetProjectItems_SolutionContainingTwoProjectsWithMatchingCustomToolFileNameInSecondProject_ReturnsOneProjectItem()
		{
			IProject project1 = CreateProject(@"d:\MyProject\FirstProject.csproj");
			IProject project2 = CreateProject(@"d:\MyProject\SecondProject.csproj");
			FileProjectItem projectItem = AddFileToProject("template.tt");
			EnableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("TEMPLATE.TT");
			CreateSolution(project1, project2);
			CreateBeforeBuildCustomToolProjectItemsUsingSolution();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			FileProjectItem[] expectedProjectItems = new FileProjectItem[] {
				projectItem
			};
			CollectionAssert.AreEqual(expectedProjectItems, projectItems);
		}
		
		[Test]
		public void GetProjectItems_SolutionContainingTwoProjectsWithMatchingCustomToolFileNameInFirstProject_ReturnsOneProjectItem()
		{
			IProject project1 = CreateProject(@"d:\MyProject\FirstProject.csproj");
			FileProjectItem projectItem = AddFileToProject("template.tt");
			EnableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("TEMPLATE.TT");
			IProject project2 = CreateProject(@"d:\MyProject\SecondProject.csproj");
			CreateSolution(project1, project2);
			CreateBeforeBuildCustomToolProjectItemsUsingSolution();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			FileProjectItem[] expectedProjectItems = new FileProjectItem[] {
				projectItem
			};
			CollectionAssert.AreEqual(expectedProjectItems, projectItems);
		}
		
		[Test]
		public void GetProjectItems_SolutionContainingTwoProjectsBothWithFilesAndMatchingCustomToolFileNameInFirstProject_ReturnsOneProjectItem()
		{
			IProject project1 = CreateProject(@"d:\MyProject\FirstProject.csproj");
			FileProjectItem projectItem = AddFileToProject("template.tt");
			EnableCustomToolRunForProject();
			ConfigureCustomToolFileNamesForProject("TEMPLATE.TT");
			IProject project2 = CreateProject(@"d:\MyProject\SecondProject.csproj");
			AddFileToProject("test.cs");
			CreateSolution(project1, project2);
			CreateBeforeBuildCustomToolProjectItemsUsingSolution();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			FileProjectItem[] expectedProjectItems = new FileProjectItem[] {
				projectItem
			};
			CollectionAssert.AreEqual(expectedProjectItems, projectItems);
		}
	}
}
