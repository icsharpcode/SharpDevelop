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
