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
		
		List<MvcProjectFile> GetAspxMasterPageFiles()
		{
			return new List<MvcProjectFile>(
				project.GetAspxMasterPageFiles()
			);
		}
		
		List<MvcProjectFile> GetRazorFiles()
		{
			return new List<MvcProjectFile>(
				project.GetRazorFiles()
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
		public void GetAspxMasterPageFiles_OneMasterPageInProject_ReturnsOneFile()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\Site.Master");
			List<MvcProjectFile> files = GetAspxMasterPageFiles();
			
			Assert.AreEqual(1, files.Count);
		}
		
		[Test]
		public void GetAspxMasterPageFiles_OneMasterPageInProject_ReturnsOneMasterPageWithExpectedFile()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\Site.Master");
			MvcProjectFile file = GetAspxMasterPageFiles().First();
			
			var expectedFile = new MvcProjectFile() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\Site.Master",
				FileName = "Site.Master",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			MvcProjectFileAssert.AreEqual(expectedFile, file);
		}
		
		[Test]
		public void GetAspxMasterPageFiles_OneHtmlFileAndOneMasterPageInProject_ReturnsOneMasterPageWithExpectedFileName()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\test.html");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\Site.Master");
			List<MvcProjectFile> files = GetAspxMasterPageFiles();
			
			var expectedFileName = new MvcProjectFile() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\Site.Master",
				FileName = "Site.Master",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			var expectedFiles = new MvcProjectFile[] {
				expectedFileName
			};
			
			MvcProjectFileCollectionAssert.AreEqual(expectedFiles, files);
		}
		
		[Test]
		public void GetAspxMasterPageFiles_OneMasterPageWithFileExtensionInUpperCaseInProject_ReturnsOneMasterPageWithExpectedFileName()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\TEST.MASTER");
			MvcProjectFile fileName = GetAspxMasterPageFiles().First();
			
			var expectedFileName = new MvcProjectFile() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\TEST.MASTER",
				FileName = "TEST.MASTER",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			MvcProjectFileAssert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetRazorFiles_OneRazorFileInProject_ReturnsOneFile()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\_Layout.cshtml");
			List<MvcProjectFile> files = GetRazorFiles();
			
			Assert.AreEqual(1, files.Count);
		}
		
		[Test]
		public void GetRazorFiles_OneRazorFileInProject_ReturnsOneRazorFileWithExpectedFile()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\_Layout.cshtml");
			MvcProjectFile file = GetRazorFiles().First();
			
			var expectedFile = new MvcProjectFile() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\_Layout.cshtml",
				FileName = "_Layout.cshtml",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			MvcProjectFileAssert.AreEqual(expectedFile, file);
		}
		
		[Test]
		public void GetRazorFiles_OneHtmlFileAndOneRazorFileInProject_ReturnsOneRazorFileWithExpectedFileName()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\test.html");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\test.cshtml");
			List<MvcProjectFile> files = GetRazorFiles();
			
			var expectedFileName = new MvcProjectFile() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\test.cshtml",
				FileName = "test.cshtml",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			var expectedFiles = new MvcProjectFile[] {
				expectedFileName
			};
			
			MvcProjectFileCollectionAssert.AreEqual(expectedFiles, files);
		}
		
		[Test]
		public void GetRazorFiles_OneRazorWithFileExtensionInUpperCaseInProject_ReturnsOneRazorFileWithExpectedFileName()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			AddFileToProject(@"d:\projects\AspNetMvcProject\Views\Shared\TEST.CSHTML");
			MvcProjectFile fileName = GetRazorFiles().First();
			
			var expectedFileName = new MvcProjectFile() {
				FullPath = @"d:\projects\AspNetMvcProject\Views\Shared\TEST.CSHTML",
				FileName = "TEST.CSHTML",
				FolderRelativeToProject = @"Views\Shared"
			};
			
			MvcProjectFileAssert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void OutputAssemblyFullPath_ProjectOutputAssemblyIsSet_ReturnsProjectOutputAssemblyFullPath()
		{
			CreateProject(@"d:\projects\AspNetMvcProject\MyProject.csproj");
			string expectedAssemblyPath = @"d:\projects\AspNetMvcProject\bin\MyProject.dll";
			testableProject.SetOutputAssemblyFullPath(expectedAssemblyPath);
			
			string assemblyPath = project.OutputAssemblyFullPath;
			
			Assert.AreEqual(expectedAssemblyPath, assemblyPath);
		}
		
		[Test]
		public void IsVisualBasic_ProjectIsVisualBasic_ReturnsTrue()
		{
			CreateProject();
			testableProject.SetLanguage(MvcTextTemplateLanguageConverter.VisualBasicProjectLanguage);
			
			bool result = project.IsVisualBasic();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsVisualBasic_ProjectIsCSharp_ReturnsFalse()
		{
			CreateProject();
			testableProject.SetLanguage("C#");
			
			bool result = project.IsVisualBasic();
			
			Assert.IsFalse(result);
		}
	}
}
