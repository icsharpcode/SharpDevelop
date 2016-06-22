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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ReferencesTests
	{
		References references;
		FakePackageManagementProjectService fakeProjectService;
		TestableProject msbuildProject;
		TestableDTEProject project;
		
		void CreateReferences()
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			fakeProjectService = project.FakeProjectService;
			ProjectObject projectObject = (ProjectObject)project.Object;
			references = (References)projectObject.References;
		}
		
		void ReferenceCollectionAssertAreEqual(string[] expectedReferences, IEnumerable referenceList)
		{
			var actualReferences = new List<string>();
			foreach (global::EnvDTE.Reference reference in referenceList) {
				actualReferences.Add(reference.Name);
			}
			
			CollectionAssert.AreEqual(expectedReferences, actualReferences);
		}
		
		[Test]
		public void Add_AddGacAssemblyReference_ReferenceAddedToMSBuildProject()
		{
			CreateReferences();
			references.Add("System.Data");
			
			var reference = msbuildProject.Items.First() as ReferenceProjectItem;
			string referenceName = reference.Name;
			
			Assert.AreEqual("System.Data", referenceName);
		}
		
		[Test]
		public void Add_AddGacAssemblyReference_MSBuildProjectIsSaved()
		{
			CreateReferences();
			references.Add("System.Xml");
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void Add_AddGacAssemblyReferenceWhenReferenceAlreadyExistsInProject_ReferenceIsNotAddedToMSBuildProject()
		{
			CreateReferences();
			msbuildProject.AddReference("System.Data");
			
			references.Add("System.Data");
			
			int count = msbuildProject.Items.Count;
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void AddFromGAC_AddGacAssemblyReference_ReferenceAddedToMSBuildProject()
		{
			CreateReferences();
			references.AddFromGAC("System.Data");
			
			var reference = msbuildProject.Items.First() as ReferenceProjectItem;
			string referenceName = reference.Name;
			
			Assert.AreEqual("System.Data", referenceName);
		}
		
		[Test]
		public void Add_AddGacAssemblyReferenceWhenReferenceAlreadyExistsInProjectButWithDifferentCase_ReferenceIsNotAddedToMSBuildProject()
		{
			CreateReferences();
			msbuildProject.AddReference("System.Data");
			
			references.Add("SYSTEM.DATA");
			
			int count = msbuildProject.Items.Count;
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasTwoReferences_TwoReferencesReturned()
		{
			CreateReferences();
			msbuildProject.AddReference("System.Data");
			msbuildProject.AddReference("System.Xml");
			
			var enumerable = references as IEnumerable;
			
			var expectedReferences = new string[] {
				"System.Data",
				"System.Xml"
			};
			
			ReferenceCollectionAssertAreEqual(expectedReferences, enumerable);
		}
		
		[Test]
		public void GetEnumerator_ProjectHasFileItemBeforeReferenceItem_OneReferenceReturned()
		{
			CreateReferences();
			msbuildProject.AddFile(@"src\Test.cs");
			msbuildProject.AddReference("System.Xml");
			
			var enumerable = references as IEnumerable;
			
			var expectedReferences = new string[] {
				"System.Xml"
			};
			
			ReferenceCollectionAssertAreEqual(expectedReferences, enumerable);
		}
		
		[Test]
		public void GetEnumerator_UseUntypedEnumeratorProjectHasOneReference_OneReferenceReturned()
		{
			CreateReferences();
			msbuildProject.AddReference("System.Xml");
			
			var enumerable = references as IEnumerable;
			
			var expectedReferences = new string[] {
				"System.Xml"
			};
			
			ReferenceCollectionAssertAreEqual(expectedReferences, enumerable);
		}
		
		[Test]
		public void Find_SystemXmlWhenProjectHasSystemXmlReference_OneReferenceReturned()
		{
			CreateReferences();
			msbuildProject.AddReference("System.Xml");
			
			global::EnvDTE.Reference reference = references.Find("System.Xml");
			
			Assert.AreEqual("System.Xml", reference.Name);
		}
		
		[Test]
		public void Find_SystemXmlWhenProjectHasSystemXmlReferenceButWithDifferentCase_OneReferenceReturned()
		{
			CreateReferences();
			msbuildProject.AddReference("System.Xml");
			
			global::EnvDTE.Reference reference = references.Find("SYSTEM.XML");
			
			Assert.AreEqual("System.Xml", reference.Name);
		}
		
		[Test]
		public void Item_SystemXmlWhenProjectHasSystemXmlReference_OneReferenceReturned()
		{
			CreateReferences();
			msbuildProject.AddReference("System.Xml");
			
			global::EnvDTE.Reference reference = references.Item("System.Xml");
			
			Assert.AreEqual("System.Xml", reference.Name);
		}
		
		[Test]
		public void Item_SystemXmlProjectHasSystemXmlReference_OneReference3Returned()
		{
			CreateReferences();
			msbuildProject.AddReference("System.Xml");
			
			Reference3 reference = references.Item("System.Xml") as Reference3;
			
			Assert.AreEqual("System.Xml", reference.Name);
		}
	}
}
