// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeModelTests : CodeModelTestBase
	{
		CodeModel codeModel;
		Project dteProject;
		IPackageManagementProjectService fakeProjectService;
		IPackageManagementFileService fakeFileService;
		TestableProject msbuildProject;
		
		void CreateCodeModel()
		{
			msbuildProject = ProjectHelper.CreateTestProject();
			
			fakeProjectService = MockRepository.GenerateStub<IPackageManagementProjectService>();
			fakeFileService = MockRepository.GenerateStub<IPackageManagementFileService>();
			dteProject = new Project(msbuildProject, fakeProjectService, fakeFileService);
			codeModelContext.DteProject = dteProject;
			
			codeModel = new CodeModel(codeModelContext, dteProject);
			
			msbuildProject.SetAssemblyModel(assemblyModel);
			project.Stub(p => p.AssemblyModel).Return(assemblyModel);
			
			fakeFileService
				.Stub(fileService => fileService.GetCompilationUnit(msbuildProject))
				.WhenCalled(compilation => compilation.ReturnValue = CreateCompilation());
		}
		
		ICompilation CreateCompilation()
		{
			var solutionSnapshot = new TestableSolutionSnapshot(msbuildProject);
			msbuildProject.SetProjectContent(projectContent);
			ICompilation compilation = new SimpleCompilation(solutionSnapshot, projectContent, projectContent.AssemblyReferences);
			solutionSnapshot.AddCompilation(projectContent, compilation);
			return compilation;
		}
		
		void CreateCodeModelWithCSharpProject()
		{
			CreateCodeModel();
			msbuildProject.FileName = new FileName(@"c:\projects\MyProject.csproj");
		}
		
		void CreateCodeModelWithVisualBasicProject()
		{
			CreateCodeModel();
			msbuildProject.FileName = new FileName(@"c:\projects\MyProject.vbproj");
		}
		
		void AddClassToProject(string code)
		{
			AddCodeFile("class.cs", code);
		}
		
//		void AddClassToDifferentProjectContent(string className)
//		{
//			helper.AddClassToDifferentProjectContent(className);
//		}
//		
//		void AddClassToProjectContent(string namespaceName, string className)
//		{
//			helper.AddClassToProjectContentAndCompletionEntries(namespaceName, className);
//		}
//		
//		void AddInterfaceToProjectContent(string interfaceName)
//		{
//			helper.AddInterfaceToProjectContent(interfaceName);
//		}
//		
//		void AddInterfaceToDifferentProjectContent(string interfaceName)
//		{
//			helper.AddInterfaceToDifferentProjectContent(interfaceName);
//		}
//		
		[Test]
		public void CodeTypeFromFullName_NoSuchTypeInProject_ReturnsNull()
		{
			CreateCodeModel();
			
			global::EnvDTE.CodeType codeType = codeModel.CodeTypeFromFullName("UnknownType");
			
			Assert.IsNull(codeType);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassExistsInProject_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProject(
				"namespace Tests {\r\n" +
				"    public class TestClass {} \r\n" +
				"}");
			
			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
			
			Assert.AreEqual("Tests.TestClass", codeClass.FullName);
			Assert.AreEqual("TestClass", codeClass.Name);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassWithoutNamespaceExistsInProject_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProject("public class TestClass {}");
			
			var codeClass = codeModel.CodeTypeFromFullName("TestClass") as CodeClass2;
			
			Assert.AreEqual("TestClass", codeClass.FullName);
		}
		
		[Test]
		public void CodeTypeFromFullName_InterfaceExistsInProject_ReturnsCodeInterface()
		{
			CreateCodeModel();
			AddClassToProject("public interface Interface1 {}");
			
			var codeInterface = codeModel.CodeTypeFromFullName("Interface1") as CodeInterface;
			
			Assert.AreEqual("Interface1", codeInterface.FullName);
		}
		
		[Test]
		public void CodeElements_OneNamespaceInProject_ReturnsOneCodeNamespaceItem()
		{
			CreateCodeModel();
			AddClassToProject("namespace Test {}");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			global::EnvDTE.CodeNamespace codeNamespace = codeElements
				.FindFirstCodeNamespaceOrDefault(e => e.Name == "Test");
			
			Assert.AreEqual("Test", codeNamespace.FullName);
			Assert.AreEqual("Test", codeNamespace.Name);
		}
		
		[Test]
		public void CodeElements_OneNamespaceInProjectWithTwoPartsToName_ReturnsOneCodeNamespaceItemWithFirstPartOfNamespaceAsName()
		{
			CreateCodeModel();
			AddClassToProject("namespace First.Second {}");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			global::EnvDTE.CodeNamespace codeNamespace = codeElements
				.FindFirstCodeNamespaceOrDefault(e => e.Name == "First");
			
			global::EnvDTE.CodeNamespace secondCodeNamespace = codeNamespace.Members.FirstCodeNamespaceOrDefault();
			Assert.AreEqual("First", codeNamespace.FullName);
			Assert.AreEqual("First", codeNamespace.Name);
			Assert.AreEqual("Second", secondCodeNamespace.Name);
			Assert.AreEqual("First.Second", secondCodeNamespace.FullName);
		}
		
		[Test]
		public void CodeElements_OneClassWithNoNamespaceInProject_ReturnsOneCodeClassItem()
		{
			CreateCodeModel();
			AddClassToProject("public class TestClass { }");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			CodeClass2 codeClass = codeElements
				.FindFirstCodeClass2OrDefault(e => e.FullName == "TestClass");
			
			Assert.AreEqual("TestClass", codeClass.Name);
		}
		
		[Test]
		public void CodeElements_TwoNamespacesInProjectWithFirstPartsTheName_ReturnsOneParentNamespaceWithTwoChildNamespaces()
		{
			CreateCodeModel();
			string code = 
				"namespace First.A { }\r\n" +
				"namespace First.B { }\r\n";
			AddClassToProject(code);
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			CodeNamespace codeNamespace = codeElements.FirstCodeNamespaceOrDefault();
			
			global::EnvDTE.CodeElements members = codeNamespace.Members;
			CodeNamespace firstChildNamespace = members.FirstCodeNamespaceOrDefault();
			CodeNamespace secondChildNamespace = members.LastCodeNamespaceOrDefault();
			
			Assert.AreEqual("First", codeNamespace.FullName);
			Assert.AreEqual(2, codeNamespace.Members.Count);
			Assert.AreEqual("A", firstChildNamespace.Name);
			Assert.AreEqual("B", secondChildNamespace.Name);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassExistsInProject_InfoLocationIsLocalProject()
		{
			CreateCodeModel();
			AddClassToProject(
				"namespace Tests {\r\n" +
				"    public class TestClass {} \r\n" +
				"}");
			
			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, codeClass.InfoLocation);
		}
//		
//		[Test]
//		public void CodeTypeFromFullName_ClassExistsInDifferentProject_InfoLocationIsExternal()
//		{
//			CreateCodeModel();
//			AddClassToDifferentProjectContent("Tests.TestClass");
//			
//			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, codeClass.InfoLocation);
//		}
//		
//		[Test]
//		public void CodeTypeFromFullName_InterfaceExistsInProject_InfoLocationIsLocalProject()
//		{
//			CreateCodeModel();
//			AddInterfaceToProjectContent("Tests.ITest");
//			
//			var codeInterface = codeModel.CodeTypeFromFullName("Tests.ITest") as CodeInterface;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, codeInterface.InfoLocation);
//		}
//		
//		[Test]
//		public void CodeTypeFromFullName_InterfaceExistsInDifferentProject_InfoLocationIsExternal()
//		{
//			CreateCodeModel();
//			AddInterfaceToDifferentProjectContent("Tests.ITest");
//			
//			var codeInterface = codeModel.CodeTypeFromFullName("Tests.ITest") as CodeInterface;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, codeInterface.InfoLocation);
//		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpProjectGuid()
		{
			CreateCodeModelWithCSharpProject();
			
			string language = codeModel.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		public void Language_VisualBasicProject_ReturnsVisualBasicProjectGuid()
		{
			CreateCodeModelWithVisualBasicProject();
			
			string language = codeModel.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, language);
		}
		
		[Test]
		[Ignore("TODO - Use NRefactory")]
		public void CodeTypeFromFullName_SystemString_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProject("public class TestClass {}");
			
			var codeClass = codeModel.CodeTypeFromFullName("System.String") as CodeClass2;
			
			Assert.AreEqual("System.String", codeClass.FullName);
		}
	}
}
