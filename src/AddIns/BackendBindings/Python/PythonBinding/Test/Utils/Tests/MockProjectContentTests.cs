// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
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
		public void AddNamespaceContentsCalledIsFalseByDefault()
		{
			Assert.IsFalse(projectContent.AddNamespaceContentsCalled);
		}
		
		[Test]
		public void LookInReferencesIsFalseByDefault()
		{
			Assert.IsFalse(projectContent.LookInReferences);
		}
		
		[Test]
		public void LanguagePassedToAddNamespaceContentsIsNullByDefault()
		{
			Assert.IsNull(projectContent.LanguagePassedToAddNamespaceContents);
		}
		
		[Test]
		public void NamespaceParameterPassedToAddNamespaceContentsIsNullByDefault()
		{
			Assert.IsNull(projectContent.NamespaceAddedName);
		}
		
		[Test]
		public void AddNamespaceContentsCalledIsTrueAfterMethodCalled()
		{
			projectContent.AddNamespaceContents(items, String.Empty, PythonLanguageProperties.Default, false);
			Assert.IsTrue(projectContent.AddNamespaceContentsCalled);
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
		public void LookInReferencesSavedAfterAddNamespaceContentsMethodCalled()
		{
			projectContent.AddNamespaceContents(items, String.Empty, PythonLanguageProperties.Default, true);
			Assert.IsTrue(projectContent.LookInReferences);
		}

		[Test]
		public void LanguagePropertiesParameterSavedAfterAddNamespaceContentsMethodCalled()
		{
			projectContent.AddNamespaceContents(items, String.Empty, PythonLanguageProperties.Default, true);
			Assert.AreEqual(PythonLanguageProperties.Default, projectContent.LanguagePassedToAddNamespaceContents);
		}
		
		[Test]
		public void NamespaceParameterIsSavedAfterAddNamespaceContentsMethodCalled()
		{
			projectContent.AddNamespaceContents(items, "abc", PythonLanguageProperties.Default, false);
			Assert.AreEqual("abc", projectContent.NamespaceAddedName);
		}
		
		[Test]
		public void NamespaceContentsSearchedIsNullByDefault()
		{
			Assert.IsNull(projectContent.NamespaceContentsSearched);
		}
		
		[Test]
		public void NamespacePassedToGetNamespaceMethodIsSaved()
		{
			projectContent.GetNamespaceContents("abc");
			Assert.AreEqual("abc", projectContent.NamespaceContentsSearched);
		}
		
		[Test]
		public void GetNamespaceContentsReturnsExpectedItems()
		{
			projectContent.NamespaceContentsToReturn.Add("test");
			items = projectContent.GetNamespaceContents(String.Empty);
			
			ArrayList expectedItems = new ArrayList();
			expectedItems.Add("test");
			
			Assert.AreEqual(expectedItems, items);
		}
	}
}
