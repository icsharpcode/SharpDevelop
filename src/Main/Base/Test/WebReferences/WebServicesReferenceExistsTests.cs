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
	/// Tests that the generated project items for a web reference do not include
	/// a reference to System.Web.Services if one already exists in the project.
	/// </summary>
	[TestFixture]
	public class WebServicesReferenceExistsTest : SDTestFixtureBase
	{
		Gui.WebReference webReference;
		DiscoveryClientProtocol protocol;
		ReferenceProjectItem webServicesReferenceProjectItem;
		MSBuildBasedProject project;
		
		string name = "localhost";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("C:\\projects\\test\\foo.csproj");
			
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(project, "System.Web.Services");
			ProjectService.AddProjectItem(project, referenceItem);
			
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
			
			webReference = new Gui.WebReference(project, updateFromUrl, name, proxyNamespace, protocol);
			webServicesReferenceProjectItem = (ReferenceProjectItem)WebReferenceTestHelper.GetProjectItem(webReference.Items, ItemType.Reference);
		}
		
		[Test]
		public void WebServicesReferenceItemDoesNotExist()
		{
			Assert.IsNull(webServicesReferenceProjectItem);
		}
		
		[Test]
		public void WebServicesReferenceDoesNotExist1()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(project, "System.Windows.Forms");
			ProjectService.AddProjectItem(project, referenceItem);
			
			Assert.IsFalse(Gui.WebReference.ProjectContainsWebServicesReference(project));
		}
		
		[Test]
		public void WebServicesReferenceExists1()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(project, "system.web.services");
			ProjectService.AddProjectItem(project, referenceItem);
			
			Assert.IsTrue(Gui.WebReference.ProjectContainsWebServicesReference(project));
		}
		
		[Test]
		public void WebServicesReferenceExists2()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(project, "System.Web.Services, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
			ProjectService.AddProjectItem(project, referenceItem);
			
			Assert.IsTrue(Gui.WebReference.ProjectContainsWebServicesReference(project));
		}
	}
}
