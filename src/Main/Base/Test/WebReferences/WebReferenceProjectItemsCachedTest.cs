// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// Tests the generated items are not changed if the items have been
	/// added to the project.  Previously a WebReferencesProjectItem would be 
	/// missing after the items have been added to the project.
	/// </summary>
	[TestFixture]
	public class WebReferenceProjectItemsCachedTest
	{
		SD.WebReference webReference;
		DiscoveryClientProtocol protocol;
		WebReferencesProjectItem webReferencesProjectItem;
		MSBuildBasedProject project;
		
		string name = "localhost";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = "C:\\projects\\test\\foo.csproj";

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
			
			foreach (ProjectItem item in webReference.Items) {
				ProjectService.AddProjectItem(project, item);
			}
			webReferencesProjectItem = webReference.WebReferencesProjectItem;
		}
		
		[Test]
		public void WebReferencesProjectItemExists()
		{
			Assert.IsNotNull(webReferencesProjectItem);
		}
	}
}
