// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class WebReferenceFolderAlreadyExistsTest
	{
		SD.WebReference webReference;
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
		
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = "C:\\Projects\\Web.csproj";
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
			
			webReference = new SD.WebReference(project, updateFromUrl, oldName, proxyNamespace, protocol);
			
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
