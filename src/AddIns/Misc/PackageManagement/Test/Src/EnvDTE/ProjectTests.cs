// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ProjectTests : CodeModelTestBase
	{
		void CreateProject(string fileName = @"d:\projects\MyProject\MyProject.csproj", string language = "C#")
		{
			msbuildProject = ProjectHelper.CreateTestProject();
			msbuildProject.FileName = new FileName(fileName);
			
			projectService = MockRepository.GenerateStub<IPackageManagementProjectService>();
			
			fileService = MockRepository.GenerateStub<IPackageManagementFileService>();
			dteProject = new Project(msbuildProject, projectService, fileService);
			
			msbuildProject.SetAssemblyModel(assemblyModel);
			
			fileService
				.Stub(fs => fs.GetCompilationUnit(msbuildProject))
				.WhenCalled(compilation => compilation.ReturnValue = projectContent.CreateCompilation());
		}
		
		void AddClassToProject(string code)
		{
			AddCodeFile("class.cs", code);
		}
		
		void SetParentSolutionFileName(string fileName)
		{
			var solutionFileName = new FileName(fileName);
			msbuildProject.ParentSolution.Stub(s => s.FileName).Return(solutionFileName);
			msbuildProject.ParentSolution.Stub(s => s.Directory).Return(solutionFileName.GetParentDirectory());
		}
		
		[Test]
		public void Name_ProjectNameIsMyApp_ReturnsMyApp()
		{
			CreateProject();
			msbuildProject.Name = "MyApp";
			
			string name = dteProject.Name;
			
			Assert.AreEqual("MyApp", name);
		}
		
		[Test]
		public void FullName_ProjectFileNameIsSet_ReturnsFullFileName()
		{
			string expectedFullName = @"d:\projects\myproject\myproject.csproj";
			CreateProject(expectedFullName);
			
			string fullName = dteProject.FullName;
			
			Assert.AreEqual(expectedFullName, fullName);
		}
		
		[Test]
		public void FileName_ProjectFileNameIsSet_ReturnsFullFileName()
		{
			string expectedFileName = @"d:\projects\myproject\myproject.csproj";
			CreateProject(expectedFileName);
			
			string fileName = dteProject.FileName;
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Type_ProjectIsCSharpProject_ReturnsCSharp()
		{
			CreateProject(@"c:\projects\myproject\test.csproj", "C#");
			
			string projectType = dteProject.Type;
			
			Assert.AreEqual("C#", projectType);
		}
		
		[Test]
		public void Type_ProjectIsCSharpProjectWithFileNameInUpperCase_ReturnsCSharp()
		{
			CreateProject(@"c:\projects\myproject\TEST.CSPROJ");
			
			string projectType = dteProject.Type;
			
			Assert.AreEqual("C#", projectType);
		}
		
		[Test]
		public void Type_ProjectIsVBProject_ReturnsVBNet()
		{
			CreateProject(@"c:\projects\myproject\test.vbproj");
			
			string projectType = dteProject.Type;
			
			Assert.AreEqual("VB.NET", projectType);
		}
		
		[Test]
		public void Type_ProjectHasUnknownProjectExtension_ReturnsEmptyString()
		{
			CreateProject(@"c:\projects\myproject\test.unknown");
			
			string projectType = dteProject.Type;
			
			Assert.AreEqual(String.Empty, projectType);
		}
		
		[Test]
		public void Kind_ProjectIsCSharpProject_ReturnsCSharpProjectTypeGuid()
		{
			CreateProject(@"d:\projects\myproject\test.csproj");
			
			string kind = dteProject.Kind;
			
			Assert.AreEqual(ProjectTypeGuids.CSharp.ToString(), kind);
		}
		
		[Test]
		public void Kind_ProjectIsVBNetProject_ReturnsCSharpProjectTypeGuid()
		{
			CreateProject( @"d:\projects\myproject\test.vbproj");
			
			string kind = dteProject.Kind;
			
			Assert.AreEqual(ProjectTypeGuids.VBNet.ToString(), kind);
		}
		
		[Test]
		public void Kind_ProjectHasUnknownFileExtension_ReturnsEmptyString()
		{
			CreateProject(@"d:\projects\myproject\test.unknown");
			
			string kind = dteProject.Kind;
			
			Assert.AreEqual(String.Empty, kind);
		}
		
		[Test]
		public void UniqueName_ProjectInSameFolderAsSolution_ReturnsProjectFileNameWithoutDirectoryPart()
		{
			CreateProject(@"d:\projects\myproject\MyProject.csproj");
			SetParentSolutionFileName(@"d:\projects\myproject\MyProject.sln");
			
			string name = dteProject.UniqueName;
			
			Assert.AreEqual("MyProject.csproj", name);
		}
		
		[Test]
		public void UniqueName_ProjectInSubDirectoryOfSolutionFolder_ReturnsProjectFileNameWithContainsSubFolder()
		{
			CreateProject(@"d:\projects\myproject\SubFolder\MyProject.csproj");
			SetParentSolutionFileName(@"d:\projects\myproject\MyProject.sln");
			
			string name = dteProject.UniqueName;
			
			Assert.AreEqual(@"SubFolder\MyProject.csproj", name);
		}
		
		[Test]
		public void ProjectItemsParent_ParentOfProjectsProjectItems_ReturnsTheProject()
		{
			CreateProject();
			
			object parent = dteProject.ProjectItems.Parent;
			
			Assert.AreEqual(dteProject, parent);
		}
		
		[Test]
		public void ConfigurationManager_ActiveConfigurationOutputPathProperty_ReturnsOutputPathForProject()
		{
			CreateProject();
			msbuildProject.SetProperty("OutputPath", @"bin\debug\");
			global::EnvDTE.Configuration activeConfig = dteProject.ConfigurationManager.ActiveConfiguration;
			
			string outputPath = (string)activeConfig.Properties.Item("OutputPath").Value;
			
			Assert.AreEqual(@"bin\debug\", outputPath);
		}
		
		[Test]
		public void CodeModel_NoTypesInProjectAndCallCodeTypeFromFullName_ReturnsNull()
		{
			CreateProject();
			AddClassToProject("");
			
			global::EnvDTE.CodeType codeType = dteProject.CodeModel.CodeTypeFromFullName("UnknownTypeName");
			
			Assert.IsNull(codeType);
		}
		
		[Test]
		public void CodeModel_ClassExistsInProjectContentAndCallCodeTypeFromFullName_ReturnsNonCodeType()
		{
			CreateProject();
			AddClassToProject(
				"namespace Tests {\r\n" +
				"    public class MyClass {}\r\n"+
				"}");
			
			global::EnvDTE.CodeType codeType = dteProject.CodeModel.CodeTypeFromFullName("Tests.MyClass");
			
			Assert.IsNotNull(codeType);
		}
		
		[Test]
		public void CodeModel_ClassExistsInProject_ReturnsClassWithLocationSetToInProject()
		{
			CreateProject();
			AddClassToProject("public class MyClass {}");
			
			CodeElement element = dteProject.CodeModel.CodeElements
				.FindFirstOrDefault(e => e.Name == "MyClass");
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, element.InfoLocation);
		}
		
		[Test]
		public void CodeModel_ClassExistsInDifferentAssembly_ReturnsClassWithLocationSetToExternal()
		{
			CreateProject();
			
			CodeElement element = dteProject
				.CodeModel
				.CodeElements
				.FindFirstCodeNamespaceOrDefault(e => e.Name == "System")
				.Members
				.FindFirstOrDefault(e => e.Name == "String");
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, element.InfoLocation);
		}
		
		[Test]
		public void CodeModel_EmptyNamespaceExistsInProject_CodeElementsReturnsNamespace()
		{
			CreateProject();
			AddClassToProject("namespace Test {}");
			
			global::EnvDTE.CodeElements codeElements = dteProject.CodeModel.CodeElements;
			global::EnvDTE.CodeNamespace codeNamespace = codeElements
				.FindFirstCodeNamespaceOrDefault(e => e.Name == "Test");
			
			Assert.AreEqual("Test", codeNamespace.FullName);
			Assert.AreEqual("Test", codeNamespace.Name);
		}
	}
}
