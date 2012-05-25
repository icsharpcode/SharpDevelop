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
		
		void CreateCodeModel()
		{
			helper = new ProjectContentHelper();
			codeModel = new CodeModel(helper.FakeProjectContent);
		}
		
		void AddClassToProjectContent(string className)
		{
			helper.AddClassToProjectContent(className);
		}
		
		void AddClassToProjectContent(string namespaceName, string className)
		{
			helper.AddClassToProjectContent(namespaceName, className);
		}
		
		void AddInterfaceToProjectContent(string interfaceName)
		{
			helper.AddInterfaceToProjectContent(interfaceName);
		}
		
		void AddNamespaceToProjectContent(string name)
		{
			helper.AddNamespaceNameToProjectContent(name);
		}
				
		void AddEmptyNamespaceContentsForRootNamespace()
		{
			helper.AddEmptyNamespaceContentsForRootNamespace();
		}
		
		void AddEmptyNamespaceContents(string namespaceName)
		{
			helper.AddEmptyNamespaceContents(namespaceName);
		}
		
		[Test]
		public void CodeTypeFromFullName_NoSuchTypeInProject_ReturnsNull()
		{
			CreateCodeModel();
			
			CodeType codeType = codeModel.CodeTypeFromFullName("UnknownType");
			
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
			AddNamespaceToProjectContent("Test");
			AddEmptyNamespaceContentsForRootNamespace();
			
			CodeElements codeElements = codeModel.CodeElements;
			CodeNamespace codeNamespace = codeElements.FirstOrDefault() as CodeNamespace;
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("Test", codeNamespace.FullName);
			Assert.AreEqual("Test", codeNamespace.Name);
		}
		
		[Test]
		public void CodeElements_OneNamespaceInProjectWithTwoPartsToName_ReturnsOneCodeNamespaceItemWithFirstPartOfNamespaceAsName()
		{
			CreateCodeModel();
			AddNamespaceToProjectContent("First.Second");
			AddEmptyNamespaceContentsForRootNamespace();
			
			CodeElements codeElements = codeModel.CodeElements;
			CodeNamespace codeNamespace = codeElements.FirstOrDefault() as CodeNamespace;
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("First", codeNamespace.FullName);
			Assert.AreEqual("First", codeNamespace.Name);
		}
		
		[Test]
		public void CodeElements_OneClassWithNoNamespaceInProject_ReturnsOneCodeClassItem()
		{
			CreateCodeModel();
			AddClassToProjectContent(String.Empty, "TestClass");
			
			CodeElements codeElements = codeModel.CodeElements;
			CodeClass2 codeClass = codeElements.FirstOrDefault() as CodeClass2;
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("TestClass", codeClass.FullName);
		}
		
		[Test]
		public void CodeElements_TwoNamespacesInProjectWithFirstPartsTheName_ReturnsOneParentNamespaceWithTwoChildNamespaces()
		{
			CreateCodeModel();
			AddNamespaceToProjectContent("First.A");
			AddNamespaceToProjectContent("First.B");
			AddEmptyNamespaceContentsForRootNamespace();
			AddEmptyNamespaceContents("First");
			
			CodeElements codeElements = codeModel.CodeElements;
			CodeNamespace codeNamespace = codeElements.FirstOrDefault() as CodeNamespace;
			
			List<CodeElement> members = codeNamespace.Members.ToList();
			CodeNamespace firstChildNamespace = members.FirstOrDefault() as CodeNamespace;
			CodeNamespace secondChildNamespace = members.LastOrDefault() as CodeNamespace;
			
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
			AddNamespaceToProjectContent(String.Empty);
			AddNamespaceToProjectContent("Tests");
			AddEmptyNamespaceContentsForRootNamespace();
			AddEmptyNamespaceContents("Tests");
			
			CodeElements members = codeModel.CodeElements;
			CodeNamespace codeNamespace = members.ToList().FirstOrDefault() as CodeNamespace;
			
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("Tests", codeNamespace.Name);
		}
	}
}
