// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		DirectoryNode directoryNode;
		TestableProject projectForSelectedFolder;
		ProjectNode projectNode;
		
		void CreateSelectedFolder()
		{
			CreateSelectedFolder(@"d:\projects\MyAspMvcApp\Views\Home");
		}
		
		void CreateSelectedFolder(string folder)
		{
			projectForSelectedFolder = TestableProject.CreateProject();
			projectNode = new ProjectNode(projectForSelectedFolder);
			
			directoryNode = new DirectoryNode(folder);
			directoryNode.AddTo(projectNode);
			
			selectedFolder = new TestableSelectedMvcViewFolder(directoryNode);
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
			
			string fileNameAddedToProject = selectedFolder.PathPassedToAddNewFileToDirectoryNode;
			string expectedFileNameAddedToProject =
				@"d:\projects\MyAspMvcApp\Views\About\Index.aspx";
			
			Assert.AreEqual(expectedFileNameAddedToProject, fileNameAddedToProject);
		}
		
		[Test]
		public void AddFileToProject_FilePassed_IsProjectSaved()
		{
			CreateSelectedFolder(@"d:\projects\MyAspMvcApp\Views\About");
			selectedFolder.AddFileToProject("Index.aspx");
			
			bool saved = projectForSelectedFolder.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void Project_DirectoryNodeHasParentProject_ReturnsProject()
		{
			CreateSelectedFolder();
			
			IProject project = selectedFolder.Project;
			
			Assert.AreEqual(projectForSelectedFolder, project);
		}
	}
}
