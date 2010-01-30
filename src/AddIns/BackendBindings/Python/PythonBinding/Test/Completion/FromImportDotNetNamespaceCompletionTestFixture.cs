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
	public class FromImportDotNetNamespaceCompletionTestFixture
	{
		PythonImportCompletion completion;
		MockProjectContent projectContent;
		DefaultClass c;
		
		[SetUp]
		public void Init()
		{
			ParseInformation parseInfo = new ParseInformation();
			projectContent = new MockProjectContent();
			completion = new PythonImportCompletion(projectContent);
			
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			c = new DefaultClass(unit, "Class");
			ArrayList namespaceItems = new ArrayList();
			namespaceItems.Add(c);
			projectContent.AddExistingNamespaceContents("System", namespaceItems);
		}

		[Test]
		public void FromDotNetSystemLibraryGetCompletionItemsReturnsAllClassesFromSystemNamespace()
		{
			ArrayList items = completion.GetCompletionItemsFromModule("System");
			ArrayList expectedItems = new ArrayList();
			expectedItems.Add(c);
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void SystemNamespaceSearchedForWhenGetCompletionItemsMethodCalled()
		{
			completion.GetCompletionItemsFromModule("System");
			Assert.AreEqual("System", projectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
	}
}
