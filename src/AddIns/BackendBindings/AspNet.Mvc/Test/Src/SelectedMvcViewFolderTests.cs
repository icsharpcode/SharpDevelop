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
using System.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class SelectedMvcViewFolderTests : MvcTestsBase
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
