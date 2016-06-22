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
