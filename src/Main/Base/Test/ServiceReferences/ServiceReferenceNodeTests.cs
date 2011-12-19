// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Gui.Pads.ProjectBrowser.TreeNodes;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferenceNodeTests
	{
		ServiceReferenceNode referenceNode;
		
		void CreateNode()
		{
			referenceNode = new ServiceReferenceNode("ServiceReference1");
			referenceNode.FileNodeStatus = FileNodeStatus.InProject;
		}
		
		[Test]
		public void SpecialFolder_CustomSpecialFolderUsedByServiceReferenceNode_ReturnsServiceReferenceSpecialFolder()
		{
			CreateNode();
			
			Assert.AreEqual(SpecialFolder.ServiceReference, referenceNode.SpecialFolder);
		}
		
		[Test]
		public void ContextmenuAddinTreePath_CustomContextMenuPathUsed_ReturnsServiceReferenceContextMenuAddinTreePath()
		{
			CreateNode();
			
			string expectedPath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ServiceReferenceNode";
			
			Assert.AreEqual(expectedPath, referenceNode.ContextmenuAddinTreePath);
		}
		
		[Test]
		public void OpenedImage_CustomOpenedImageUsed_ReturnsWebReferenceImage()
		{
			CreateNode();
			
			Assert.AreEqual("ProjectBrowser.WebReference", referenceNode.OpenedImage);
		}
		
		[Test]
		public void ClosedImage_CustomOpenedImageUsed_ReturnsWebReferenceImage()
		{
			CreateNode();
			
			Assert.AreEqual("ProjectBrowser.WebReference", referenceNode.ClosedImage);
		}
	}
}
