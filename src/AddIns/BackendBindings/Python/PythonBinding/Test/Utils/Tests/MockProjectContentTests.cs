// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockProjectContentTests
	{
		MockProjectContent projectContent;
		ArrayList items;
		
		[SetUp]
		public void Init()
		{
			projectContent = new MockProjectContent();
			items = new ArrayList();
		}
		
		[Test]
		public void AddNamespaceContentsAddsNamespaces()
		{
			projectContent.NamespacesToAdd.Add("test");
			projectContent.AddNamespaceContents(items, String.Empty, PythonLanguageProperties.Default, false);
			
			ArrayList expectedItems = new ArrayList();
			expectedItems.Add("test");
			
			Assert.AreEqual(expectedItems, items);
		}
			
		[Test]
		public void AddNamespaceContentsAddsClasses()
		{
			MockClass c = new MockClass(new MockProjectContent(), "TestClass");
			projectContent.ClassesInProjectContent.Add(c);
			projectContent.AddNamespaceContents(items, String.Empty, PythonLanguageProperties.Default, false);
			
			ArrayList expectedItems = new ArrayList();
			expectedItems.Add(c);
			
			Assert.AreEqual(expectedItems, items);
		}

		[Test]
		public void NamespaceContentsSearchedIsNullByDefault()
		{
			Assert.IsNull(projectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
		
		[Test]
		public void NamespacePassedToGetNamespaceMethodIsSaved()
		{
			projectContent.GetNamespaceContents("abc");
			Assert.AreEqual("abc", projectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
		
		[Test]
		public void GetNamespaceContentsReturnsExpectedItems()
		{
			ArrayList namespaceItems = new ArrayList();
			namespaceItems.Add("test");
			projectContent.AddExistingNamespaceContents(String.Empty, namespaceItems);
			items = projectContent.GetNamespaceContents(String.Empty);
			
			ArrayList expectedItems = new ArrayList();
			expectedItems.Add("test");
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void NamespaceExistsReturnsTrueForAddedExistingNamespace()
		{
			ArrayList items = new ArrayList();
			projectContent.AddExistingNamespaceContents("System", items);

			Assert.IsTrue(projectContent.NamespaceExists("System"));
		}
		
		[Test]
		public void NamespaceExistsReturnsFalseForUnknownNamespace()
		{
			ArrayList items = new ArrayList();
			projectContent.AddExistingNamespaceContents("System", items);
			
			Assert.IsFalse(projectContent.NamespaceExists("Unknown"));
		}
		
		[Test]
		public void GetNamespaceContentsReturnsItemsForAddedExistingNamespace()
		{
			ArrayList items = new ArrayList();
			items.Add("test");
			
			projectContent.AddExistingNamespaceContents("Math", new ArrayList());
			projectContent.AddExistingNamespaceContents("System", items);
			
			ArrayList expectedItems = new ArrayList();
			expectedItems.Add("test");
			
			Assert.AreEqual(expectedItems, projectContent.GetNamespaceContents("System"));
		}
		
		[Test]
		public void GetNamespaceContentsReturnsEmptyArrayListForUnknownNamespace()
		{
			ArrayList items = new ArrayList();
			items.Add("test");
			projectContent.AddExistingNamespaceContents("System", items);
			
			ArrayList expectedItems = new ArrayList();
			
			Assert.AreEqual(expectedItems, projectContent.GetNamespaceContents("Unknown"));
		}
		
		[Test]
		public void NamespaceUsedWhenCallingNamespaceExistsIsSaved()
		{
			projectContent.NamespaceExists("System");
			Assert.AreEqual("System", projectContent.NamespacePassedToNamespaceExistsMethod);
		}
		
		[Test]
		public void NamespaceExistsCalledIsFalseInitially()
		{
			Assert.IsFalse(projectContent.NamespaceExistsCalled);
		}
		
		[Test]
		public void NamespaceExistsCalledReturnsTrueAfterMethodCall()
		{
			projectContent.NamespaceExists("System");
			Assert.IsTrue(projectContent.NamespaceExistsCalled);
		}
		
		[Test]
		public void GetClassReturnsNullByDefault()
		{
			Assert.IsNull(projectContent.GetClass("test", 0));
		}
		
		[Test]
		public void GetClassNameReturnsClassNamePassedToGetClassMethod()
		{
			projectContent.GetClass("abc", 0);
			Assert.AreEqual("abc", projectContent.GetClassName);
		}
		
		[Test]
		public void GetClassCalledIsFalseByDefault()
		{
			Assert.IsFalse(projectContent.GetClassCalled);
		}
		
		[Test]
		public void GetClassCalledIsTrueAfterGetClassCalled()
		{
			projectContent.GetClass("abc", 0);
			Assert.IsTrue(projectContent.GetClassCalled);
		}
		
		[Test]
		public void GetClassReturnsClassEvenIfClassNameDoesNotMatchAndNoClassNameForGetClassSpecified()
		{
			MockClass c = new MockClass(projectContent, "test");
			projectContent.ClassToReturnFromGetClass = c;
			
			Assert.AreEqual(c, projectContent.GetClass("abcdef", 0));
		}
		
		[Test]
		public void GetClassReturnsNullIfClassNameDoesNotMatchClassNameForGetClassProperty()
		{
			MockClass c = new MockClass(projectContent, "test");
			projectContent.ClassToReturnFromGetClass = c;
			projectContent.ClassNameForGetClass = "test";
			
			Assert.IsNull(projectContent.GetClass("abcdef", 0));
		}
		
		[Test]
		public void GetClassReturnsClassIfClassNameMatchesClassNameForGetClassProperty()
		{
			MockClass c = new MockClass(projectContent, "test");
			projectContent.ClassToReturnFromGetClass = c;
			projectContent.ClassNameForGetClass = "test";
			
			Assert.AreEqual(c, projectContent.GetClass("test", 0));
		}
		
		[Test]
		public void NamespaceNamesHasNoItemsByDefault()
		{
			Assert.AreEqual(0, projectContent.ReferencedContents.Count);
		}
		
		[Test]
		public void NamespaceNamesContainingsNamespaceAddedToExistingNamespaces()
		{
			projectContent.AddExistingNamespaceContents("System", new ArrayList());
			List<string> expectedNames = new List<string>();
			expectedNames.Add("System");
			
			Assert.AreEqual(expectedNames, projectContent.NamespaceNames);
		}
	}
}
