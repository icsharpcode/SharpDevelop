// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockProjectContentTests
	{
		ICSharpCode.Scripting.Tests.Utils.MockProjectContent projectContent;
		List<ICompletionEntry> items;
		
		[SetUp]
		public void Init()
		{
			projectContent = new ICSharpCode.Scripting.Tests.Utils.MockProjectContent();
			items = new List<ICompletionEntry>();
		}
		
		[Test]
		public void AddNamespaceContentsAddsNamespaces()
		{
			projectContent.NamespacesToAdd.Add("test");
			projectContent.AddNamespaceContents(items, String.Empty, null, false);
			
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
			expectedItems.Add(new NamespaceEntry("test"));
			
			Assert.AreEqual(expectedItems, items);
		}
			
		[Test]
		public void AddNamespaceContentsAddsClasses()
		{
			MockClass c = new MockClass(new MockProjectContent(), "TestClass");
			projectContent.ClassesInProjectContent.Add(c);
			projectContent.AddNamespaceContents(items, String.Empty, null, false);
			
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
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
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			namespaceItems.Add(new NamespaceEntry("test"));
			projectContent.AddExistingNamespaceContents(String.Empty, namespaceItems);
			items = projectContent.GetNamespaceContents(String.Empty);
			
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
			expectedItems.Add(new NamespaceEntry("test"));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void NamespaceExistsReturnsTrueForAddedExistingNamespace()
		{
			List<ICompletionEntry> items = new List<ICompletionEntry>();
			projectContent.AddExistingNamespaceContents("System", items);

			Assert.IsTrue(projectContent.NamespaceExists("System"));
		}
		
		[Test]
		public void NamespaceExistsReturnsFalseForUnknownNamespace()
		{
			List<ICompletionEntry> items = new List<ICompletionEntry>();
			projectContent.AddExistingNamespaceContents("System", items);
			
			Assert.IsFalse(projectContent.NamespaceExists("Unknown"));
		}
		
		[Test]
		public void GetNamespaceContentsReturnsItemsForAddedExistingNamespace()
		{
			List<ICompletionEntry> items = new List<ICompletionEntry>();
			items.Add(new NamespaceEntry("test"));
			
			projectContent.AddExistingNamespaceContents("Math", new List<ICompletionEntry>());
			projectContent.AddExistingNamespaceContents("System", items);
			
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
			expectedItems.Add(new NamespaceEntry("test"));
			
			Assert.AreEqual(expectedItems, projectContent.GetNamespaceContents("System"));
		}
		
		[Test]
		public void GetNamespaceContentsReturnsEmptyArrayListForUnknownNamespace()
		{
			List<ICompletionEntry> items = new List<ICompletionEntry>();
			items.Add(new NamespaceEntry("test"));
			projectContent.AddExistingNamespaceContents("System", items);
			
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
			
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
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			projectContent.AddExistingNamespaceContents("System", namespaceItems);
			List<string> expectedNames = new List<string>();
			expectedNames.Add("System");
			
			Assert.AreEqual(expectedNames, projectContent.NamespaceNames);
		}
	}
}
