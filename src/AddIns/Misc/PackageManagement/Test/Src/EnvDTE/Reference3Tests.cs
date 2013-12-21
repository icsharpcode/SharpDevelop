// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

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
		}
		
		void CreateReference(Project project, ReferenceProjectItem referenceProjectItem)
		{
			reference = new Reference3(project, referenceProjectItem);
		}
		
		TestableProject CreateProjectReference()
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			TestableProject referencedProject = ProjectHelper.CreateTestProject();
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
			TestableProject referencedProject = CreateProjectReference();
			referencedProject.FileName = @"d:\projects\referencedproject.csproj";
			
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
			referenceProjectItem.FileName = @"c:\Program Files\Microsoft\Reference Assemblies\v4\System.Xml.dll";
			
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
