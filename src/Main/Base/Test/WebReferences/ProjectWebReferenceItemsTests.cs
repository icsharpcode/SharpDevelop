// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			MSBuildProject project = new MSBuildProject();
			project.Language = "C#";
			project.FileName = "c:\\projects\\test\\foo.csproj";
			
			// Web references item.
			WebReferencesProjectItem webReferencesItem = new WebReferencesProjectItem(project);
			webReferencesItem.Include = "Web References\\";
			project.Items.Add(webReferencesItem);
			
			// Web reference url.
			WebReferenceUrl webReferenceUrl = new WebReferenceUrl(project);
			webReferenceUrl.Include = "http://localhost/test.asmx";
			webReferenceUrl.UpdateFromURL = "http://localhost/test.asmx";
			webReferenceUrl.RelPath = "Web References\\localhost";
			project.Items.Add(webReferenceUrl);
			
			FileProjectItem discoFileItem = new FileProjectItem(project, ItemType.None);
			discoFileItem.Include = "Web References\\localhost\\test.disco";
			project.Items.Add(discoFileItem);

			FileProjectItem wsdlFileItem = new FileProjectItem(project, ItemType.None);
			wsdlFileItem.Include = "Web References\\localhost\\test.wsdl";
			project.Items.Add(wsdlFileItem);
			
			// Proxy
			FileProjectItem proxyItem = new FileProjectItem(project, ItemType.Compile);
			proxyItem.Include = "Web References\\localhost\\Reference.cs";
			proxyItem.DependentUpon = "Reference.map";
			project.Items.Add(proxyItem);
			
			// Reference map.
			FileProjectItem mapItem = new FileProjectItem(project, ItemType.None);
			mapItem.Include = "Web References\\localhost\\Reference.map";
			project.Items.Add(mapItem);
			
			// System.Web.Services reference.
			ReferenceProjectItem webServicesReferenceItem = new ReferenceProjectItem(project, "System.Web.Services");
			project.Items.Add(webServicesReferenceItem);
			
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
