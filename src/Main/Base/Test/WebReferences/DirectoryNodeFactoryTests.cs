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

using ICSharpCode.Core;
using NUnit.Framework;
using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	[TestFixture]
	public class DirectoryNodeFactoryTests : SDTestFixtureBase
	{		
		DirectoryNode appDesignerFolderNode;
		DirectoryNode ordinaryFolderNode;
		DirectoryNode webReferencesFolderNode;
		DirectoryNode missingWebReferencesFolderNode;
		DirectoryNode missingOrdinaryFolderNode;
		DirectoryNode webReferenceNode;

		string projectDirectory = "c:\\projects\\test";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create(Path.Combine(projectDirectory, "foo.csproj"));
			project.AppDesignerFolder = "Properties";
			
			WebReferencesProjectItem webReferencesItem = new WebReferencesProjectItem(project);
			webReferencesItem.Include = "Web References\\";
			ProjectService.AddProjectItem(project, webReferencesItem);
									
			FileProjectItem fileItem = new FileProjectItem(project, ItemType.Folder);
			fileItem.Include = "MissingFolder\\";
			ProjectService.AddProjectItem(project, fileItem);
			
			ProjectNode projectNode = new ProjectNode(project);

			appDesignerFolderNode = DirectoryNodeFactory.CreateDirectoryNode(projectNode, project, Path.Combine(projectDirectory, "Properties"));
			ordinaryFolderNode = DirectoryNodeFactory.CreateDirectoryNode(projectNode, project, Path.Combine(project.Directory, "Test"));
			webReferencesFolderNode = DirectoryNodeFactory.CreateDirectoryNode(projectNode, project, Path.Combine(project.Directory, webReferencesItem.Include));	
			webReferenceNode = DirectoryNodeFactory.CreateDirectoryNode(webReferencesFolderNode, project, Path.Combine(Path.Combine(project.Directory, webReferencesItem.Include), "localhost"));
			
			missingWebReferencesFolderNode = DirectoryNodeFactory.CreateDirectoryNode(webReferencesItem, FileNodeStatus.Missing);
			missingOrdinaryFolderNode = DirectoryNodeFactory.CreateDirectoryNode(fileItem, FileNodeStatus.Missing);
		}
		
		[Test]
		public void AppDesignerFolderNodeIsSpecialAppDesignerFolder()
		{
			Assert.AreEqual(SpecialFolder.AppDesigner, appDesignerFolderNode.SpecialFolder);
		}
		
		[Test]
		public void AppDesignerFolderNodeDirectory()
		{
			Assert.AreEqual(Path.Combine(projectDirectory, "Properties"), appDesignerFolderNode.Directory.ToString());
		}
		
		[Test]
		public void OrdinaryFolderNodeIsNotSpecialFolder()
		{
			Assert.AreEqual(SpecialFolder.None, ordinaryFolderNode.SpecialFolder);
		}
		
		[Test]
		public void OrdinaryFolderNodeDirectory()
		{
			Assert.AreEqual(Path.Combine(projectDirectory, "Test"), ordinaryFolderNode.Directory.ToString());
		}
		
		[Test]
		public void OrdinaryFolderNodeType()
		{
			Assert.IsTrue(ordinaryFolderNode is DirectoryNode);
		}
		
		[Test]
		public void WebReferencesFolderNodeIsSpecialWebReferencesFolder()
		{
			Assert.AreEqual(SpecialFolder.WebReferencesFolder, webReferencesFolderNode.SpecialFolder);
		}
		
		[Test]
		public void WebReferencesFolderNodeType()
		{
			Assert.IsTrue(webReferencesFolderNode is WebReferencesFolderNode);
		}
		
		[Test]
		public void WebReferencesFolderNodeDirectory()
		{
			Assert.AreEqual(Path.Combine(projectDirectory, "Web References"), webReferencesFolderNode.Directory.ToString());
		}

		[Test]
		public void WebReferenceNodeIsSpecialWebReferencesFolder()
		{
			Assert.AreEqual(SpecialFolder.WebReference, webReferenceNode.SpecialFolder);
		}
		
		[Test]
		public void WebReferenceNodeType()
		{
			Assert.IsTrue(webReferenceNode is WebReferenceNode);
		}
		
		[Test]
		public void WebReferenceNodeDirectory()
		{
			Assert.AreEqual(Path.Combine(projectDirectory, "Web References\\localhost"), webReferenceNode.Directory.ToString());
		}
		
		[Test]
		public void MissingWebReferencesFolderNodeIsMissing()
		{
			Assert.AreEqual(FileNodeStatus.Missing, missingWebReferencesFolderNode.FileNodeStatus);
		}
		
		[Test]
		public void MissingWebReferencesFolderNodeIsSpecialWebReferencesFolder()
		{
			Assert.AreEqual(SpecialFolder.WebReferencesFolder, missingWebReferencesFolderNode.SpecialFolder);
		}
		
		[Test]
		public void MissingWebReferencesFolderNodeType()
		{
			Assert.IsTrue(missingWebReferencesFolderNode is WebReferencesFolderNode);
		}
		
		[Test]
		public void MissingOrdinaryFolderNodeIsMissing()
		{
			Assert.AreEqual(FileNodeStatus.Missing, missingOrdinaryFolderNode.FileNodeStatus);
		}
		
		[Test]
		public void MissingOrdinaryFolderNodeIsNotSpecialFolder()
		{
			Assert.AreEqual(SpecialFolder.None, missingOrdinaryFolderNode.SpecialFolder);
		}
		
		[Test]
		public void MissingOrdinaryFolderNodeType()
		{
			Assert.IsTrue(missingOrdinaryFolderNode is DirectoryNode);
		}
		
		[Test]
		public void MissingOrdinaryFolderName()
		{
			Assert.AreEqual("c:\\projects\\test\\MissingFolder", missingOrdinaryFolderNode.Directory.ToString());
		}
		
		[Test]
		public void MissingOrdinaryFolderProjectItemExists()
		{
			Assert.IsNotNull(missingOrdinaryFolderNode.ProjectItem);
		}
	}
}
