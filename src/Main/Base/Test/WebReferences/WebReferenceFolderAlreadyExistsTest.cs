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
using ICSharpCode.SharpDevelop;
using SD = ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Services.Description;
using System.Web.Services.Discovery;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// Checks that the web reference folder name is changed if
	/// one exists with the same name.
	/// </summary>
	[TestFixture]
	public class WebReferenceFolderAlreadyExistsTest : SDTestFixtureBase
	{
		Gui.WebReference webReference;
		DiscoveryClientProtocol protocol;
		MSBuildBasedProject project;
		WebReferenceUrl webReferenceUrl;
		FileProjectItem discoFileProjectItem;
		FileProjectItem referenceMapFileProjectItem;
		FileProjectItem wsdlFileProjectItem;
		FileProjectItem proxyFileProjectItem;
		
		string oldName = "localhost";
		string name = "localhost1";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("C:\\Projects\\Web.csproj");
			WebReferencesProjectItem item = new WebReferencesProjectItem(project);
			item.Include = "Web References\\";
			ProjectService.AddProjectItem(project, item);
			
			protocol = new DiscoveryClientProtocol();
			DiscoveryDocumentReference discoveryRef = new DiscoveryDocumentReference();
			discoveryRef.Url = updateFromUrl;
			protocol.References.Add(discoveryRef);
			
			ContractReference contractRef = new ContractReference();
			contractRef.Url = "http://localhost/test.asmx?wsdl";
			contractRef.ClientProtocol = new DiscoveryClientProtocol();
			ServiceDescription desc = new ServiceDescription();
			contractRef.ClientProtocol.Documents.Add(contractRef.Url, desc);
			protocol.References.Add(contractRef);
			
			WebReferenceTestHelper.InitializeProjectBindings();
			
			webReference = new Gui.WebReference(project, updateFromUrl, oldName, proxyNamespace, protocol);
			
			// Force generation of items.
			List<ProjectItem> items = webReference.Items;
			
			// Change the web reference name.
			webReference.Name = name;
			webReferenceUrl = (WebReferenceUrl)WebReferenceTestHelper.GetProjectItem(webReference.Items, ItemType.WebReferenceUrl);
			
			discoFileProjectItem = WebReferenceTestHelper.GetFileProjectItem(webReference.Items, "Web References\\localhost1\\test.disco", ItemType.None);
			referenceMapFileProjectItem = WebReferenceTestHelper.GetFileProjectItem(webReference.Items, "Web References\\localhost1\\Reference.map", ItemType.None);
			wsdlFileProjectItem = WebReferenceTestHelper.GetFileProjectItem(webReference.Items, "Web References\\localhost1\\test.wsdl", ItemType.None);
			proxyFileProjectItem = WebReferenceTestHelper.GetFileProjectItem(webReference.Items, "Web References\\localhost1\\Reference.cs", ItemType.Compile);
		}
		
		[Test]
		public void WebReferenceRelativePath()
		{
			Assert.AreEqual("Web References\\localhost1", webReferenceUrl.RelPath);
		}
		
		[Test]
		public void WebReferenceName()
		{
			Assert.AreEqual(name, webReference.Name);
		}
		
		[Test]
		public void WebReferenceDirectory()
		{
			Assert.AreEqual(Path.Combine(project.Directory, "Web References\\localhost1"), webReference.Directory);
		}
		
		[Test]
		public void ProxyFileName()
		{
			Assert.AreEqual("C:\\Projects\\Web References\\localhost1\\Reference.cs", webReference.WebProxyFileName);
		}
		
		[Test]
		public void DiscoFileItemExists()
		{
			Assert.IsNotNull(discoFileProjectItem);
		}
		
		[Test]
		public void WsdlFileItemExists()
		{
			Assert.IsNotNull(wsdlFileProjectItem);
		}
		
		[Test]
		public void ReferenceMapFileItemExists()
		{
			Assert.IsNotNull(referenceMapFileProjectItem);
		}
		
		[Test]
		public void ProxyFileItemExists()
		{
			Assert.IsNotNull(proxyFileProjectItem);
		}
	}
}
