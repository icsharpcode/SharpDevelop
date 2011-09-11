// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcProjectTests
	{
		MvcProject project;
		TestableProject testableProject;
		FakeMvcModelClassLocator fakeModelClassLocator;
		
		void CreateProject()
		{
			CreateProject(@"d:\projects\MyProject\MyProject.csproj");
		}
		
		void CreateProject(string fileName)
		{
			testableProject = TestableProject.CreateProject(fileName, "MyProject");
			fakeModelClassLocator = new FakeMvcModelClassLocator();
			project = new MvcProject(testableProject, fakeModelClassLocator);
		}
		
		void AddFileToProject(string fileName)
		{
			testableProject.AddFileToProject(fileName);
		}
		
		List<MvcMasterPageFileName> GetAspxMasterPageFileNames()
		{
			return new List<MvcMasterPageFileName>(
				project.GetAspxMasterPageFileNames()
			);
		}
		
		[Test]
		public void Project_ProjectPassedToConstructor_ReturnsProjectPassedToConstructor()
		{
			CreateProject();
			
			IProject msbuildProject = project.Project;
			
			Assert.AreEqual(testableProject, msbuildProject);
		}
		
		[Test]
		public void RootNamespace_ProjectPassedToConstructorHasRootNamespace_ReturnsRootNamespace()
		{
			CreateProject();
			testableProject.RootNamespace = "MyProjectNamespace";
			
			string rootNamespace = project.RootNamespace;
			
			Assert.AreEqual("MyProjectNamespace", rootNamespace);
		}
		
		[Test]
		public void Save_ProjectPassedToConstructor_ProjectIsSaved()
		{
			CreateProject();
			project.Save();
			
			bool saved = testableProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void GetTemplateLanguage_ProjectIsVisualBasicProject_ReturnsVisualBasicTemplateLanguage()
		{
			CreateProject();
			testableProject.SetLanguage("VBNet");
			
			MvcTextTemplateLanguage language = project.GetTemplateLanguage();
			
			Assert.AreEqual(MvcTextTemplateLanguage.VisualBasic, language);
		}
		
		[Test]
		public void GetTemplateLanguage_ProjectIsCSharpProject_ReturnsCSharpTemplateLanguage()
		{
			CreateProject();
			testableProject.SetLanguage("C#");
			
			MvcTextTemplateLanguage language = project.GetTemplateLanguage();
			
			Assert.AreEqual(MvcTextTemplateLanguage.CSharp, language);
		}
		
		[Test]
		public void GetModelClasses_OneModelClassInProject_ReturnsOneModelClass()
		{
			CreateProject();
			fakeModelClassLocator.AddModelClass("MyNamespace.MyClass");
			
			string[] modelClasses = project
				.GetModelClasses()
				.Select(m => m.FullName)
				.ToArray();
			
			string[] expectedModelClasses = new string[] {
				"MyNamespace.MyClass"
			};
			
			Assert.AreEqual(expectedModelClasses, modelClasses);
		}
		
		[Test]
		public void GetModelClasses_OneModelClassInProject_ProjectUsedToFindModelClasses()
		{
			CreateProject();
			fakeModelClassLocator.AddModelClass("MyNamespace.MyClass");
			project.GetModelClasses();
			
			Assert.AreEqual(project, fakeModelClassLocator.ProjectPassedToGetModelClasses);
		}
		
		[Test]
		public void GetAspxMasterPageFileNames_OneMasterPageInProject_ReturnsOneFileName()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\Site.Master");
			List<MvcMasterPageFileName> fileNames = GetAspxMasterPageFileNames();
			
			Assert.AreEqual(1, fileNames.Count);
		}
		
		[Test]
		public void GetAspxMasterPageFileNames_OneMasterPageInProject_ReturnsOneMasterPageWithExpectedFileName()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\Site.Master");
			MvcMasterPageFileName fileName = GetAspxMasterPageFileNames().First();
			
			var expectedFileName = new MvcMasterPageFileName() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\Site.Master",
				FileName = "Site.Master",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			MvcMasterPageFileNameAssert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetAspxMasterPageFileNames_OneHtmlFileAndOneMasterPageInProject_ReturnsOneMasterPageWithExpectedFileName()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\test.html");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\Site.Master");
			List<MvcMasterPageFileName> fileNames = GetAspxMasterPageFileNames();
			
			var expectedFileName = new MvcMasterPageFileName() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\Site.Master",
				FileName = "Site.Master",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			var expectedFileNames = new MvcMasterPageFileName[] {
				expectedFileName
			};
			
			MvcMasterPageFileNameCollectionAssert.AreEqual(expectedFileNames, fileNames);
		}
		
		[Test]
		public void GetAspxMasterPageFileNames_OneMasterPageWithFileExtensionInUpperCaseInProject_ReturnsOneMasterPageWithExpectedFileName()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\TEST.MASTER");
			MvcMasterPageFileName fileName = GetAspxMasterPageFileNames().First();
			
			var expectedFileName = new MvcMasterPageFileName() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\TEST.MASTER",
				FileName = "TEST.MASTER",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			MvcMasterPageFileNameAssert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void IsMvcMasterPage_NullFileNamePassed_ReturnsFalse()
		{
			bool result = MvcMasterPageFileName.IsMasterPageFileName(null);
			
			Assert.IsFalse(result);
		}
	}
}
