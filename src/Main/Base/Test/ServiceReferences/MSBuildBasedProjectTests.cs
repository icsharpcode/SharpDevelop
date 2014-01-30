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

using System;
using System.Xml.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.WebReferences;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class MSBuildBasedProjectTests : SDTestFixtureBase
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
