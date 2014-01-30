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
using Gui.Pads.ProjectBrowser.TreeNodes;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.WebReferences;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class DirectoryNodeFactoryTests : SDTestFixtureBase
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
			
			Assert.AreEqual("ServiceReference1", node.Directory.ToString());
		}
		
		[Test]
		public void CreateDirectoryNode_ProjectHasServiceReferencesItemAndDirectoryMatchesServiceReferencesPath_CreatesServiceReferencesFolderNode()
		{
			CreateProject();
			project.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			ServiceReferencesProjectItem projectItem = AddWCFMetadataProjectItemToProject();
			projectItem.Include = @"Service References\";
			
			string directory = @"d:\projects\MyProject\Service References";
			ServiceReferencesFolderNode referencesNode = 
				DirectoryNodeFactory.CreateDirectoryNode(null, project, directory) as ServiceReferencesFolderNode;
			
			Assert.AreEqual(directory, referencesNode.Directory.ToString());
		}
		
		[Test]
		public void CreateDirectoryNode_FileProjectItemThatEndsWithForwardSlash_DirectoryNodeCreatedWithForwardSlashRemoved()
		{
			CreateProject();
			project.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			FileProjectItem projectItem = CreateFileProjectItem(@"MyFolder/");
			
			DirectoryNode node = DirectoryNodeFactory.CreateDirectoryNode(projectItem, FileNodeStatus.None);
			
			Assert.AreEqual(@"d:\projects\MyProject\MyFolder", node.Directory.ToString());
		}
		
		[Test]
		public void CreateDirectoryNode_FileProjectItemThatEndsWithBackSlash_DirectoryNodeCreatedWithBackSlashRemoved()
		{
			CreateProject();
			project.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			FileProjectItem projectItem = CreateFileProjectItem(@"MyFolder\");
			
			DirectoryNode node = DirectoryNodeFactory.CreateDirectoryNode(projectItem, FileNodeStatus.None);
			
			Assert.AreEqual(@"d:\projects\MyProject\MyFolder", node.Directory.ToString());
		}
	}
}
