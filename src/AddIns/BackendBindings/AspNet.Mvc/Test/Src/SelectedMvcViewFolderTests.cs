// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class SelectedMvcViewFolderTests
	{
		TestableSelectedMvcViewFolder selectedFolder;
		FakeMvcProject projectForSelectedFolder;
		FakeMvcFileService fakeFileService;
		FakeSelectedFolderNodeInProjectsView fakeSelectedFolderNode;
		
		void CreateSelectedFolder()
		{
			CreateSelectedFolder(@"d:\projects\MyAspMvcApp\Views\Home");
		}
		
		void CreateSelectedFolder(string folder)
		{
			selectedFolder = new TestableSelectedMvcViewFolder();
			fakeSelectedFolderNode = selectedFolder.FakeSelectedFolderNodeInProjectsView;
			fakeSelectedFolderNode.Folder = folder;
			projectForSelectedFolder = fakeSelectedFolderNode.FakeMvcProject;
			fakeFileService = selectedFolder.FakeFileService;
		}
		
		[Test]
		public void Path_DirectorySpecified_ReturnsDirectory()
		{
			string expectedPath = @"d:\projects\MyAspMvcApp\Views\Home";
			CreateSelectedFolder(expectedPath);
			
			string path = selectedFolder.Path;
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void AddFileToProject_FileRelativePathPassed_FullPathIncludingViewFolderIsUsedToAddFile()
		{
			CreateSelectedFolder(@"d:\projects\MyAspMvcApp\Views\About");
			selectedFolder.AddFileToProject("Index.aspx");
			
			string fileNameAddedToProject = fakeSelectedFolderNode.PathPassedToAddNewFile;
			string expectedFileNameAddedToProject =
				@"d:\projects\MyAspMvcApp\Views\About\Index.aspx";
			
			Assert.AreEqual(expectedFileNameAddedToProject, fileNameAddedToProject);
		}
		
		[Test]
		public void AddFileToProject_FilePassed_IsProjectSaved()
		{
			CreateSelectedFolder(@"d:\projects\MyAspMvcApp\Views\About");
			selectedFolder.AddFileToProject("Index.aspx");
			
			bool saved = fakeSelectedFolderNode.FakeMvcProject.SaveCalled;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void Project_FolderNodeHasProject_ReturnsProjectFromFolderNode()
		{
			CreateSelectedFolder();
			
			IMvcProject project = selectedFolder.Project;
			
			Assert.AreEqual(projectForSelectedFolder, project);
		}
		
		[Test]
		public void GetTemplateLanguage_ProjectIsVisualBasicProject_ReturnsVisualBasicTemplateLanguage()
		{
			CreateSelectedFolder();
			projectForSelectedFolder.SetVisualBasicAsTemplateLanguage();
			
			MvcTextTemplateLanguage language = selectedFolder.GetTemplateLanguage();
			
			Assert.AreEqual(MvcTextTemplateLanguage.VisualBasic, language);
		}
		
		[Test]
		public void GetTemplateLanguage_ProjectIsCSharpProject_ReturnsCSharpTemplateLanguage()
		{
			CreateSelectedFolder();
			projectForSelectedFolder.SetCSharpAsTemplateLanguage();
			
			MvcTextTemplateLanguage language = selectedFolder.GetTemplateLanguage();
			
			Assert.AreEqual(MvcTextTemplateLanguage.CSharp, language);
		}
		
		[Test]
		public void AddFileToProject_FileRelativePathPassed_FullPathIncludingViewFolderIsUsedToOpenFile()
		{
			CreateSelectedFolder(@"d:\projects\MyAspMvcApp\Views\About");
			selectedFolder.AddFileToProject("Index.aspx");
			
			string fileNameOpened = fakeFileService.FileNamePassedToOpenFile;
			string expectedFileNameOpened =
				@"d:\projects\MyAspMvcApp\Views\About\Index.aspx";
			
			Assert.AreEqual(expectedFileNameOpened, fileNameOpened);
		}
	}
}
