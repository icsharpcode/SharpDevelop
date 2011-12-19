// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		public void Create_ItemTypeIsWCFMetadata_ReturnsServiceReferenceProjectItem()
		{
			CreateProject();
			CreateProjectItemBackendStore();
			backendStore.ItemType = new ItemType("WCFMetadata");
			
			var projectItem = project.CreateProjectItem(backendStore) as ServiceReferencesProjectItem;
			
			Assert.IsNotNull(projectItem);
		}
	}
}
