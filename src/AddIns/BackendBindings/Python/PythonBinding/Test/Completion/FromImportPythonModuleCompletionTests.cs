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
	public class FromImportPythonModuleCompletionTests
	{
		PythonImportCompletion completion;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			projectContent = new MockProjectContent();
			ParseInformation parseInfo = new ParseInformation(new DefaultCompilationUnit(projectContent));
			completion = new PythonImportCompletion(projectContent);
		}
		
		[Test]
		public void GetCompletionItemsFromModule_UnknownModule_OnlyImportAllItemsCompletionItemReturned()
		{
			List<ICompletionEntry> items = completion.GetCompletionItemsFromModule("unknown");
			List<ICompletionEntry> expectedItems = new List<ICompletionEntry>();
			expectedItems.Add(new NamespaceEntry("*"));
			Assert.AreEqual(expectedItems, items);
		}
		
		[Test]
		public void GetCompletionItemsFromModule_MathModule_ReturnsPiField()
		{
			List<ICompletionEntry> items = completion.GetCompletionItemsFromModule("math");
			IField field = PythonCompletionItemsHelper.FindFieldFromCollection("pi", items);
			Assert.IsNotNull(field);
		}
		
		[Test]
		public void GetCompletionItemsFromModule_MathModule_LastCompletionItemIsAsterisk()
		{
			List<ICompletionEntry> items = completion.GetCompletionItemsFromModule("math");
			int lastItem = items.Count - 1;			
			ICompletionEntry lastCompletionItem = items[lastItem];
			NamespaceEntry expectedCompletionItem = new NamespaceEntry("*");
			Assert.AreEqual(expectedCompletionItem, lastCompletionItem);
		}
	}
}
