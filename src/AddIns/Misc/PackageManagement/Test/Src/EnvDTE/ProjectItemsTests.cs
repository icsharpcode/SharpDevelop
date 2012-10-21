// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.VisualStudio;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;
using DTE = ICSharpCode.PackageManagement.EnvDTE;

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
			projectItems = (ProjectItems)project.ProjectItems;
			fakeFileService = project.FakeFileService;
		}
		
		void CreateProjectItemsFromProjectSubFolder(string projectFileName, string folderName)
		{
			CreateProjectItems();
			msbuildProject.FileName = projectFileName;
			msbuildProject.AddDirectory(folderName);
			projectItems = (ProjectItems)project.ProjectItems.Item(folderName).ProjectItems;
		}
		
		void AddFileToFakeFileSystem(string directory, string relativeFileName)
		{
			fakeFileService.AddFilesToFakeFileSystem(directory, relativeFileName);
		}
		
		void AddDirectoryToFakeFileSystem(string parentDirectory, string childDirectory)
		{
			fakeFileService.AddDirectoryToFakeFileSystem(parentDirectory, childDirectory);
		}
		
		IProjectBrowserUpdater CreateProjectBrowserUpdater()
		{
			IProjectBrowserUpdater projectBrowserUpdater = MockRepository.GenerateStub<IProjectBrowserUpdater>();
			project.FakeProjectService.ProjectBrowserUpdater = projectBrowserUpdater;
			return projectBrowserUpdater;
		}
		
		DTE.ProjectItem GetChildItem(global::EnvDTE.ProjectItems projectItems, string name)
		{
			return projectItems
				.OfType<DTE.ProjectItem>()
				.SingleOrDefault(item => item.Name == name);
		}
		
		DTE.ProjectItem GetFirstChildItem(global::EnvDTE.ProjectItems projectItems)
		{
			return projectItems.OfType<DTE.ProjectItem>().FirstOrDefault();
		}
		
		List<DTE.ProjectItem> GetAllChildItems(global::EnvDTE.ProjectItems projectItems)
		{
			return projectItems.OfType<DTE.ProjectItem>().ToList();
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
		
		[Test]
		public void AddFromFileCopy_AddFileNameOutsideProjectFolderFromSubFolderProjectItems_FileIsIncludedInProjectInSubFolder()
		{
			string projectFileName = @"d:\projects\myproject\myproject\myproject.csproj";
			CreateProjectItemsFromProjectSubFolder(projectFileName, "SubFolder");
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			projectItems.AddFromFileCopy(fileName);
			
			string addedFileName = @"d:\projects\myproject\myproject\SubFolder\test.cs";
			FileProjectItem fileItem = msbuildProject.FindFile(addedFileName);
			
			Assert.AreEqual(@"SubFolder\test.cs", fileItem.Include);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasTwoFiles_TwoFilesReturned()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"Test.cs");
			
			var expectedItems = new string[] {
				"Test.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, projectItems);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneFile_OneFileReturned()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"Program.cs");
			
			var expectedFiles = new string[] {
				"Program.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedFiles, projectItems);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneFileAndOneReference_OneFileReturned()
		{
			CreateProjectItems();
			msbuildProject.AddReference("NUnit.Framework");
			msbuildProject.AddFile(@"Program.cs");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedFiles = new string[] {
				"Program.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedFiles, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneFileInSubDirectory_OneFolderReturned()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\Program.cs");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedItems = new string[] {
				"src"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasTwoFilesInDifferentSubDirectories_TwoFoldersReturned()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"Controllers\Program.cs");
			msbuildProject.AddFile(@"ViewModels\Program.cs");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedItems = new string[] {
				"Controllers",
				"ViewModels"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasTwoFilesInSameSubDirectory_OneFolderReturned()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"Controllers\Tests\Program1.cs");
			msbuildProject.AddFile(@"Controllers\Tests\Program2.cs");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedItems = new string[] {
				"Controllers",
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneFolderAndOneFileInSameSubDirectory_OneFileAndOneFolderReturned()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"Controllers\Program.cs");
			msbuildProject.AddDirectory(@"Controllers");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedItems = new string[] {
				"Controllers",
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneFolderAndOneFileInSameSubDirectoryWithDirectoryFirstInProject_OneFileAndOneFolderReturned()
		{
			CreateProjectItems();
			msbuildProject.AddDirectory(@"Controllers");
			msbuildProject.AddFile(@"Controllers\Program.cs");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedItems = new string[] {
				"Controllers",
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneLinkedFile_OneFileReturned()
		{
			CreateProjectItems();
			var fileItem = msbuildProject.AddFile(@"..\..\Program.cs");
			fileItem.SetMetadata("Link", "MyProgram.cs");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedFiles = new string[] {
				"Program.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedFiles, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasFileWithRelativePath_FileIsTreatedAsLinkAndReturned()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"..\..\Program.cs");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedFiles = new string[] {
				"Program.cs"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedFiles, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneLinkedFileInProjectSubDirectoryAndOneDirectory_OneDirectoryReturned()
		{
			CreateProjectItems();
			msbuildProject.AddDirectory("Configuration");
			var fileItem = msbuildProject.AddFile(@"..\..\AssemblyInfo.cs");
			fileItem.SetMetadata("Link", @"Configuration\MyAssemblyInfo.cs");
			
			var enumerable = projectItems as IEnumerable;
			
			var expectedItems = new string[] {
				"Configuration"
			};
			
			ProjectItemCollectionAssert.AreEqual(expectedItems, enumerable);
		}
		
		[Test]
		public void Item_GetProjectItemByName_ReturnsFileInsideProject()
		{
			CreateProjectItems();
			msbuildProject.AddFile("Program.cs");
			
			var projectItem = projectItems.Item("Program.cs") as DTE.ProjectItem;
			string projectItemName = projectItem.Name;
			
			Assert.AreEqual("Program.cs", projectItemName);
		}
		
		[Test]
		public void Item_GetProjectItemByNameWhenNameCaseDoesNotMatchFileNameInProject_ReturnsFileInsideProject()
		{
			CreateProjectItems();
			msbuildProject.AddFile("Program.cs");
			
			var projectItem = projectItems.Item("PROGRAM.CS") as DTE.ProjectItem;
			string projectItemName = projectItem.Name;
			
			Assert.AreEqual("Program.cs", projectItemName);
		}
		
		[Test]
		public void Kind_FileProjectItems_ReturnsItemKindPhysicalFile()
		{
			CreateProjectItems();
			msbuildProject.AddFile("program.cs");
			
			string kind = projectItems
				.Item("program.cs")
				.ProjectItems
				.Kind;
			
			Assert.AreEqual(global::EnvDTE.Constants.vsProjectItemKindPhysicalFile, kind);
		}
		
		[Test]
		public void Kind_ProjectItemsForProject_ReturnsItemKindPhysicalFolder()
		{
			CreateProjectItems();
			msbuildProject.AddFile("program.cs");
			
			string kind = projectItems.Kind;
			
			Assert.AreEqual(global::EnvDTE.Constants.vsProjectItemKindPhysicalFolder, kind);
		}
		
		[Test]
		public void AddFileFromCopy_FileAlreadyExistsOnFileSystem_ThrowsException()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			string existingFile = @"d:\projects\myproject\myproject\test.cs";
			fakeFileService.ExistingFileNames.Add(existingFile);
			
			Exception ex = 
				Assert.Throws(typeof(FileExistsException), () => projectItems.AddFromFileCopy(fileName));
			
			bool contains = ex.Message.Contains("'test.cs'");
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void Count_ProjectHasOneFile_ReturnsOne()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"Test.cs");
			
			int count = projectItems.Count;
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void Parent_GetProjectItemsParentForFileProjectItem_ReturnsFileProjectItem()
		{
			CreateProjectItems();
			msbuildProject.AddFile("Program.cs");
			
			var projectItem = projectItems.Item("program.cs") as DTE.ProjectItem;
			object parent = projectItem.ProjectItems.Parent;
			
			Assert.AreEqual(projectItem, parent);
		}
		
		[Test]
		public void Parent_GetProjectItemsParentForDirectoryProjectItem_ReturnsDirectoryProjectItem()
		{
			CreateProjectItems();
			msbuildProject.AddFile(@"src\Program.cs");
			var projectItem = projectItems.Item("src") as DTE.ProjectItem;
			
			object parent = projectItem.ProjectItems.Parent;			

			Assert.AreEqual(projectItem, parent);
		}
		
		[Test]
		public void Item_GetProjectItemByIndex_ReturnsFileInsideProject()
		{
			CreateProjectItems();
			msbuildProject.AddFile("Program.cs");
			
			var projectItem = projectItems.Item(1) as DTE.ProjectItem;
			string projectItemName = projectItem.Name;
			
			Assert.AreEqual("Program.cs", projectItemName);
		}
		
		[Test]
		public void AddFromFile_FullFileNameIsInsideProject_FileIsAddedToProject()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\tools\test.cs";
			
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			projectItems.AddFromFile(fileName);
			
			var fileItem = msbuildProject.Items[0] as FileProjectItem;
			
			Assert.AreEqual(@"tools\test.cs", fileItem.Include);
			Assert.AreEqual(@"d:\projects\myproject\tools\test.cs", fileItem.FileName);
			Assert.AreEqual(ItemType.Page, fileItem.ItemType);
			Assert.AreEqual(msbuildProject, fileItem.Project);
		}
		
		[Test]
		public void AddFromFile_FullFileNameIsInsideProject_ProjectIsSaved()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			projectItems.AddFromFile(fileName);
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void AddFromFile_FullFileNameIsInsideProject_FileIsParsed()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\packages\tools\test.cs";
			
			projectItems.AddFromFile(fileName);
			
			string parsedFileName = fakeFileService.FileNamePassedToParseFile;
			
			Assert.AreEqual(fileName, parsedFileName);
		}
		
		[Test]
		public void AddFromFile_FullFileNameIsInsideProject_ProjectItemReturned()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\tools\test.cs";
			
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			global::EnvDTE.ProjectItem item = projectItems.AddFromFile(fileName);
			
			string fullPath = (string)item.Properties.Item("FullPath").Value;
			
			Assert.AreEqual("test.cs", item.Name);
			Assert.AreEqual(@"d:\projects\myproject\tools\test.cs", fullPath);
		}
		
		[Test]
		public void AddFromFile_FullFileNameIsInsideProject_FileNameUsedToDetermineProjectItemType()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\tools\test.cs";
			
			projectItems.AddFromFile(fileName);
			
			Assert.AreEqual("test.cs", msbuildProject.FileNamePassedToGetDefaultItemType);
		}
		
		[Test]
		public void AddFromDirectory_EmptyDirectoryInsideProject_ProjectIsSaved()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string directory = @"d:\projects\myproject\tools";
			
			projectItems.AddFromDirectory(directory);
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void AddFromDirectory_EmptyDirectoryInsideProject_ProjectItemIsReturnedForNewDirectory()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string directory = @"d:\projects\myproject\tools";
			
			global::EnvDTE.ProjectItem item = projectItems.AddFromDirectory(directory);
			string name = item.Name;
			
			Assert.AreEqual("tools", name);
			Assert.AreEqual(project, item.ContainingProject);
			Assert.AreEqual(global::EnvDTE.Constants.vsProjectItemKindPhysicalFolder, item.Kind);
		}
		
		[Test]
		public void AddFromDirectory_EmptyDirectoryInsideProject_FolderProjectItemAddedToProject()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string directory = @"d:\projects\myproject\tools";
			
			projectItems.AddFromDirectory(directory);
			
			var item = msbuildProject.Items[0] as FileProjectItem;
			
			Assert.AreEqual("tools", item.Include);
			Assert.AreEqual(ItemType.Folder, item.ItemType);
		}
		
		[Test]
		public void AddFromDirectory_EmptyDirectoryIsTwoLevelsInsideProject_FolderProjectItemAddedToProject()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string directory = @"d:\projects\myproject\tools\packages";
			
			projectItems.AddFromDirectory(directory);
			
			var item = msbuildProject.Items[0] as FileProjectItem;
			
			Assert.AreEqual(@"tools\packages", item.Include);
			Assert.AreEqual(ItemType.Folder, item.ItemType);
		}
		
		[Test]
		public void AddFromDirectory_DirectoryContainsOneFile_FileAddedToMSBuildProjectButNoFolder()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string directory = @"d:\projects\myproject\tools";
			AddFileToFakeFileSystem(directory, "a.txt");
			project.TestableProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.None;
			
			projectItems.AddFromDirectory(directory);
			
			var item = msbuildProject.Items[0] as FileProjectItem;
			
			Assert.AreEqual(@"tools\a.txt", item.Include);
			Assert.AreEqual(ItemType.None, item.ItemType);
			Assert.AreEqual(@"d:\projects\myproject\tools\a.txt", item.FileName);
			Assert.AreEqual(1, msbuildProject.Items.Count);
		}
		
		[Test]
		public void AddFromDirectory_DirectoryContainsOneFile_ProjectItemReturnedHasDirectoryName()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string directory = @"d:\projects\myproject\tools";
			AddFileToFakeFileSystem(directory, "a.txt");
			
			global::EnvDTE.ProjectItem item = projectItems.AddFromDirectory(directory);
			string name = item.Name;
			
			Assert.AreEqual("tools", name);
			Assert.AreEqual(project, item.ContainingProject);
			Assert.AreEqual(global::EnvDTE.Constants.vsProjectItemKindPhysicalFolder, item.Kind);
		}
		
		[Test]
		public void AddFromDirectory_DirectoryContainsChildDirectoryWithNoFiles_DirectoryProjectItemReturnedForParentDirectory()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string parentDirectory = @"d:\projects\myproject\tools";
			AddDirectoryToFakeFileSystem(parentDirectory, "packages");
			
			global::EnvDTE.ProjectItem item = projectItems.AddFromDirectory(parentDirectory);
			string name = item.Name;
			
			global::EnvDTE.ProjectItem childItem = item.ProjectItems.Item("packages");
			
			Assert.AreEqual("tools", name);
			Assert.AreEqual(project, item.ContainingProject);
			Assert.AreEqual(global::EnvDTE.Constants.vsProjectItemKindPhysicalFolder, item.Kind);
			Assert.AreEqual(1, item.ProjectItems.Count);
			Assert.AreEqual(global::EnvDTE.Constants.vsProjectItemKindPhysicalFolder, childItem.Kind);
		}
		
		[Test]
		public void AddFromDirectory_DirectoryContainsChildDirectoryWithNoFiles_MSBuildProjectItemAddedForChildDirectoryButNotParent()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string parentDirectory = @"d:\projects\myproject\tools";
			AddDirectoryToFakeFileSystem(parentDirectory, "packages");
			
			projectItems.AddFromDirectory(parentDirectory);
			
			var item = msbuildProject.Items[0] as FileProjectItem;
			
			Assert.AreEqual(@"tools\packages", item.Include);
			Assert.AreEqual(ItemType.Folder, item.ItemType);
			Assert.AreEqual(@"d:\projects\myproject\tools\packages", item.FileName);
			Assert.AreEqual(1, msbuildProject.Items.Count);
		}
		
		[Test]
		public void AddFromDirectory_DirectoryContainsChildDirectoryWithOneFile_MSBuildProjectItemAddedForFileButNotForParentNorChildDirectory()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string parentDirectory = @"d:\projects\myproject\tools";
			string childDirectory = @"d:\projects\myproject\tools\packages";
			AddDirectoryToFakeFileSystem(parentDirectory, "packages");
			AddFileToFakeFileSystem(childDirectory, "a.txt");
			project.TestableProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.None;			
			
			projectItems.AddFromDirectory(parentDirectory);
			
			var item = msbuildProject.Items[0] as FileProjectItem;
			
			Assert.AreEqual(@"tools\packages\a.txt", item.Include);
			Assert.AreEqual(ItemType.None, item.ItemType);
			Assert.AreEqual(@"d:\projects\myproject\tools\packages\a.txt", item.FileName);
			Assert.AreEqual(1, msbuildProject.Items.Count);
		}
		
		[Test]
		public void AddFromFile_FullFileNameIsInsideProject_ProjectBrowserUpdaterIsDisposed()
		{
			CreateProjectItems();
			IProjectBrowserUpdater projectBrowserUpdater = CreateProjectBrowserUpdater();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\tools\test.cs";
			
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			projectItems.AddFromFile(fileName);
			
			projectBrowserUpdater.AssertWasCalled(updater => updater.Dispose());
		}
		
		[Test]
		public void AddFromDirectory_EmptyDirectoryInsideProject_ProjectBrowserUpdaterIsDisposed()
		{
			CreateProjectItems();
			IProjectBrowserUpdater projectBrowserUpdater = CreateProjectBrowserUpdater();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string directory = @"d:\projects\myproject\tools";
			
			projectItems.AddFromDirectory(directory);
			
			bool saved = msbuildProject.IsSaved;
			
			projectBrowserUpdater.AssertWasCalled(updater => updater.Dispose());
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneFileInFolderTwoLevelsDeep_FolderTwoLevelsDeepFullPathIsFullDirectoryName()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\Program.cs");
			
			DTE.ProjectItem codeTemplatesFolderItem = GetChildItem(projectItems, "CodeTemplates");
			DTE.ProjectItem scaffolderFolderItem = GetChildItem(codeTemplatesFolderItem.ProjectItems, "Scaffolders");
			
			string directory = (string)scaffolderFolderItem.Properties.Item(DTE.ProjectItem.FullPathPropertyName).Value;
			
			string expectedDirectory = @"d:\projects\MyProject\CodeTemplates\Scaffolders";
			Assert.AreEqual(expectedDirectory, directory);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasOneFileInFolderThreeLevelsDeep_FileReturnedInProjectItems()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\jQueryPlugin\jQueryPlugin.ps1");
			DTE.ProjectItem codeTemplatesFolderItem = GetChildItem(projectItems, "CodeTemplates");
			DTE.ProjectItem scaffolderFolderItem = GetChildItem(codeTemplatesFolderItem.ProjectItems, "Scaffolders");
			DTE.ProjectItem jqueryPluginFolderItem = GetChildItem(scaffolderFolderItem.ProjectItems, "jQueryPlugin");
			
			DTE.ProjectItem jqueryPluginFileItem = GetFirstChildItem(jqueryPluginFolderItem.ProjectItems);
			
			Assert.AreEqual("jQueryPlugin.ps1", jqueryPluginFileItem.Name);
		}
		
		[Test]
		public void Item_GetFileProjectItemByNameWhenProjectHasOneFileInFolderThreeLevelsDeep_ReturnsFileProjectItem()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\jQueryPlugin\jQueryPlugin.ps1");
			DTE.ProjectItem codeTemplatesFolderItem = GetChildItem(projectItems, "CodeTemplates");
			DTE.ProjectItem scaffolderFolderItem = GetChildItem(codeTemplatesFolderItem.ProjectItems, "Scaffolders");
			DTE.ProjectItem jqueryPluginFolderItem = GetChildItem(scaffolderFolderItem.ProjectItems, "jQueryPlugin");
			
			global::EnvDTE.ProjectItem jqueryPluginFileItem = jqueryPluginFolderItem.ProjectItems.Item("jQueryPlugin.ps1");
			
			Assert.AreEqual("jQueryPlugin.ps1", jqueryPluginFileItem.Name);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasTwoFilesInFolderTwoLevelsDeep_TopLevelFolderOnlyHasOneProjectItemForChildFolder()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\File1.cs");
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\File2.cs");
			
			DTE.ProjectItem codeTemplatesFolderItem = GetChildItem(projectItems, "CodeTemplates");
			List<DTE.ProjectItem> childItems = GetAllChildItems(codeTemplatesFolderItem.ProjectItems);
			DTE.ProjectItem scaffoldersFolderItem = childItems.FirstOrDefault();
			
			Assert.AreEqual(1, childItems.Count);
			Assert.AreEqual("Scaffolders", scaffoldersFolderItem.Name);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasTwoFilesInFolderTwoLevelsDeepAndProjectItemsForChildFolderCalledTwice_TopLevelFolderOnlyHasOneProjectItemForChildFolder()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\File1.cs");
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\File2.cs");
			
			DTE.ProjectItem codeTemplatesFolderItem = GetChildItem(projectItems, "CodeTemplates");
			List<DTE.ProjectItem> childItems = GetAllChildItems(codeTemplatesFolderItem.ProjectItems);
			// Call ProjectItems again.
			childItems = GetAllChildItems(codeTemplatesFolderItem.ProjectItems);
			DTE.ProjectItem scaffoldersFolderItem = childItems.FirstOrDefault();
			
			Assert.AreEqual(1, childItems.Count);
			Assert.AreEqual("Scaffolders", scaffoldersFolderItem.Name);
		}

		[Test]
		public void GetEnumerator_ProjectHasTwoDuplicateFileNamesInFolderTwoLevelsDeep_FolderTwoLevelsDownOnlyReturnsOneFileProjectItem()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\duplicate.cs");
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\duplicate.cs");
			
			DTE.ProjectItem codeTemplatesFolderItem = GetChildItem(projectItems, "CodeTemplates");
			DTE.ProjectItem scaffolderFolderItem = GetChildItem(codeTemplatesFolderItem.ProjectItems, "Scaffolders");
			List<DTE.ProjectItem> childItems = GetAllChildItems(scaffolderFolderItem.ProjectItems);
			DTE.ProjectItem duplicateFileItem = GetFirstChildItem(scaffolderFolderItem.ProjectItems);
			
			Assert.AreEqual(1, childItems.Count);
			Assert.AreEqual("duplicate.cs", duplicateFileItem.Name);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasTwoFilesInFolderTwoLevelsDeepButParentFolderHasDifferentCase_TopLevelFolderOnlyHasOneProjectItemForChildFolder()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"CodeTemplates\Scaffolders\File1.cs");
			msbuildProject.AddFile(@"CodeTemplates\SCAFFOLDERS\File2.cs");
			
			DTE.ProjectItem codeTemplatesFolderItem = GetChildItem(projectItems, "CodeTemplates");
			List<DTE.ProjectItem> childItems = GetAllChildItems(codeTemplatesFolderItem.ProjectItems);
			DTE.ProjectItem scaffoldersFolderItem = childItems.FirstOrDefault();
			
			Assert.AreEqual(1, childItems.Count);
			Assert.AreEqual("Scaffolders", scaffoldersFolderItem.Name);
		}
		
		[Test]
		public void AddFromFile_FullFileNameIsOutsideProjectPath_FileIsAddedToProjectAsLink()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\anotherproject\test.cs";
			
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			projectItems.AddFromFile(fileName);
			
			var fileItem = msbuildProject.Items[0] as FileProjectItem;
			string linkName = fileItem.GetEvaluatedMetadata("Link");
			
			Assert.AreEqual(@"..\anotherproject\test.cs", fileItem.Include);
			Assert.AreEqual(fileName, fileItem.FileName);
			Assert.AreEqual(ItemType.Page, fileItem.ItemType);
			Assert.IsTrue(fileItem.IsLink);
			Assert.AreEqual("test.cs", linkName);
		}
		
		[Test]
		public void Item_GetItemFromControllersFolderProjectItemsWhenProjectHasTwoFilesOneInRootAndOneInControllersFolder_ReturnsFileFromControllersFolder()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\MyProject\MyProject.csproj";
			msbuildProject.AddFile(@"AccountController.generated.cs");
			msbuildProject.AddFile(@"Controllers\AccountController.cs");
			
			global::EnvDTE.ProjectItem projectItem = projectItems
				.Item("Controllers")
				.ProjectItems
				.Item(1);
			
			Assert.AreEqual("AccountController.cs", projectItem.Name);
		}
		
		[Test]
		public void Item_UnknownProjectItemName_ThrowsException()
		{
			CreateProjectItems();
			msbuildProject.AddFile("Program.cs");
			
			Assert.Throws<ArgumentException>(() => projectItems.Item("unknown.cs"));
		}
		
		[Test]
		public void AddFromFile_AddFromFileFromProjectItemsBelongingToFile_FileIsAddedAsDependentFile()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			msbuildProject.AddFile("MainForm.cs");
			string fileName = @"d:\projects\myproject\MainForm.Designer.cs";
			msbuildProject.ItemTypeToReturnFromGetDefaultItemType = ItemType.Page;
			projectItems = (ProjectItems)project.ProjectItems.Item("MainForm.cs").ProjectItems;
			
			projectItems.AddFromFile(fileName);
			
			FileProjectItem fileItem = msbuildProject.FindFile(fileName);
			
			Assert.AreEqual("MainForm.Designer.cs", fileItem.Include);
			Assert.AreEqual(fileName, fileItem.FileName);
			Assert.AreEqual(ItemType.Page, fileItem.ItemType);
			Assert.AreEqual(msbuildProject, fileItem.Project);
			Assert.AreEqual("MainForm.cs", fileItem.DependentUpon);
		}
		
		[Test]
		public void AddFromFileCopy_AddExistingFileNameInsideProjectFolder_FileIsAddedToProject()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\test.cs";
			fakeFileService.ExistingFileNames.Add(fileName);
			
			projectItems.AddFromFileCopy(fileName);
			
			string addedFileName = @"d:\projects\myproject\test.cs";
			FileProjectItem fileItem = msbuildProject.FindFile(addedFileName);
			Assert.AreEqual("test.cs", fileItem.Include);
		}
		
		[Test]
		public void AddFromFileCopy_AddExistingFileNameInsideProjectFolder_ProjectUpdaterIsDisposed()
		{
			CreateProjectItems();
			IProjectBrowserUpdater projectBrowserUpdater = CreateProjectBrowserUpdater();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\test.cs";
			fakeFileService.ExistingFileNames.Add(fileName);
			
			projectItems.AddFromFileCopy(fileName);
			
			projectBrowserUpdater.AssertWasCalled(updater => updater.Dispose());
		}
		
		[Test]
		public void AddFromFileCopy_AddNonExistentFileNameInsideProjectFolder_MissingFileExceptionThrown()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			string fileName = @"d:\projects\myproject\test.cs";
			
			FileNotFoundException ex = Assert.Throws<FileNotFoundException>(() => projectItems.AddFromFileCopy(fileName));
			
			Assert.AreEqual("Cannot find file", ex.Message);
			Assert.AreEqual(fileName, ex.FileName);
		}
		
		[Test]
		public void AddFromFileCopy_AddResxFileWhenParentFileExistsInProject_FileIsAddedToProjectAsDependentFile()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			msbuildProject.AddFile("MainForm.cs");
			string fileToAdd = @"d:\projects\myproject\MainForm.resx";
			fakeFileService.ExistingFileNames.Add(fileToAdd);
			
			projectItems.AddFromFileCopy(fileToAdd);
			
			FileProjectItem fileItem = msbuildProject.FindFile(fileToAdd);
			Assert.AreEqual("MainForm.resx", fileItem.Include);
			Assert.AreEqual("MainForm.cs", fileItem.DependentUpon);
		}
		
		[Test]
		public void AddFromFileCopy_AddDesignerFileInProjectSubFolderWhenParentFileExistsInProject_FileIsAddedToProjectAsDependentFile()
		{
			CreateProjectItems();
			msbuildProject.FileName = @"d:\projects\myproject\myproject.csproj";
			msbuildProject.AddFile(@"UI\MainForm.cs");
			string fileToAdd = @"d:\projects\myproject\UI\MainForm.Designer.cs";
			fakeFileService.ExistingFileNames.Add(fileToAdd);
			
			projectItems.AddFromFileCopy(fileToAdd);
			
			FileProjectItem fileItem = msbuildProject.FindFile(fileToAdd);
			Assert.AreEqual(@"UI\MainForm.Designer.cs", fileItem.Include);
			Assert.AreEqual("MainForm.cs", fileItem.DependentUpon);
		}
	}
}
