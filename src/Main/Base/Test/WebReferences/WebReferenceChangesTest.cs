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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.Web.Services.Description;
using System.Web.Services.Discovery;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// Tests the WebReferenceChanges class.
	/// </summary>
	[TestFixture]
	public class WebReferenceChangesTest : SDTestFixtureBase
	{
		WebReferenceChanges changes;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			// Set up the project.
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("c:\\projects\\test\\foo.csproj");
			
			// Web references item.
			WebReferencesProjectItem webReferencesItem = new WebReferencesProjectItem(project);
			webReferencesItem.Include = "Web References\\";
			ProjectService.AddProjectItem(project, webReferencesItem);
			
			// Web reference url.
			WebReferenceUrl webReferenceUrl = new WebReferenceUrl(project);
			webReferenceUrl.Include = "http://localhost/test.asmx";
			webReferenceUrl.UpdateFromURL = "http://localhost/test.asmx";
			webReferenceUrl.RelPath = "Web References\\localhost";
			ProjectService.AddProjectItem(project, webReferenceUrl);
			
			FileProjectItem discoFileItem = new FileProjectItem(project, ItemType.None);
			discoFileItem.Include = "Web References\\localhost\\test.disco";
			ProjectService.AddProjectItem(project, discoFileItem);

			FileProjectItem wsdlFileItem = new FileProjectItem(project, ItemType.None);
			wsdlFileItem.Include = "Web References\\localhost\\test.wsdl";
			ProjectService.AddProjectItem(project, wsdlFileItem);
			
			// Proxy
			FileProjectItem proxyItem = new FileProjectItem(project, ItemType.Compile);
			proxyItem.Include = "Web References\\localhost\\Reference.cs";
			proxyItem.DependentUpon = "Reference.map";
			ProjectService.AddProjectItem(project, proxyItem);
			
			// Reference map.
			FileProjectItem mapItem = new FileProjectItem(project, ItemType.None);
			mapItem.Include = "Web References\\localhost\\Reference.map";
			ProjectService.AddProjectItem(project, mapItem);
			
			// System.Web.Services reference.
			ReferenceProjectItem webServicesReferenceItem = new ReferenceProjectItem(project, "System.Web.Services");
			ProjectService.AddProjectItem(project, webServicesReferenceItem);
			
			// Set up the web reference.
			DiscoveryClientProtocol	protocol = new DiscoveryClientProtocol();
			DiscoveryDocumentReference discoveryRef = new DiscoveryDocumentReference();
			discoveryRef.Url = "http://localhost/new.asmx";
			protocol.References.Add(discoveryRef);
			
			ContractReference contractRef = new ContractReference();
			contractRef.Url = "http://localhost/new.asmx?wsdl";
			contractRef.ClientProtocol = new DiscoveryClientProtocol();
			ServiceDescription desc = new ServiceDescription();
			contractRef.ClientProtocol.Documents.Add(contractRef.Url, desc);
			protocol.References.Add(contractRef);
			
			WebReferenceTestHelper.InitializeProjectBindings();
			
			var webReference = new Gui.WebReference(project, "http://localhost/new.asmx", "localhost", "ProxyNamespace", protocol);
			changes = webReference.GetChanges(project);
		}
				
		[Test]
		public void HasChanged()
		{
			Assert.IsTrue(changes.Changed);
		}
		
		[Test]
		public void HasNewWsdlFile()
		{
			Assert.IsNotNull(WebReferenceTestHelper.GetFileProjectItem(changes.NewItems, "Web References\\localhost\\new.wsdl", ItemType.None));
		}
		
		[Test]
		public void HasNewDiscoFile()
		{
			Assert.IsNotNull(WebReferenceTestHelper.GetFileProjectItem(changes.NewItems, "Web References\\localhost\\new.disco", ItemType.None));
		}
		
		[Test]
		public void OldWsdlFileRemoved()
		{
			Assert.IsNotNull(WebReferenceTestHelper.GetFileProjectItem(changes.ItemsRemoved, "Web References\\localhost\\test.wsdl", ItemType.None));
		}
		
		[Test]
		public void OldDiscoFileRemoved()
		{
			Assert.IsNotNull(WebReferenceTestHelper.GetFileProjectItem(changes.ItemsRemoved, "Web References\\localhost\\test.disco", ItemType.None));
		}
		
		[Test]
		public void WebReferenceUrlNotConsideredNewItem()
		{
			Assert.IsNull(WebReferenceTestHelper.GetProjectItem(changes.NewItems, ItemType.WebReferenceUrl));
		}
		
		[Test]
		public void WebReferenceUrlNotConsideredRemoved()
		{
			Assert.IsNull(WebReferenceTestHelper.GetProjectItem(changes.ItemsRemoved, ItemType.WebReferenceUrl));
		}
	}
}
