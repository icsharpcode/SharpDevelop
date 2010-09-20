// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class FromImportDotNetNamespaceCompletionTes
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
		public void GetCompletionItemsFromModule_DotNetSystemLibrary_ReturnsAllClassesFromSystemNamespaceAndAsteriskForImportAll()
		{
			List<ICompletionEntry> items = completion.GetCompletionItemsFromModule("System");
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
			expectedItems.Add(c);
			expectedItems.Add(new NamespaceEntry("*"));
			
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void GetCompletionItems_DotNetSystemLibrary_SystemNamespaceSearchedForWhenGetCompletionItemsMethodCalled()
		{
			completion.GetCompletionItemsFromModule("System");
			string expectedNamespace = "System";
			Assert.AreEqual(expectedNamespace, projectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
	}
}
