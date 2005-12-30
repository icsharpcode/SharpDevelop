// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		MSBuildProject project;
		WebReferenceUrl webReferenceUrl;
		
		string name = "localhost";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
			
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			project = new MSBuildProject();
			project.FileName = "C:\\Projects\\Web.csproj";
			project.Language = "C#";
			WebReferencesProjectItem item = new WebReferencesProjectItem(project);
			item.Include = "Foo\\";
			project.Items.Add(item);
			
			protocol = new DiscoveryClientProtocol();
			
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
