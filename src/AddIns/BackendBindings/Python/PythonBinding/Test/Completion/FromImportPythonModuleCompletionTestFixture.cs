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
	public class FromImportPythonModuleCompletionTestFixture
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
		public void FromUnknownLibraryNoCompletionItemsReturned()
		{
			List<ICompletionEntry> items = completion.GetCompletionItemsFromModule("unknown");
			Assert.AreEqual(0, items.Count);
		}
		
		[Test]
		public void FromMathLibraryGetCompletionItemsReturnsPiField()
		{
			List<ICompletionEntry> items = completion.GetCompletionItemsFromModule("math");
			IField field = PythonCompletionItemsHelper.FindFieldFromCollection("pi", items);
			Assert.IsNotNull(field);
		}
	}
}
