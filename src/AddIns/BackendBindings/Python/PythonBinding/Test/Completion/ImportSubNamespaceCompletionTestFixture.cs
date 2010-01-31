// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class ImportSubNamespaceCompletionTestFixture
	{
		List<ICompletionEntry> completionItems;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			projectContent = new MockProjectContent();
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			namespaceItems.Add(new NamespaceEntry("ICSharpCode.PythonBinding.Test"));
			projectContent.AddExistingNamespaceContents("ICSharpCode", namespaceItems);
			
			PythonImportCompletion completion = new PythonImportCompletion(projectContent);
			completionItems = completion.GetCompletionItems("ICSharpCode");
		}
		
		[Test]
		public void TestNamespaceIsAddedToCompletionItems()
		{
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
			expectedItems.Add(new NamespaceEntry("ICSharpCode.PythonBinding.Test"));
			Assert.AreEqual(expectedItems, completionItems);
		}
		
		[Test]
		public void NamespacePassedToProjectContentGetNamespaceContentsIsSubNamespaceName()
		{
			Assert.AreEqual("ICSharpCode", projectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
	}
}
