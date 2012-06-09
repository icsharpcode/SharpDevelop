// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeElementsInNamespaceTests
	{
		CodeElementsInNamespace codeElements;
		ProjectContentHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateCodeElements(string namespaceName)
		{
			codeElements = new CodeElementsInNamespace(helper.ProjectContent, namespaceName);
		}
		
		[Test]
		public void Count_NoItemsInNamespace_ReturnsZero()
		{
			helper.NoCompletionItemsInNamespace("Test");
			CreateCodeElements("Test");
			
			int count = codeElements.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void GetEnumerator_NoItemsInNamespace_ReturnsNoItems()
		{
			helper.NoCompletionItemsInNamespace("Test");
			CreateCodeElements("Test");
			
			List<CodeElement> codeElementsList = codeElements.ToList();
			
			Assert.AreEqual(0, codeElementsList.Count);
		}
		
		[Test]
		public void GetEnumerator_OneNamespaceCompletionEntryInNamespace_ReturnsOneCodeNamespace()
		{
			helper.AddNamespaceCompletionEntryInNamespace("Parent", "Child");
			CreateCodeElements("Parent");
			
			CodeNamespace codeNamespace = codeElements.FirstCodeNamespaceOrDefault();
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("Child", codeNamespace.Name);
			Assert.AreEqual("Parent.Child", codeNamespace.FullName);
		}
		
		[Test]
		public void GetEnumerator_OneClassCompletionEntryInNamespace_ReturnsOneCodeClass()
		{
			helper.AddClassToProjectContentAndCompletionEntries("Test", "Test.MyClass");
			CreateCodeElements("Test");
			
			CodeClass2 codeClass = codeElements.ToList().FirstOrDefault() as CodeClass2;
			
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyClass", codeClass.Name);
			Assert.AreEqual("Test.MyClass", codeClass.FullName);
		}
		
		[Test]
		public void GetEnumerator_UnknownCompletionEntryInNamespace_ReturnsNoItems()
		{
			helper.AddUnknownCompletionEntryTypeToNamespace("Test");
			CreateCodeElements("Test");
			
			List<CodeElement> codeElementsList = codeElements.ToList();
			
			Assert.AreEqual(0, codeElementsList.Count);
		}
		
		[Test]
		public void GetEnumerator_EmptyNamespaceEntry_ReturnsNoItems()
		{
			helper.AddNamespaceCompletionEntryInNamespace(String.Empty, String.Empty);
			CreateCodeElements(String.Empty);
			
			List<CodeElement> codeElementsList = codeElements.ToList();
			
			Assert.AreEqual(0, codeElementsList.Count);
		}
		
		[Test]
		public void GetEnumerator_ParentChildAndGrandChildNamespaces_ReturnsOneCodeNamespaceWhichHasGrandChildNamespace()
		{
			helper.AddNamespaceCompletionEntryInNamespace("Parent", "Child");
			helper.AddNamespaceCompletionEntryInNamespace("Parent.Child", "GrandChild");
			helper.NoCompletionItemsInNamespace("Parent.Child.GrandChild");
			CreateCodeElements("Parent");
			
			CodeNamespace codeNamespace = codeElements.FirstCodeNamespaceOrDefault();
			CodeNamespace grandChildNamespace = codeNamespace.Members.FirstCodeNamespaceOrDefault();
			
			Assert.AreEqual("GrandChild", grandChildNamespace.Name);
		}
		
		[Test]
		public void Item_OneClassCompletionEntryAndFirstItemSelected_ReturnsOneCodeClass()
		{
			helper.AddClassToProjectContentAndCompletionEntries("Test", "Test.MyClass");
			CreateCodeElements("Test");
			
			CodeClass2 codeClass = codeElements.Item(1) as CodeClass2;
			
			Assert.AreEqual("Test.MyClass", codeClass.FullName);
		}
		
		[Test]
		public void Item_OneClassCompletionEntryAndItemSelectedByName_ReturnsOneCodeClass()
		{
			helper.AddClassToProjectContentAndCompletionEntries("Test", "Test.MyClass");
			CreateCodeElements("Test");
			
			CodeClass2 codeClass = codeElements.Item("MyClass") as CodeClass2;
			
			Assert.AreEqual("Test.MyClass", codeClass.FullName);
		}
		
		[Test]
		public void Item_OneInterfaceCompletionEntryAndItemSelectedByName_ReturnsOneCodeInterface()
		{
			helper.AddInterfaceToProjectContentAndCompletionEntries("Test", "Test.IClass");
			CreateCodeElements("Test");
			
			CodeInterface codeInterface = codeElements.Item("IClass") as CodeInterface;
			
			Assert.AreEqual("Test.IClass", codeInterface.FullName);
		}
	}
}
