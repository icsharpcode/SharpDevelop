// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ProjectItemsTests
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
		public void AddFromFileCopy_AddFileNameOutsideProjectFolder_FileIsIncludedInProjectInProjectFolder()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			projectItems.AddFromFileCopy(fileName);
			
			var fileItem = msbuildProject.Items[0] as FileProjectItem;
			
			Assert.AreEqual("test.cs", fileItem.Include);
		}
		
		[Test]
		public void AddFromFileCopy_AddFileNameOutsideProjectFolder_FileItemTypeTakenFromProject()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			projectItems.AddFromFileCopy(fileName);
			
			var fileItem = msbuildProject.Items[0] as FileProjectItem;
			
			Assert.AreEqual(ItemType.Page, fileItem.ItemType);
		}
		
		[Test]
		public void AddFromFileCopy_AddFileNameOutsideProjectFolder_FileNamePassedToDetermineFileItemType()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			projectItems.AddFromFileCopy(fileName);
			
			Assert.AreEqual("test.cs", msbuildProject.FileNamePassedToGetDefaultItemType);
		}
		
		[Test]
		public void AddFromFileCopy_AddFileNameOutsideProjectFolder_ProjectIsSaved()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			projectItems.AddFromFileCopy(fileName);
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void AddFromFileCopy_AddFileNameOutsideProjectFolder_FileIsCopied()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			projectItems.AddFromFileCopy(fileName);
			
			string[] expectedFileNames = new string[] {
				@"d:\projects\myproject\packages\tools\test.cs",
				@"d:\projects\myproject\myproject\test.cs"
			};
			
			string[] actualFileNames = new string[] {
				fakeFileService.OldFileNamePassedToCopyFile,
				fakeFileService.NewFileNamePassedToCopyFile
			};
			
			CollectionAssert.AreEqual(expectedFileNames, actualFileNames);
		}
	}
}
