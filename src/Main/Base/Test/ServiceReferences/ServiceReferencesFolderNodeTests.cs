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
	public class ServiceReferencesFolderNodeTests
	{
		MSBuildBasedProject project;
		ServiceReferencesProjectItem projectItem;
		ServiceReferencesFolderNode folderNode;
		
		void CreateProject()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");
		}
		
		void CreateServiceReferenceProjectItem()
		{
			CreateProject();
			projectItem = new ServiceReferencesProjectItem(project);
		}
		
		void CreateFolderNode()
		{
			CreateFolderNode(FileNodeStatus.None);
		}
		
		void CreateFolderNode(FileNodeStatus status)
		{
			folderNode = new ServiceReferencesFolderNode(projectItem, status);
		}
		
		[Test]
		public void Directory_ProjectItemHasServiceReferenceDirectory_ReturnsFullPathToServiceReferencesDirectory()
		{
			CreateServiceReferenceProjectItem();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			projectItem.Include = @"Service References\";
			CreateFolderNode();
			
			string directory = folderNode.Directory;
			
			Assert.AreEqual(@"d:\projects\MyProject\Service References", directory);
		}
		
		[Test]
		public void SortOrder_ServiceReferencesFolderNodeShouldAppearAtTopOfProjectsWindow_ReturnsZero()
		{
			CreateServiceReferenceProjectItem();
			CreateFolderNode();
			
			Assert.AreEqual(0, folderNode.SortOrder);
		}
		
		[Test]
		public void SpecialFolder_ServiceReferencesFolderNodeIdentifiedAsSpecialFolder_ReturnsSpecialFolderServicesReferencesFolder()
		{
			CreateServiceReferenceProjectItem();
			CreateFolderNode();
			
			Assert.AreEqual(SpecialFolder.ServiceReferencesFolder, folderNode.SpecialFolder);
		}
		
		[Test]
		public void ContextMenuAddinTreePath_ServiceReferencesFolderNodeHasContextMenuAddinTreePath_ReturnsContextMenuAddinTreePathForServiceReferences()
		{
			CreateServiceReferenceProjectItem();
			CreateFolderNode();
			
			string expectedPath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ServiceReferencesFolderNode";
			
			Assert.AreEqual(expectedPath, folderNode.ContextmenuAddinTreePath);
		}
		
		[Test]
		public void OpenedImage_ServiceReferencesFolderNodeHasCustomIcon_ReturnsWebReferenceFolderIcon()
		{
			CreateServiceReferenceProjectItem();
			CreateFolderNode(FileNodeStatus.InProject);
			
			Assert.AreEqual("ProjectBrowser.WebReferenceFolder.Open", folderNode.OpenedImage);
		}
		
		[Test]
		public void ClosedImage_ServiceReferencesFolderNodeHasCustomIcon_ReturnsWebReferenceFolderIcon()
		{
			CreateServiceReferenceProjectItem();
			CreateFolderNode(FileNodeStatus.InProject);
			
			Assert.AreEqual("ProjectBrowser.WebReferenceFolder.Closed", folderNode.ClosedImage);
		}
	}
}
