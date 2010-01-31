// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockProjectContentTests
	{
		MockProjectContent projectContent;
		List<ICompletionEntry> items;
		
		[SetUp]
		public void Init()
		{
			projectContent = new MockProjectContent();
			items = new List<ICompletionEntry>();
		}
		
		[Test]
		public void AddNamespaceContentsAddsNamespaces()
		{
			projectContent.NamespacesToAdd.Add("test");
			projectContent.AddNamespaceContents(items, String.Empty, PythonLanguageProperties.Default, false);
			
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
			expectedItems.Add(new NamespaceEntry("test"));
			
			Assert.AreEqual(expectedItems, items);
		}
			
		[Test]
		public void AddNamespaceContentsAddsClasses()
		{
			MockClass c = new MockClass(new MockProjectContent(), "TestClass");
			projectContent.ClassesInProjectContent.Add(c);
			projectContent.AddNamespaceContents(items, String.Empty, PythonLanguageProperties.Default, false);
			
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
	}
}
