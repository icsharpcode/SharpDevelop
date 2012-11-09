// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeModelTests
	{
		CodeModel codeModel;
		ProjectContentHelper helper;
		TestableProject msbuildProject;
		
		void CreateCodeModel()
		{
			CreateProjectContentHelper();
			CreateProjectForProjectContent();
			CreateCodeModel(helper.ProjectContent);
		}
		
		void CreateProjectForProjectContent()
		{
			msbuildProject = ProjectHelper.CreateTestProject();
			helper.SetProjectForProjectContent(msbuildProject);
		}

		void CreateProjectContentHelper()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateCodeModel(IProjectContent projectContent)
		{
			codeModel = new CodeModel(projectContent);
		}
		
		void CreateCodeModelWithCSharpProject()
		{
			CreateProjectContentHelper();
			helper.ProjectContentIsForCSharpProject();
			CreateCodeModel(helper.ProjectContent);
		}
		
		void CreateCodeModelWithVisualBasicProject()
		{
			CreateProjectContentHelper();
			helper.ProjectContentIsForVisualBasicProject();
			CreateCodeModel(helper.ProjectContent);
		}
		
		void AddClassToProjectContent(string className)
		{
			helper.AddClassToProjectContent(className);
		}
		
		void AddClassToDifferentProjectContent(string className)
		{
			helper.AddClassToDifferentProjectContent(className);
		}
		
		void AddClassToProjectContent(string namespaceName, string className)
		{
			helper.AddClassToProjectContentAndCompletionEntries(namespaceName, className);
		}
		
		void AddInterfaceToProjectContent(string interfaceName)
		{
			helper.AddInterfaceToProjectContent(interfaceName);
		}
		
		void AddInterfaceToDifferentProjectContent(string interfaceName)
		{
			helper.AddInterfaceToDifferentProjectContent(interfaceName);
		}
		
		void ProjectIsCSharpProject()
		{
			helper.ProjectContentIsForCSharpProject();
		}
		
		void ProjectIsVisualBasicProject()
		{
			helper.ProjectContentIsForVisualBasicProject();
		}
		
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
			AddClassToProjectContent("Tests.TestClass");
			
			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
			
			Assert.AreEqual("Tests.TestClass", codeClass.FullName);
			Assert.AreEqual("TestClass", codeClass.Name);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassWithoutNamespaceExistsInProject_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProjectContent("TestClass");
			
			var codeClass = codeModel.CodeTypeFromFullName("TestClass") as CodeClass2;
			
			Assert.AreEqual("TestClass", codeClass.FullName);
		}
		
		[Test]
		public void CodeTypeFromFullName_InterfaceExistsInProject_ReturnsCodeInterface()
		{
			CreateCodeModel();
			AddInterfaceToProjectContent("Interface1");
			
			var codeInterface = codeModel.CodeTypeFromFullName("Interface1") as CodeInterface;
			
			Assert.AreEqual("Interface1", codeInterface.FullName);
		}
		
		[Test]
		public void CodeElements_OneNamespaceInProject_ReturnsOneCodeNamespaceItem()
		{
			CreateCodeModel();
			helper.AddNamespaceCompletionEntryInNamespace(String.Empty, "Test");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			global::EnvDTE.CodeNamespace codeNamespace = codeElements.FirstCodeNamespaceOrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("Test", codeNamespace.FullName);
			Assert.AreEqual("Test", codeNamespace.Name);
		}
		
		[Test]
		public void CodeElements_OneNamespaceInProjectWithTwoPartsToName_ReturnsOneCodeNamespaceItemWithFirstPartOfNamespaceAsName()
		{
			CreateCodeModel();
			helper.AddNamespaceCompletionEntryInNamespace(String.Empty, "First");
			helper.AddNamespaceCompletionEntryInNamespace("First", "Second");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			global::EnvDTE.CodeNamespace codeNamespace = codeElements.FirstCodeNamespaceOrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("First", codeNamespace.FullName);
			Assert.AreEqual("First", codeNamespace.Name);
		}
		
		[Test]
		public void CodeElements_OneClassWithNoNamespaceInProject_ReturnsOneCodeClassItem()
		{
			CreateCodeModel();
			AddClassToProjectContent(String.Empty, "TestClass");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			CodeClass2 codeClass = codeElements.FirstCodeClass2OrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("TestClass", codeClass.FullName);
		}
		
		[Test]
		public void CodeElements_TwoNamespacesInProjectWithFirstPartsTheName_ReturnsOneParentNamespaceWithTwoChildNamespaces()
		{
			CreateCodeModel();
			helper.AddNamespaceCompletionEntryInNamespace(String.Empty, "First");
			helper.AddNamespaceCompletionEntriesInNamespace("First", "A", "B");
			helper.NoCompletionItemsInNamespace("First.A");
			helper.NoCompletionItemsInNamespace("First.B");
			
			global::EnvDTE.CodeElements codeElements = codeModel.CodeElements;
			CodeNamespace codeNamespace = codeElements.FirstCodeNamespaceOrDefault();
			
			global::EnvDTE.CodeElements members = codeNamespace.Members;
			CodeNamespace firstChildNamespace = members.FirstCodeNamespaceOrDefault();
			CodeNamespace secondChildNamespace = members.LastCodeNamespaceOrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("First", codeNamespace.FullName);
			Assert.AreEqual(2, codeNamespace.Members.Count);
			Assert.AreEqual("A", firstChildNamespace.Name);
			Assert.AreEqual("B", secondChildNamespace.Name);
		}
		
		[Test]
		public void CodeElements_ProjectHasEmptyNamespaceName_EmptyNamespaceNameNotIncludedInMembers()
		{
			CreateCodeModel();
			helper.AddNamespaceCompletionEntriesInNamespace(String.Empty, String.Empty, "Tests");
			
			global::EnvDTE.CodeElements members = codeModel.CodeElements;
			CodeNamespace codeNamespace = members.FirstCodeNamespaceOrDefault();
			
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Tests", codeNamespace.Name);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassExistsInProject_InfoLocationIsLocalProject()
		{
			CreateCodeModel();
			AddClassToProjectContent("Tests.TestClass");
			
			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, codeClass.InfoLocation);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassExistsInDifferentProject_InfoLocationIsExternal()
		{
			CreateCodeModel();
			AddClassToDifferentProjectContent("Tests.TestClass");
			
			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, codeClass.InfoLocation);
		}
		
		[Test]
		public void CodeTypeFromFullName_InterfaceExistsInProject_InfoLocationIsLocalProject()
		{
			CreateCodeModel();
			AddInterfaceToProjectContent("Tests.ITest");
			
			var codeInterface = codeModel.CodeTypeFromFullName("Tests.ITest") as CodeInterface;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject, codeInterface.InfoLocation);
		}
		
		[Test]
		public void CodeTypeFromFullName_InterfaceExistsInDifferentProject_InfoLocationIsExternal()
		{
			CreateCodeModel();
			AddInterfaceToDifferentProjectContent("Tests.ITest");
			
			var codeInterface = codeModel.CodeTypeFromFullName("Tests.ITest") as CodeInterface;
			
			Assert.AreEqual(global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal, codeInterface.InfoLocation);
		}
		
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
	}
}
