// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using NUnit.Framework;
using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	[TestFixture]
	public class DirectoryNodeFactoryTests
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
			project.FileName = Path.Combine(projectDirectory, "foo.csproj");
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
			Assert.AreEqual(Path.Combine(projectDirectory, "Properties"), appDesignerFolderNode.Directory);
		}
		
		[Test]
		public void OrdinaryFolderNodeIsNotSpecialFolder()
		{
			Assert.AreEqual(SpecialFolder.None, ordinaryFolderNode.SpecialFolder);
		}
		
		[Test]
		public void OrdinaryFolderNodeDirectory()
		{
			Assert.AreEqual(Path.Combine(projectDirectory, "Test"), ordinaryFolderNode.Directory);
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
			Assert.AreEqual(Path.Combine(projectDirectory, "Web References"), webReferencesFolderNode.Directory);
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
			Assert.AreEqual(Path.Combine(projectDirectory, "Web References\\localhost"), webReferenceNode.Directory);
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
			Assert.AreEqual("c:\\projects\\test\\MissingFolder", missingOrdinaryFolderNode.Directory);
		}
		
		[Test]
		public void MissingOrdinaryFolderProjectItemExists()
		{
			Assert.IsNotNull(missingOrdinaryFolderNode.ProjectItem);
		}
	}
}
