// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.EnvDTE;
using DTE = ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ProjectItemTests
	{
		TestableDTEProject project;
		ProjectItems projectItems;
		TestableProject msbuildProject;
		FakeFileService fakeFileService;
		
		void CreateProjectItems()
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			projectItems = project.ProjectItems;
			fakeFileService = project.FakeFileService;
		}
		
		[Test]
		public void ProjectItems_ProjectHasOneFileInsideSrcDirectory_ReturnsOneFileForSrcDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItems directoryProjectItems = directoryItem.ProjectItems;
			
			string[] expectedFiles = new string[] {
				"program.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedFiles, directoryProjectItems);
		}
		
		[Test]
		public void ProjectItems_ProjectHasTestDirectoryInsideSrcDirectory_ReturnsTestDirectoryItemForSrcDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddDirectory(@"src\test");
		
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItems directoryProjectItems = directoryItem.ProjectItems;
			var items = directoryProjectItems as IEnumerable;
			
			string[] expectedItems = new string[] {
				"test"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void ProjectItems_ProjectHasTwoFilesOneNotInSrcDirectory_ReturnsOneFileItemForSrcDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\test.cs");
			msbuildProject.AddFile("program.cs");
		
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItems directoryProjectItems = directoryItem.ProjectItems;
			var items = directoryProjectItems as IEnumerable;
			
			string[] expectedItems = new string[] {
				"test.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void ProjectItems_ProjectHasOneFileInTestDirectoryTwoLevelsDeep_ReturnsOneFileItemForTestDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\test\test.cs");
		
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItem testDirectoryItem = directoryItem.ProjectItems.Item("test");
			ProjectItems testDirectoryProjectItems = testDirectoryItem.ProjectItems;
			var items = testDirectoryProjectItems as IEnumerable;
			
			string[] expectedItems = new string[] {
				"test.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void Kind_ProjectDirectory_ReturnsGuidForDirectory()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			ProjectItem directoryItem = projectItems.Item("src");
			
			string kind = directoryItem.Kind;
			
			Assert.AreEqual(Constants.VsProjectItemKindPhysicalFolder, kind);
		}
		
		[Test]
		public void Kind_ProjectFile_ReturnsGuidForFile()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			string kind = fileItem.Kind;
			
			Assert.AreEqual(Constants.VsProjectItemKindPhysicalFile, kind);
		}
	}
}
