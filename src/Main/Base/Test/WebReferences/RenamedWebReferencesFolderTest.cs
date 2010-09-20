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
	/// Non-standard web references folder name.
	/// </summary>
	[TestFixture]
	public class RenamedWebReferencesFolderTest
	{
		SD.WebReference webReference;
		DiscoveryClientProtocol protocol;
		MSBuildBasedProject project;
		WebReferenceUrl webReferenceUrl;
		
		string name = "localhost";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
		
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = "C:\\Projects\\Web.csproj";
			WebReferencesProjectItem item = new WebReferencesProjectItem(project);
			item.Include = "Foo\\";
			ProjectService.AddProjectItem(project, item);
			
			protocol = new DiscoveryClientProtocol();
			
			WebReferenceTestHelper.InitializeProjectBindings();
			
			webReference = new SD.WebReference(project, updateFromUrl, name, proxyNamespace, protocol);
			webReferenceUrl = (WebReferenceUrl)WebReferenceTestHelper.GetProjectItem(webReference.Items, ItemType.WebReferenceUrl);
		}
		
		[Test]
		public void WebReferenceRelativePath()
		{
			Assert.AreEqual("Foo\\localhost", webReferenceUrl.RelPath);
		}
		
		[Test]
		public void WebReferencesFolder()
		{
			Assert.AreEqual("C:\\Projects\\Foo", webReference.WebReferencesDirectory);
		}
	}
}
