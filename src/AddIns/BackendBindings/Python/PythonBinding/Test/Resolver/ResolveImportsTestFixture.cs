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
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests the PythonResolver returns the correct namespaces for
	/// an import statement when the user presses Ctrl+Space.
	/// </summary>
	[TestFixture]
	public class ResolveImportsTestFixture
	{
		List<ICompletionEntry> results;
		PythonResolver resolver;
		MockProjectContent mockProjectContent;		
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			mockProjectContent = new MockProjectContent();
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			namespaceItems.Add(new NamespaceEntry("Test"));
			mockProjectContent.AddExistingNamespaceContents(String.Empty, namespaceItems);

			ParseInformation parseInfo = new ParseInformation(new DefaultCompilationUnit(mockProjectContent));
			
			results = resolver.CtrlSpace(0, "import".Length, parseInfo, "import", ExpressionContext.Namespace);
		}
		
		[Test]
		public void NamespaceName()
		{
			Assert.AreEqual(String.Empty, mockProjectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
				
		[Test]
		public void TestNamespaceAdded()
		{
			Assert.Contains(new NamespaceEntry("Test"), results);
		}
		
		[Test]
		public void ContainsSysModule()
		{
			Assert.Contains(new NamespaceEntry("sys"), results);
		}
	}
}
