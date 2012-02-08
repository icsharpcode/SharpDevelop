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
	public class SelectedFolderNodeInProjectsViewTests
	{
		SelectedFolderNodeInProjectsView selectedFolderNode;
		TestableProject projectForSelectedFolder;
		DirectoryNode directoryNode;
		ProjectNode projectNode;
		
		void CreateSelectedFolderNode(string folder)
		{
			projectForSelectedFolder = TestableProject.CreateProject();
			projectNode = new ProjectNode(projectForSelectedFolder);
			
			directoryNode = new DirectoryNode(folder);
			directoryNode.AddTo(projectNode);
			
			selectedFolderNode = new SelectedFolderNodeInProjectsView(directoryNode);
		}
		
		[Test]
		public void Folder_DirectorySpecifiedForNode_ReturnsDirectoryTakenFromDirectoryNode()
		{
			string expectedFolder = @"d:\projects\MyAspMvcApp\Views\Home";
			CreateSelectedFolderNode(expectedFolder);
			
			string folder = selectedFolderNode.Folder;
			
			Assert.AreEqual(expectedFolder, folder);
		}
		
		[Test]
		public void MvcProject_DirectoryNodeHasProjectNodeAsParent_ReturnsProjectFromDirectoryNode()
		{
			CreateSelectedFolderNode(@"d:\projects\MyAspMvcApp\Views\Home");
			
			IProject project = selectedFolderNode.Project.Project;
			
			Assert.AreEqual(projectForSelectedFolder, project);
		}
	}
}
