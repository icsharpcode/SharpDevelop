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
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class SelectedFolderNodeInProjectsViewTests : MvcTestsBase
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
