// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
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
			projectContent = new MockProjectContent();
			completion = new PythonImportCompletion(projectContent);
			
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			ParseInformation parseInfo = new ParseInformation(unit);
			c = new DefaultClass(unit, "Class");
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			namespaceItems.Add(c);
			projectContent.AddExistingNamespaceContents("System", namespaceItems);
		}

		[Test]
		public void FromDotNetSystemLibraryGetCompletionItemsReturnsAllClassesFromSystemNamespace()
		{
			List<ICompletionEntry> items = completion.GetCompletionItemsFromModule("System");
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
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
