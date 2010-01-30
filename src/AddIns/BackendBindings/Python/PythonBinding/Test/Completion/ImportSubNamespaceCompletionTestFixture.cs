// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class ImportSubNamespaceCompletionTestFixture
	{
		ArrayList completionItems;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			ParseInformation parseInfo = new ParseInformation();
			projectContent = new MockProjectContent();
			ArrayList namespaceItems = new ArrayList();
			namespaceItems.Add("ICSharpCode.PythonBinding.Test");
			projectContent.AddExistingNamespaceContents("ICSharpCode", namespaceItems);
			
			PythonImportCompletion completion = new PythonImportCompletion(projectContent);
			completionItems = completion.GetCompletionItems("ICSharpCode");
		}
		
		[Test]
		public void TestNamespaceIsAddedToCompletionItems()
		{
			ArrayList expectedItems = new ArrayList();
			expectedItems.Add("ICSharpCode.PythonBinding.Test");
			Assert.AreEqual(expectedItems, completionItems);
		}
		
		[Test]
		public void NamespacePassedToProjectContentGetNamespaceContentsIsSubNamespaceName()
		{
			Assert.AreEqual("ICSharpCode", projectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
	}
}
