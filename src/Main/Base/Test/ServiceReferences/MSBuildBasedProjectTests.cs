// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.WebReferences;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class MSBuildBasedProjectTests
	{
		MSBuildBasedProject project;
		IProjectItemBackendStore backendStore;
		
		void CreateProject()
		{
			project = WebReferenceTestHelper.CreateTestProject("C#");
		}
		
		void CreateProjectItemBackendStore()
		{
			backendStore = MockRepository.GenerateStub<IProjectItemBackendStore>();
		}
		
		[Test]
		public void CreateProjectItem_ItemTypeIsWCFMetadata_ReturnsServiceReferencesProjectItem()
		{
			CreateProject();
			CreateProjectItemBackendStore();
			backendStore.ItemType = new ItemType("WCFMetadata");
			
			var projectItem = project.CreateProjectItem(backendStore) as ServiceReferencesProjectItem;
			
			Assert.IsNotNull(projectItem);
		}
		
		[Test]
		public void CreateProjectItem_ItemTypeIsWCFMetadataStorage_ReturnsServiceReferenceProjectItem()
		{
			CreateProject();
			CreateProjectItemBackendStore();
			backendStore.ItemType = new ItemType("WCFMetadataStorage");
			
			var projectItem = project.CreateProjectItem(backendStore) as ServiceReferenceProjectItem;
			
			Assert.IsNotNull(projectItem);
		}
		
		[Test]
		public void ContainsProjectExtension_ProjectDoesNotContainsExtension_ReturnsFalse()
		{
			CreateProject();
			
			bool contains = project.ContainsProjectExtension("UNKNOWN_EXTENSION");
			
			Assert.IsFalse(contains);
		}
		
		[Test]
		public void ContainsProjectExtension_ProjectContainsExtension_ReturnsTrue()
		{
			CreateProject();
			project.SaveProjectExtensions("Test", new XElement("MyProjectExtension"));
			
			bool contains = project.ContainsProjectExtension("Test");
			
			Assert.IsTrue(contains);
		}
	}
}
