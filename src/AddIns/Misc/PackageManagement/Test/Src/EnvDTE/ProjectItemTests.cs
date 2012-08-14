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
		
		[Test]
		public void Delete_ProjectItemIsFile_FileIsDeleted()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			msbuildProject.AddFile(@"src\program.cs");
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			fileItem.Delete();
			
			Assert.AreEqual(@"d:\projects\myproject\src\program.cs", project.FakeFileService.PathPassedToRemoveFile);
		}
		
		[Test]
		public void Delete_ProjectItemIsFile_ProjectIsSaved()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			msbuildProject.AddFile(@"src\program.cs");
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			fileItem.Delete();
			
			Assert.IsTrue(msbuildProject.IsSaved);
		}
		
		[Test]
		public void FileCodeModel_ProjectDirectory_ReturnsNull()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			ProjectItem directoryItem = projectItems.Item("src");
			
			FileCodeModel2 fileCodeModel = directoryItem.FileCodeModel;
			
			Assert.IsNull(fileCodeModel);
		}
		
		[Test]
		public void FileCodeModel_ProjectFile_ReturnsFileCodeModel()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\program.cs");
		
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			FileCodeModel2 fileCodeModel = fileItem.FileCodeModel;
			
			Assert.IsNotNull(fileCodeModel);
		}
		
		[Test]
		public void FileCodeModel_GetCodeElementsFromFileCodeModelForProjectFile_FileServicePassedToFileCodeModel()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"src\program.cs");
		
			ProjectItem directoryItem = projectItems.Item("src");
			ProjectItem fileItem = directoryItem.ProjectItems.Item("program.cs");
			
			CodeElements codeElements = fileItem.FileCodeModel.CodeElements;
			
			Assert.AreEqual(@"d:\projects\MyProject\src\program.cs", fakeFileService.FileNamePassedToGetCompilationUnit);
			Assert.AreEqual(0, codeElements.Count);
		}
		
		[Test]
		public void Remove_FileItemInProject_FileItemRemovedFromProject()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"program.cs");
			
			ProjectItem fileItem = projectItems.Item("program.cs");
			fileItem.Remove();
			
			Assert.AreEqual(0, msbuildProject.Items.Count);
		}
		
		[Test]
		public void Remove_FileItemInProject_ProjectIsSaved()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"program.cs");
			
			ProjectItem fileItem = projectItems.Item("program.cs");
			fileItem.Remove();
			
			Assert.IsTrue(msbuildProject.IsSaved);
		}
		
		[Test]
		public void FileNames_IndexOneRequested_ReturnsFullPathToProjectItem()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"src\program.cs");
			ProjectItem directoryItem = projectItems.Item("src");
			
			string fileName = directoryItem.FileNames(1);
			
			Assert.AreEqual(@"d:\projects\MyProject\src", fileName);
		}
	}
}
