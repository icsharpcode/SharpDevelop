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

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferencesFolderNodeTests : SDTestFixtureBase
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
			project.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
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
