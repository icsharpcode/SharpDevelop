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
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// Tests the WebReference.GetProjectItems method returns the 
	/// correct project items from a project.
	/// </summary>
	[TestFixture]
	public class ProjectWebReferenceItemsTests : SDTestFixtureBase
	{
		List<ProjectItem> projectItems;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
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
