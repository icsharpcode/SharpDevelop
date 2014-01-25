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
	/// Non-standard web references folder name.
	/// </summary>
	[TestFixture]
	public class RenamedWebReferencesFolderTest : SDTestFixtureBase
	{
		Gui.WebReference webReference;
		DiscoveryClientProtocol protocol;
		MSBuildBasedProject project;
		WebReferenceUrl webReferenceUrl;
		
		string name = "localhost";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("C:\\Projects\\Web.csproj");
			WebReferencesProjectItem item = new WebReferencesProjectItem(project);
			item.Include = "Foo\\";
			ProjectService.AddProjectItem(project, item);
			
			protocol = new DiscoveryClientProtocol();
			
			WebReferenceTestHelper.InitializeProjectBindings();
			
			webReference = new Gui.WebReference(project, updateFromUrl, name, proxyNamespace, protocol);
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
