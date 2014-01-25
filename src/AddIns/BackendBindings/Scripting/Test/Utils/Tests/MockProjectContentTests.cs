// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

//using System;
//using System.Collections.Generic;
//using ICSharpCode.Scripting.Tests.Utils;
//using ICSharpCode.SharpDevelop.Dom;
//using NUnit.Framework;
//using UnitTesting.Tests.Utils;
//
//namespace ICSharpCode.Scripting.Tests.Utils.Tests
//{
//	[TestFixture]
//	public class MockProjectContentTests
//	{
//		ICSharpCode.Scripting.Tests.Utils.MockProjectContent projectContent;
//		List<ICompletionEntry> items;
//		
//		[SetUp]
//		public void Init()
//		{
//			projectContent = new ICSharpCode.Scripting.Tests.Utils.MockProjectContent();
//			items = new List<ICompletionEntry>();
//		}
//		
//		[Test]
//		public void AddNamespaceContents_NamespacesSetToBeAdded_AddsNamespacesToList()
//		{
//			projectContent.NamespacesToAdd.Add("test");
//			projectContent.AddNamespaceContents(items, String.Empty, null, false);
//			
//			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
//			expectedItems.Add(new NamespaceEntry("test"));
//			
//			Assert.AreEqual(expectedItems, items);
//		}
//			
//		[Test]
//		public void AddNamespaceContents_OneClassInProjectContent_AddsClassToList()
//		{
//			MockClass c = new MockClass(new MockProjectContent(), "TestClass");
//			projectContent.ClassesInProjectContent.Add(c);
//			projectContent.AddNamespaceContents(items, String.Empty, null, false);
//			
//			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
//			expectedItems.Add(c);
//			
//			Assert.AreEqual(expectedItems, items);
//		}
//
//		[Test]
//		public void NamespacePassedToGetNamespaceContentsMethod_GetNamespaceContentsNotCalled_ReturnsNull()
//		{
//			string name = projectContent.NamespacePassedToGetNamespaceContentsMethod;
//			Assert.IsNull(name);
//		}
//		
//		[Test]
//		public void NamespacePassedToGetNamespaceMethod_GetNamespaceContentsCalled_ReturnsNamespacePassedMethod()
//		{
//			projectContent.GetNamespaceContents("abc");
//			string name = projectContent.NamespacePassedToGetNamespaceContentsMethod;
//			Assert.AreEqual("abc", name);
//		}
//		
//		[Test]
//		public void GetNamespaceContents_EmptyNamespacePassed_ReturnsExpectedItemsForEmptyNamespace()
//		{
//			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
//			namespaceItems.Add(new NamespaceEntry("test"));
//			projectContent.AddExistingNamespaceContents(String.Empty, namespaceItems);
//			items = projectContent.GetNamespaceContents(String.Empty);
//			
//			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
//			expectedItems.Add(new NamespaceEntry("test"));
//			
//			Assert.AreEqual(expectedItems, items);
//		}
//		
//		[Test]
//		public void NamespaceExists_NamespaceAddedToExistingNamespaceContents_ReturnsTrue()
//		{
//			List<ICompletionEntry> items = new List<ICompletionEntry>();
//			projectContent.AddExistingNamespaceContents("System", items);
//			bool result = projectContent.NamespaceExists("System");
//			
//			Assert.IsTrue(result);
//		}
//		
//		[Test]
//		public void NamespaceExists_UnknownNamespace_ReturnsFalse()
//		{
//			List<ICompletionEntry> items = new List<ICompletionEntry>();
//			projectContent.AddExistingNamespaceContents("System", items);
//			bool result = projectContent.NamespaceExists("Unknown");
//			Assert.IsFalse(result);
//		}
//		
//		[Test]
//		public void GetNamespaceContents_AddExistingNamespaceContentsCalledWithCompletionItemsForTwoNamespaces_ReturnsItemsForSystemNamespace()
//		{
//			List<ICompletionEntry> items = new List<ICompletionEntry>();
//			items.Add(new NamespaceEntry("test"));
//			
//			projectContent.AddExistingNamespaceContents("Math", new List<ICompletionEntry>());
//			projectContent.AddExistingNamespaceContents("System", items);
//			
//			List<ICompletionEntry> actualItems = projectContent.GetNamespaceContents("System");
//			
//			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
//			expectedItems.Add(new NamespaceEntry("test"));
//			
//			Assert.AreEqual(expectedItems, actualItems);
//		}
//		
//		[Test]
//		public void GetNamespaceContents_UnknownNamespace_ReturnsEmptyArrayListF()
//		{
//			List<ICompletionEntry> items = new List<ICompletionEntry>();
//			items.Add(new NamespaceEntry("test"));
//			projectContent.AddExistingNamespaceContents("System", items);
//			
//			List<ICompletionEntry> actualItems = projectContent.GetNamespaceContents("Unknown");
//			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
//			
//			Assert.AreEqual(expectedItems, actualItems);
//		}
//		
//		[Test]
//		public void NamespaceUsedWhenCallingNamespaceExistsIsSaved()
//		{
//			projectContent.NamespaceExists("System");
//			Assert.AreEqual("System", projectContent.NamespacePassedToNamespaceExistsMethod);
//		}
//		
//		[Test]
//		public void NamespaceExistsCalled_NamespaceExistsMethodNotCalled_ReturnsFalse()
//		{
//			Assert.IsFalse(projectContent.NamespaceExistsCalled);
//		}
//		
//		[Test]
//		public void NamespaceExistsCalled_NamespaceExistsMethodCalled_ReturnsTrue()
//		{
//			projectContent.NamespaceExists("System");
//			bool result = projectContent.NamespaceExistsCalled;
//			Assert.IsTrue(result);
//		}
//		
//		[Test]
//		public void GetClass_NewInstance_ReturnsNullByDefault()
//		{
//			IClass c = projectContent.GetClass("test", 0);
//			Assert.IsNull(c);
//		}
//		
//		[Test]
//		public void GetClassName_GetClassCalledWithClassName_ReturnsClassNamePassedToGetClassMethod()
//		{
//			projectContent.GetClass("abc", 0);
//			string name = projectContent.GetClassName;
//			Assert.AreEqual("abc", name);
//		}
//		
//		[Test]
//		public void GetClass_ClassNameDoesNotMatchClassNameForGetClassProperty_ReturnsNull()
//		{
//			MockClass c = new MockClass(projectContent, "test");
//			projectContent.SetClassToReturnFromGetClass("test", c);
//			
//			Assert.IsNull(projectContent.GetClass("abcdef", 0));
//		}
//		
//		[Test]
//		public void GetClass_ClassNameMatchesClassNameForGetClassProperty_ReturnsTestClass()
//		{
//			MockClass expectedClass = new MockClass(projectContent, "test");
//			projectContent.SetClassToReturnFromGetClass("test", expectedClass);
//			
//			IClass c = projectContent.GetClass("test", 0);
//			
//			Assert.AreEqual(expectedClass, c);
//		}
//		
//		[Test]
//		public void ReferencedContents_NewInstance_HasNoItemsByDefault()
//		{
//			int count = projectContent.ReferencedContents.Count;
//			Assert.AreEqual(0, count);
//		}
//		
//		[Test]
//		public void NamespaceNames_NewInstance_HasNoItemsByDefault()
//		{
//			int count = projectContent.NamespaceNames.Count;
//			Assert.AreEqual(0, count);
//		}
//		
//		[Test]
//		public void NamespaceNames_NamespaceAddedToExistingNamespaces_NamespaceIncludedInReturnedCollection()
//		{
//			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
//			projectContent.AddExistingNamespaceContents("System", namespaceItems);
//			
//			ICollection<string> names = projectContent.NamespaceNames;
//			
//			List<string> expectedNames = new List<string>();
//			expectedNames.Add("System");
//			
//			Assert.AreEqual(expectedNames, names);
//		}
//	}
//}
