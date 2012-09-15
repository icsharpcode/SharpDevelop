// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.Project
{
	[TestFixture]
	public class BeforeBuildCustomToolProjectItemsTests
	{
		ProjectHelper projectHelper;
		BeforeBuildCustomToolProjectItems beforeBuildCustomToolProjectItems;
		Solution solution;
		
		class UnknownBuildable : IBuildable
		{
			public string Name { get; set; }
			
			public Solution ParentSolution { get; set; }
			
			public ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
			{
				return new IBuildable[0];
			}
			
			public void StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
			{
			}
			
			public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
			{
				return new ProjectBuildOptions(BuildTarget.Build);
			}
		}
		
		IProject CreateProject(string fileName = @"d:\MyProject\MyProject.csproj")
		{
			CreateProjectWithNoProjectSpecifiedProperties(fileName);
			projectHelper.AddProjectSpecificProperties();
			return projectHelper.Project;
		}
		
		void CreateProjectWithNoProjectSpecifiedProperties(string fileName)
		{
			projectHelper = new ProjectHelper(fileName);
		}
		
		void CreateSolution(params IProject[] projects)
		{
			IProjectChangeWatcher watcher = MockRepository.GenerateStub<IProjectChangeWatcher>();
			solution = new Solution(watcher);
			projects.ForEach(p => solution.Folders.Add(p));
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
			CreateBeforeBuildCustomToolProjectItems(projectHelper.Project as IBuildable);
		}
		
		void CreateBeforeBuildCustomToolProjectItems(IBuildable buildable)
		{
			beforeBuildCustomToolProjectItems = new BeforeBuildCustomToolProjectItems(buildable);
		}
		
		void CreateBeforeBuildCustomToolProjectItemsUsingSolution()
		{
			CreateBeforeBuildCustomToolProjectItems(solution as IBuildable);
		}
		
		FileProjectItem AddFileToProject(string include)
		{
			var projectItem = new FileProjectItem(projectHelper.Project, ItemType.Compile, include);
			projectHelper.AddProjectItem(projectItem);
			return projectItem;
		}
		
		void CreateBeforeBuildCustomToolProjectItemsWithUnknownIBuildableDerivedClass()
		{
			var unknownBuildable = new UnknownBuildable();
			CreateBeforeBuildCustomToolProjectItems(unknownBuildable);
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
		
		[Test]
		public void GetProjectItems_ProjectSpecificPropertiesIsNull_NoProjectItemsReturnedAndNoNullReferenceExceptionThrown()
		{
			CreateProjectWithNoProjectSpecifiedProperties(@"d:\MyProject\FirstProject.csproj");
			CreateBeforeBuildCustomToolProjectItems();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			Assert.AreEqual(0, projectItems.Count);
		}
		
		[Test]
		public void GetProjectItems_UnknownIBuildableDerivedClass_NullReferenceExceptionIsNotThrown()
		{
			CreateBeforeBuildCustomToolProjectItemsWithUnknownIBuildableDerivedClass();
			
			List<FileProjectItem> projectItems = GetProjectItems();
			
			Assert.AreEqual(0, projectItems.Count);
		}
	}
}
