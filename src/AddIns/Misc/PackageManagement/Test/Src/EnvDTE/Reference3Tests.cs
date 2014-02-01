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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class Reference3Tests
	{
		Reference3 reference;
		TestableProject msbuildProject;
		FakePackageManagementProjectService fakeProjectService;
		TestableDTEProject project;
		ReferenceProjectItem referenceProjectItem;
		
		void CreateReference(string name)
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			referenceProjectItem = msbuildProject.AddReference(name);
			fakeProjectService = project.FakeProjectService;
			CreateReference(project, referenceProjectItem);
			IWorkbench workbench = MockRepository.GenerateStub<IWorkbench>();
			ICSharpCode.SharpDevelop.SD.Services.AddService(typeof(IWorkbench), workbench);
		}
		
		void CreateReference(Project project, ReferenceProjectItem referenceProjectItem)
		{
			reference = new Reference3(project, referenceProjectItem);
		}
		
		TestableProject CreateProjectReference(string parentProjectFileName, string referencedProjectFileName)
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			msbuildProject.FileName = new FileName(parentProjectFileName);
			TestableProject referencedProject = ProjectHelper.CreateTestProject();
			referencedProject.FileName = new FileName(referencedProjectFileName);
			((ICollection<IProject>)msbuildProject.ParentSolution.Projects).Add(referencedProject);
			IWorkbench workbench = MockRepository.GenerateStub<IWorkbench>();
			ICSharpCode.SharpDevelop.SD.Services.AddService(typeof(IWorkbench), workbench);
			ProjectReferenceProjectItem referenceProjectItem = msbuildProject.AddProjectReference(referencedProject);
			fakeProjectService = project.FakeProjectService;
			CreateReference(project, referenceProjectItem);
			return referencedProject;
		}
		
		[Test]
		public void Name_ReferenceNameIsSystemXml_ReturnsSystemXml()
		{
			CreateReference("System.Xml");
			string name = reference.Name;
			
			Assert.AreEqual("System.Xml", name);
		}
		
		[Test]
		public void Remove_RemoveSystemXmlReferenceFromProject_ProjectReferenceRemoved()
		{
			CreateReference("System.Xml");
			
			reference.Remove();
			
			int count = msbuildProject.Items.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Remove_RemoveSystemXmlReferenceFromProject_ProjectIsSaved()
		{
			CreateReference("System.Xml");
			
			reference.Remove();
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void SourceProject_SystemXmlReference_ReturnsNull()
		{
			CreateReference("System.Xml");
			
			global::EnvDTE.Project project = reference.SourceProject;
			
			Assert.IsNull(project);
		}
		
		[Test]
		public void SourceProject_ReferenceIsProjectReference_ReturnsReferencedProject()
		{
			string parentProjectFileName = @"d:\projects\project.csproj";
			string referencedProjectFileName = @"d:\projects\referencedproject.csproj";
			TestableProject referencedProject = CreateProjectReference(parentProjectFileName, referencedProjectFileName);
			
			global::EnvDTE.Project project = reference.SourceProject;
			
			Assert.AreEqual(@"d:\projects\referencedproject.csproj", project.FileName);
		}
		
		[Test]
		public void AutoReferenced_SystemXmlReferenceInProjectReferences_ReturnsFalse()
		{
			CreateReference("System.Xml");
			
			bool result = reference.AutoReferenced;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Path_SystemXmlReferenceInProjectReferences_ReturnsFullPathToSystemXml()
		{
			CreateReference("System.Xml");
			referenceProjectItem.FileName = FileName.Create(@"c:\Program Files\Microsoft\Reference Assemblies\v4\System.Xml.dll");
			
			string path = reference.Path;
			
			Assert.AreEqual(@"c:\Program Files\Microsoft\Reference Assemblies\v4\System.Xml.dll", path);
		}
		
		[Test]
		public void PublicKeyToken_ReferenceHasPublicKeyToken_ReturnsPublicKeyToken()
		{
			CreateReference("ICSharpCode.Core, Version=4.4.0.0, Culture=neutral, PublicKeyToken=f829da5c02be14ee");
			
			string publicKeyToken = reference.PublicKeyToken;
			
			Assert.AreEqual("f829da5c02be14ee", publicKeyToken);
		}
		
		[Test]
		public void PublicKeyToken_ReferenceHasNullPublicKeyToken_ReturnsEmptyString()
		{
			CreateReference("ICSharpCode.Core, Version=4.4.0.0, Culture=neutral, PublicKeyToken=null");
			
			string publicKeyToken = reference.PublicKeyToken;
			
			Assert.AreEqual(String.Empty, publicKeyToken);
		}
		
		[Test]
		public void StrongName_ReferenceHasStrongName_ReturnsTrue()
		{
			CreateReference("ICSharpCode.Core, Version=4.4.0.0, Culture=neutral, PublicKeyToken=f829da5c02be14ee");
			
			bool result = reference.StrongName;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void StrongName_ReferenceMissingVersion_ReturnsFalse()
		{
			CreateReference("ICSharpCode.Core, Culture=neutral, PublicKeyToken=f829da5c02be14ee");
			
			bool result = reference.StrongName;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void StrongName_ReferenceMissingPublicKeyToken_ReturnsFalse()
		{
			CreateReference("ICSharpCode.Core, Version=4.4.0.0, Culture=neutral");
			
			bool result = reference.StrongName;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void StrongName_ReferenceHasNullPublicKeyToken_ReturnsFalse()
		{
			CreateReference("ICSharpCode.Core, Version=4.4.0.0, Culture=neutral, PublicKeyToken=null");
			
			bool result = reference.StrongName;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Identity_FullyQualifiedReference_ReturnsAssemblyShortName()
		{
			CreateReference("ICSharpCode.Core, Version=4.4.0.0, Culture=neutral, PublicKeyToken=f829da5c02be14ee");
			
			string identity = reference.Identity;
			
			Assert.AreEqual("ICSharpCode.Core", identity);
		}
	}
}
