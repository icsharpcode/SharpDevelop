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
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Services.Description;
using System.Web.Services.Discovery;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	using SD = ICSharpCode.SharpDevelop.Gui;
	
	/// <summary>
	/// Tests the generated project items for a web reference.
	/// </summary>
	[TestFixture]
	public class WebReferenceTests : SDTestFixtureBase
	{
		SD.WebReference webReference;
		DiscoveryClientProtocol protocol;
		WebReferenceUrl webReferenceUrl;
		FileProjectItem discoFileProjectItem;
		FileProjectItem referenceMapFileProjectItem;
		FileProjectItem wsdlFileProjectItem;
		FileProjectItem proxyFileProjectItem;
		WebReferencesProjectItem webReferencesProjectItem;
		ReferenceProjectItem webServicesReferenceProjectItem;
		MSBuildBasedProject project;
		
		string name = "localhost";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("C:\\projects\\test\\foo.csproj");

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
			
			webReference = new SD.WebReference(project, updateFromUrl, name, proxyNamespace, protocol);
			
			webReferenceUrl = webReference.WebReferenceUrl;
			discoFileProjectItem = WebReferenceTestHelper.GetFileProjectItem(webReference.Items, "Web References\\localhost\\test.disco", ItemType.None);
			referenceMapFileProjectItem = WebReferenceTestHelper.GetFileProjectItem(webReference.Items, "Web References\\localhost\\Reference.map", ItemType.None);
			wsdlFileProjectItem = WebReferenceTestHelper.GetFileProjectItem(webReference.Items, "Web References\\localhost\\test.wsdl", ItemType.None); 
			proxyFileProjectItem = WebReferenceTestHelper.GetFileProjectItem(webReference.Items, "Web References\\localhost\\Reference.cs", ItemType.Compile);
			webReferencesProjectItem = (WebReferencesProjectItem)WebReferenceTestHelper.GetProjectItem(webReference.Items, "Web References\\", ItemType.WebReferences);
			webServicesReferenceProjectItem = (ReferenceProjectItem)WebReferenceTestHelper.GetProjectItem(webReference.Items, ItemType.Reference);
		}
		
		[Test]
		public void WebReferenceInProjectItems()
		{
			Assert.IsNotNull((WebReferenceUrl)WebReferenceTestHelper.GetProjectItem(webReference.Items, ItemType.WebReferenceUrl));
		}
		
		[Test]
		public void WebReferenceInclude()
		{
			Assert.AreEqual(updateFromUrl, webReferenceUrl.Include);
		}
		
		[Test]
		public void WebReferenceHasProject()
		{
			Assert.IsNotNull(webReferenceUrl.Project);
		}
		
		[Test]
		public void WebReferenceHasWebReferencesProjectItem()
		{
			Assert.IsNotNull(webReference.WebReferencesProjectItem);
		}
		
		[Test]
		public void WebReferencesDirectory()
		{
			Assert.AreEqual("C:\\projects\\test\\Web References", webReferencesProjectItem.Directory);
		}
		
		[Test]
		public void WebReferenceUpdateFromUrl()
		{
			Assert.AreEqual(updateFromUrl, webReferenceUrl.UpdateFromURL);
		}
		
		[Test]
		public void WebReferenceUrlBehaviour()
		{
			Assert.AreEqual("Static", webReferenceUrl.UrlBehavior);
		}
		
		[Test]
		public void WebReferenceUrlNamespace()
		{
			Assert.AreEqual(proxyNamespace, webReferenceUrl.Namespace);
		}
		
		[Test]
		public void WebReferenceRelPath()
		{
			Assert.AreEqual("Web References\\localhost", webReferenceUrl.RelPath);
		}
		
		[Test]
		public void WebReferenceDirectory()
		{
			Assert.AreEqual("C:\\projects\\test\\Web References\\localhost", webReference.Directory);
		}
		
		[Test]
		public void DiscoFileItemExists()
		{
			Assert.IsNotNull(discoFileProjectItem);
		}
		
		[Test]
		public void WsdlFileItemHasProject()
		{
			Assert.IsNotNull(wsdlFileProjectItem.Project);
		}
		
		[Test]
		public void ReferenceMapFileItemGeneratorProperty()
		{
			Assert.AreEqual("MSDiscoCodeGenerator", referenceMapFileProjectItem.GetEvaluatedMetadata("Generator"));
		}
		
		[Test]
		public void ReferenceMapFileItemLastGenOutputProperty()
		{
			Assert.AreEqual("Reference.cs", referenceMapFileProjectItem.GetEvaluatedMetadata("LastGenOutput"));
		}
		
		[Test]
		public void ReferenceMapFileItemHasProject()
		{
			Assert.IsNotNull(referenceMapFileProjectItem.Project);
		}
		
		[Test]
		public void ProxyFileItemAutoGenProperty()
		{
			Assert.AreEqual("True", proxyFileProjectItem.GetEvaluatedMetadata("AutoGen"));
		}
		
		[Test]
		public void ProxyFileItemDesignTimeProperty()
		{
			Assert.AreEqual("True", proxyFileProjectItem.GetEvaluatedMetadata("DesignTime"));
		}
		
		[Test]
		public void ProxyFileName()
		{
			Assert.AreEqual("C:\\projects\\test\\Web References\\localhost\\Reference.cs", webReference.WebProxyFileName);
		}
		
		[Test]
		public void ProxyFileItemDependentUpon()
		{
			Assert.AreEqual("Reference.map", proxyFileProjectItem.DependentUpon);
		}
		
		[Test]
		public void ProxyFileItemHasProject()
		{
			Assert.IsNotNull(proxyFileProjectItem.Project);
		}
				
		[Test]
		public void WebReferencesItemHasProject()
		{
			Assert.IsNotNull(webReferencesProjectItem.Project);
		}
		
		[Test]
		public void WebServicesReferenceItemHasProject()
		{
			Assert.IsNotNull(webServicesReferenceProjectItem.Project);
		}
		
		[Test]
		public void WebServicesReferenceItemInclude()
		{
			Assert.AreEqual("System.Web.Services", webServicesReferenceProjectItem.Include);
		}
		
		[Test]
		public void WebReferencesFolder()
		{
			Assert.AreEqual("C:\\projects\\test\\Web References", webReference.WebReferencesDirectory);
		}
	}
}
