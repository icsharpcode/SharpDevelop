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
	public class ImportCompletionTestFixture
	{
		ArrayList completionItems;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			ParseInformation parseInfo = new ParseInformation();
			projectContent = new MockProjectContent();
			ArrayList namespaceItems = new ArrayList();
			namespaceItems.Add("Test");
			projectContent.AddExistingNamespaceContents(String.Empty, namespaceItems);
			
			PythonImportCompletion completion = new PythonImportCompletion(projectContent);
			completionItems = completion.GetCompletionItems();
		}
		
		[Test]
		public void TestNamespaceIsAddedToCompletionItems()
		{
			Assert.Contains("Test", completionItems);
		}
		
		[Test]
		public void MathStandardPythonModuleIsAddedToCompletionItems()
		{
			Assert.Contains("math", completionItems);
		}
		
		[Test]
		public void NamespacePassedToProjectContentGetNamespaceContentsIsEmptyString()
		{
			Assert.AreEqual(String.Empty, projectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
	}
}
