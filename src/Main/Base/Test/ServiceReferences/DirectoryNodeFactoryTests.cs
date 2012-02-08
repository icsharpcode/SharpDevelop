// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Gui.Pads.ProjectBrowser.TreeNodes;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.WebReferences;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class DirectoryNodeFactoryTests
	{
		MSBuildBasedProject project;
		
		void CreateProject()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");
		}
		
		ServiceReferencesProjectItem AddWCFMetadataProjectItemToProject()
		{
			var projectItem = new ServiceReferencesProjectItem(project);
			projectItem.Include = @"Service References\";
			ProjectService.AddProjectItem(project, projectItem);
			return projectItem;
		}
		
		FileProjectItem CreateFileProjectItem(string include)
		{
			return new FileProjectItem(project, ItemType.None, include);
		}
		
		[Test]
		public void CreateDirectoryNode_WCFMetadataProjectItem_CreatesServiceReferencesFolderNode()
		{
			CreateProject();
			ServiceReferencesProjectItem projectItem = AddWCFMetadataProjectItemToProject();
			
			DirectoryNode node = DirectoryNodeFactory.CreateDirectoryNode(projectItem, FileNodeStatus.None);
			
			Assert.IsTrue(node is ServiceReferencesFolderNode);
		}
		
		[Test]
		public void CreateDirectoryNode_WCFMetadataProjectItem_FileNodeStatusPassedToDirectoryNodeCreated()
		{
			CreateProject();
			ServiceReferencesProjectItem projectItem = AddWCFMetadataProjectItemToProject();
			
			DirectoryNode node = DirectoryNodeFactory.CreateDirectoryNode(projectItem, FileNodeStatus.Missing);
			FileNodeStatus nodeStatus = node.FileNodeStatus;
			
			Assert.AreEqual(FileNodeStatus.Missing, nodeStatus);
		}
		
		[Test]
		public void CreateDirectoryNode_WCFMetadataProjectItem_ProjectItemPassedToDirectoryNodeCreated()
		{
			CreateProject();
			ServiceReferencesProjectItem expectedProjectItem = AddWCFMetadataProjectItemToProject();
			
			DirectoryNode node = DirectoryNodeFactory.CreateDirectoryNode(expectedProjectItem, FileNodeStatus.None);
			ProjectItem projectItem = node.ProjectItem;
			
			Assert.AreEqual(expectedProjectItem, projectItem);
		}
		
		[Test]
		public void CreateDirectoryNode_ParentIsServiceReferencesFolderNode_CreatesServiceReferenceNode()
		{
			CreateProject();
			ServiceReferencesProjectItem expectedProjectItem = AddWCFMetadataProjectItemToProject();
			DirectoryNode parentServiceReferencesFolderNode = 
				DirectoryNodeFactory.CreateDirectoryNode(expectedProjectItem, FileNodeStatus.None);
			string directory = "ServiceReference1";
			
			ServiceReferenceNode node =
				DirectoryNodeFactory.CreateDirectoryNode(parentServiceReferencesFolderNode, project, directory) as ServiceReferenceNode;
			
			Assert.AreEqual("ServiceReference1", node.Directory);
		}
		
		[Test]
		public void CreateDirectoryNode_ProjectHasServiceReferencesItemAndDirectoryMatchesServiceReferencesPath_CreatesServiceReferencesFolderNode()
		{
			CreateProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			ServiceReferencesProjectItem projectItem = AddWCFMetadataProjectItemToProject();
			projectItem.Include = @"Service References\";
			
			string directory = @"d:\projects\MyProject\Service References";
			ServiceReferencesFolderNode referencesNode = 
				DirectoryNodeFactory.CreateDirectoryNode(null, project, directory) as ServiceReferencesFolderNode;
			
			Assert.AreEqual(directory, referencesNode.Directory);
		}
		
		[Test]
		public void CreateDirectoryNode_FileProjectItemThatEndsWithForwardSlash_DirectoryNodeCreatedWithForwardSlashRemoved()
		{
			CreateProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			FileProjectItem projectItem = CreateFileProjectItem(@"MyFolder/");
			
			DirectoryNode node = DirectoryNodeFactory.CreateDirectoryNode(projectItem, FileNodeStatus.None);
			
			Assert.AreEqual(@"d:\projects\MyProject\MyFolder", node.Directory);
		}
		
		[Test]
		public void CreateDirectoryNode_FileProjectItemThatEndsWithBackSlash_DirectoryNodeCreatedWithBackSlashRemoved()
		{
			CreateProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			FileProjectItem projectItem = CreateFileProjectItem(@"MyFolder\");
			
			DirectoryNode node = DirectoryNodeFactory.CreateDirectoryNode(projectItem, FileNodeStatus.None);
			
			Assert.AreEqual(@"d:\projects\MyProject\MyFolder", node.Directory);
		}
	}
}
