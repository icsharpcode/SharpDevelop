// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// Tests the WebReference.GetProjectItems method returns the 
	/// correct project items from a project.
	/// </summary>
	[TestFixture]
	public class ProjectWebReferenceItemsTests
	{
		List<ProjectItem> projectItems;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = "c:\\projects\\test\\foo.csproj";
			
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
			
			projectItems = WebReference.GetFileItems(project, "localhost");
		}
		
		[Test]
		public void ReferenceMapFileItemFound()
		{
			Assert.IsNotNull(WebReferenceTestHelper.GetProjectItem(projectItems, "Web References\\localhost\\Reference.map", ItemType.None));
		}
		
		[Test]
		public void ProxyFileItemFound()
		{
			Assert.IsNotNull(WebReferenceTestHelper.GetProjectItem(projectItems, "Web References\\localhost\\Reference.cs", ItemType.Compile));
		}

		[Test]
		public void WsdlFileItemFound()
		{
			Assert.IsNotNull(WebReferenceTestHelper.GetProjectItem(projectItems, "Web References\\localhost\\test.wsdl", ItemType.None));
		}
		
		[Test]
		public void DiscoFileItemFound()
		{
			Assert.IsNotNull(WebReferenceTestHelper.GetProjectItem(projectItems, "Web References\\localhost\\test.disco", ItemType.None));
		}
	}
}
